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
	readonly List<Gtk.Label> _itemLabels = new();
	int _currentPosition;

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
		sw.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Never);

		_itemsBox = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		_itemsBox.SetHomogeneous(true);
		sw.SetChild(_itemsBox);

		return sw;
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
		handler._itemsBox?.SetOrientation(vertical ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal);
		handler.PlatformView.SetPolicy(
			vertical ? Gtk.PolicyType.Never : Gtk.PolicyType.Automatic,
			vertical ? Gtk.PolicyType.Automatic : Gtk.PolicyType.Never);
	}

	public static void MapLoop(CarouselViewHandler handler, IView view) { /* no-op */ }
	public static void MapIsBounceEnabled(CarouselViewHandler handler, IView view) { /* no-op */ }
	public static void MapIsSwipeEnabled(CarouselViewHandler handler, IView view) { /* no-op */ }
	public static void MapPeekAreaInsets(CarouselViewHandler handler, IView view) { /* no-op */ }

	void PopulateItems(CarouselView cv)
	{
		if (_itemsBox == null) return;

		// Clear existing
		foreach (var lbl in _itemLabels)
			_itemsBox.Remove(lbl);
		_itemLabels.Clear();

		if (cv.ItemsSource is not IEnumerable items) return;

		foreach (var item in items)
		{
			var label = Gtk.Label.New(item?.ToString() ?? "");
			label.SetHexpand(true);
			label.SetVexpand(true);
			label.SetHalign(Gtk.Align.Center);
			label.SetValign(Gtk.Align.Center);
			label.SetWrap(true);
			_itemLabels.Add(label);
			_itemsBox.Append(label);
		}

		if (_currentPosition < _itemLabels.Count)
			ScrollToPosition();
	}

	void ScrollToPosition()
	{
		// Gtk.ScrolledWindow child scrolling - approximate via label focus
		if (_currentPosition >= 0 && _currentPosition < _itemLabels.Count)
		{
			_itemLabels[_currentPosition].GrabFocus();
		}
	}
}
