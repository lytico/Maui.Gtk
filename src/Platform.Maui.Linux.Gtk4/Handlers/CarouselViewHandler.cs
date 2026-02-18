using System.Collections;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// CarouselView handler. Renders items in a horizontal (or vertical) scrollable
/// container with snap-to-item behavior. Backed by Gtk.ScrolledWindow + Gtk.Box.
/// </summary>
public class CarouselViewHandler : GtkViewHandler<IView, Gtk.ScrolledWindow>
{
	Gtk.Box? _itemsBox;
	readonly List<Gtk.Widget> _itemWidgets = new();
	int _currentPosition;
	bool _isVertical;
	Gtk.Adjustment? _scrollAdj;

	public static new IPropertyMapper<IView, CarouselViewHandler> Mapper =
		new PropertyMapper<IView, CarouselViewHandler>(ViewHandler.ViewMapper)
		{
			["ItemsSource"] = MapItemsSource,
			["Position"] = MapPosition,
			["CurrentItem"] = MapCurrentItem,
			["Loop"] = MapLoop,
			["IsBounceEnabled"] = MapIsBounceEnabled,
			["IsSwipeEnabled"] = MapIsSwipeEnabled,
			["PeekAreaInsets"] = MapPeekAreaInsets,
			["ItemTemplate"] = MapItemsSource,
			["ItemsLayout"] = MapItemsLayout,
		};

	public CarouselViewHandler() : base(Mapper) { }

	protected override Gtk.ScrolledWindow CreatePlatformView()
	{
		var sw = Gtk.ScrolledWindow.New();
		sw.SetVexpand(true);
		sw.SetHexpand(true);
		sw.SetSizeRequest(-1, 150);
		// Hide scrollbar — navigation is via buttons/swipe/snap only
		sw.SetPolicy(Gtk.PolicyType.External, Gtk.PolicyType.Never);

		_itemsBox = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		_itemsBox.SetHomogeneous(true);
		sw.SetChild(_itemsBox);

		return sw;
	}

	protected override void ConnectHandler(Gtk.ScrolledWindow platformView)
	{
		base.ConnectHandler(platformView);

		// Hook scroll adjustment for snap-to-item on scroll end
		_scrollAdj = _isVertical ? platformView.GetVadjustment() : platformView.GetHadjustment();
		if (_scrollAdj != null)
			_scrollAdj.OnNotify += OnScrollChanged;

		// Add swipe gesture for snap navigation
		var swipe = Gtk.GestureSwipe.New();
		swipe.OnSwipe += OnSwipe;
		platformView.AddController(swipe);

		// Resize cards when viewport changes
		platformView.OnNotify += OnViewportNotify;

		// Populate on connect since ItemsSource may already be set
		if (VirtualView is CarouselView cv)
			PopulateItems(cv);
	}

	protected override void DisconnectHandler(Gtk.ScrolledWindow platformView)
	{
		if (_animTimerId != 0)
		{
			GLib.Functions.SourceRemove(_animTimerId);
			_animTimerId = 0;
		}
		if (_scrollAdj != null)
			_scrollAdj.OnNotify -= OnScrollChanged;
		platformView.OnNotify -= OnViewportNotify;
		base.DisconnectHandler(platformView);
	}

	void OnViewportNotify(GObject.Object sender, GObject.Object.NotifySignalArgs args)
	{
		if (args.Pspec.GetName() == "default-width" || args.Pspec.GetName() == "default-height")
		{
			ResizeCardsToViewport();
			ScrollToPosition(animate: false);
		}
	}

	void OnSwipe(Gtk.GestureSwipe sender, Gtk.GestureSwipe.SwipeSignalArgs args)
	{
		if (_itemWidgets.Count == 0 || VirtualView is not CarouselView cv) return;

		int newPos;
		if (_isVertical)
			newPos = args.VelocityY < 0 ? _currentPosition + 1 : _currentPosition - 1;
		else
			newPos = args.VelocityX < 0 ? _currentPosition + 1 : _currentPosition - 1;

		if (cv.Loop)
			newPos = ((newPos % _itemWidgets.Count) + _itemWidgets.Count) % _itemWidgets.Count;
		else
			newPos = Math.Clamp(newPos, 0, _itemWidgets.Count - 1);

		if (newPos != _currentPosition)
		{
			_currentPosition = newPos;
			cv.Position = newPos;
			ScrollToPosition();
		}
	}

	uint _snapTimerId;
	void OnScrollChanged(GObject.Object sender, GObject.Object.NotifySignalArgs args)
	{
		if (args.Pspec.GetName() != "value") return;
		// Debounce: snap after scrolling stops
		if (_snapTimerId != 0) return; // already scheduled
		_snapTimerId = GLib.Functions.TimeoutAdd(0, 300, () =>
		{
			_snapTimerId = 0;
			SnapToNearest();
			return false;
		});
	}

	void SnapToNearest()
	{
		if (_itemWidgets.Count == 0 || _scrollAdj == null) return;

		double scrollPos = _scrollAdj.GetValue();
		double itemSize = _isVertical
			? (_itemWidgets[0].GetAllocatedHeight())
			: (_itemWidgets[0].GetAllocatedWidth());
		if (itemSize <= 0) return;

		int nearest = (int)Math.Round(scrollPos / itemSize);
		nearest = Math.Clamp(nearest, 0, _itemWidgets.Count - 1);

		if (nearest != _currentPosition)
		{
			_currentPosition = nearest;
			if (VirtualView is CarouselView cv)
				cv.Position = nearest;
		}
		ScrollToPosition();
	}

	public static void MapItemsSource(CarouselViewHandler handler, IView view)
	{
		if (view is not CarouselView cv) return;
		handler.PopulateItems(cv);
	}

	public static void MapPosition(CarouselViewHandler handler, IView view)
	{
		if (view is not CarouselView cv) return;
		handler._currentPosition = cv.Position;
		handler.ScrollToPosition();
	}

	public static void MapCurrentItem(CarouselViewHandler handler, IView view)
	{
		// Sync'd through Position
	}

	public static void MapItemsLayout(CarouselViewHandler handler, IView view)
	{
		if (view is not CarouselView cv) return;
		bool vertical = cv.ItemsLayout is LinearItemsLayout l && l.Orientation == ItemsLayoutOrientation.Vertical;
		handler._isVertical = vertical;
		handler._itemsBox?.SetOrientation(vertical ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal);
		handler.PlatformView.SetPolicy(
			vertical ? Gtk.PolicyType.Never : Gtk.PolicyType.External,
			vertical ? Gtk.PolicyType.External : Gtk.PolicyType.Never);
		// Re-hook scroll adjustment for new orientation
		if (handler._scrollAdj != null)
			handler._scrollAdj.OnNotify -= handler.OnScrollChanged;
		handler._scrollAdj = vertical ? handler.PlatformView.GetVadjustment() : handler.PlatformView.GetHadjustment();
		if (handler._scrollAdj != null)
			handler._scrollAdj.OnNotify += handler.OnScrollChanged;
	}

	public static void MapPeekAreaInsets(CarouselViewHandler handler, IView view)
	{
		if (view is not CarouselView cv) return;
		var insets = cv.PeekAreaInsets;
		handler._itemsBox?.SetMarginStart((int)insets.Left);
		handler._itemsBox?.SetMarginEnd((int)insets.Right);
		handler._itemsBox?.SetMarginTop((int)insets.Top);
		handler._itemsBox?.SetMarginBottom((int)insets.Bottom);
	}

	public static void MapLoop(CarouselViewHandler handler, IView view) { /* stored on CarouselView, used by snap */ }
	public static void MapIsBounceEnabled(CarouselViewHandler handler, IView view) { /* no-op */ }
	public static void MapIsSwipeEnabled(CarouselViewHandler handler, IView view) { /* no-op */ }

	void PopulateItems(CarouselView cv)
	{
		if (_itemsBox == null) return;

		// Clear existing
		foreach (var w in _itemWidgets)
			_itemsBox.Remove(w);
		_itemWidgets.Clear();

		if (cv.ItemsSource is not IEnumerable items) return;

		foreach (var item in items)
		{
			var card = Gtk.Box.New(Gtk.Orientation.Vertical, 4);
			card.SetVexpand(true);
			card.SetHalign(Gtk.Align.Fill);
			card.SetValign(Gtk.Align.Fill);

			var label = Gtk.Label.New(item?.ToString() ?? "");
			label.SetWrap(true);
			label.SetHalign(Gtk.Align.Center);
			label.SetValign(Gtk.Align.Center);
			label.SetHexpand(true);
			card.Append(label);

			_itemWidgets.Add(card);
			_itemsBox.Append(card);
		}

		// Size cards to fill viewport after layout
		ResizeCardsToViewport();

		if (_currentPosition < _itemWidgets.Count)
			ScrollToPosition(animate: false);
	}

	void ResizeCardsToViewport()
	{
		int viewportWidth = PlatformView.GetAllocatedWidth();
		int viewportHeight = PlatformView.GetAllocatedHeight();
		if (viewportWidth <= 0) viewportWidth = 400;
		if (viewportHeight <= 0) viewportHeight = 150;

		foreach (var card in _itemWidgets)
			card.SetSizeRequest(viewportWidth, viewportHeight);
	}

	uint _animTimerId;

	void ScrollToPosition(bool animate = true)
	{
		if (_currentPosition < 0 || _currentPosition >= _itemWidgets.Count || _scrollAdj == null)
			return;

		var target = _itemWidgets[_currentPosition];
		double itemSize = _isVertical ? target.GetAllocatedHeight() : target.GetAllocatedWidth();
		if (itemSize <= 0) { target.GrabFocus(); return; }

		double targetScroll = _currentPosition * itemSize;

		if (!animate)
		{
			_scrollAdj.SetValue(targetScroll);
			return;
		}

		AnimateScrollTo(targetScroll);
	}

	void AnimateScrollTo(double target)
	{
		// Cancel any running animation
		if (_animTimerId != 0)
		{
			GLib.Functions.SourceRemove(_animTimerId);
			_animTimerId = 0;
		}

		if (_scrollAdj == null) return;

		double start = _scrollAdj.GetValue();
		double distance = target - start;
		if (Math.Abs(distance) < 1)
		{
			_scrollAdj.SetValue(target);
			return;
		}

		const int durationMs = 250;
		const int stepMs = 16; // ~60fps
		int elapsed = 0;

		_animTimerId = GLib.Functions.TimeoutAdd(0, stepMs, () =>
		{
			elapsed += stepMs;
			double t = Math.Min(1.0, (double)elapsed / durationMs);
			// Ease-out cubic: 1 - (1-t)^3
			double eased = 1.0 - Math.Pow(1.0 - t, 3);
			_scrollAdj!.SetValue(start + distance * eased);

			if (t >= 1.0)
			{
				_animTimerId = 0;
				return false;
			}
			return true;
		});
	}
}
