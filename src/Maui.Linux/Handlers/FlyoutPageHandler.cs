using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class FlyoutPageHandler : GtkViewHandler<IFlyoutView, Gtk.Box>
{
	Gtk.Paned? _paned;
	Gtk.Button? _toggleButton;
	int _lastPosition = 250;

	public static new IPropertyMapper<IFlyoutView, FlyoutPageHandler> Mapper =
		new PropertyMapper<IFlyoutView, FlyoutPageHandler>(ViewMapper)
		{
			[nameof(IFlyoutView.Flyout)] = MapFlyout,
			[nameof(IFlyoutView.Detail)] = MapDetail,
			[nameof(IFlyoutView.IsPresented)] = MapIsPresented,
		};

	public FlyoutPageHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		var container = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		container.SetVexpand(true);
		container.SetHexpand(true);

		// Toggle bar with hamburger button
		var toggleBar = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		_toggleButton = Gtk.Button.New();
		_toggleButton.SetLabel("☰");
		_toggleButton.AddCssClass("flat");
		_toggleButton.SetTooltipText("Toggle sidebar");
		_toggleButton.OnClicked += OnToggleClicked;
		toggleBar.Append(_toggleButton);

		var sep = Gtk.Separator.New(Gtk.Orientation.Horizontal);

		container.Append(toggleBar);
		container.Append(sep);

		// Paned for flyout + detail
		_paned = Gtk.Paned.New(Gtk.Orientation.Horizontal);
		_paned.SetPosition(250);
		_paned.SetVexpand(true);
		_paned.SetHexpand(true);
		_paned.SetResizeStartChild(false);
		_paned.SetShrinkStartChild(false);
		_paned.SetResizeEndChild(true);
		_paned.SetShrinkEndChild(false);
		container.Append(_paned);

		return container;
	}

	void OnToggleClicked(Gtk.Button sender, EventArgs args)
	{
		if (_paned == null) return;

		var startChild = _paned.GetStartChild();
		if (startChild == null) return;

		if (startChild.GetVisible())
		{
			// Collapse: save position, hide sidebar
			_lastPosition = _paned.GetPosition();
			startChild.SetVisible(false);
			_paned.SetPosition(0);
		}
		else
		{
			// Expand: restore sidebar
			startChild.SetVisible(true);
			_paned.SetPosition(_lastPosition);
		}

		// Sync MAUI's IsPresented
		if (VirtualView is Microsoft.Maui.Controls.FlyoutPage fp)
			fp.IsPresented = startChild.GetVisible();
	}

	protected override void DisconnectHandler(Gtk.Box platformView)
	{
		if (_toggleButton != null)
			_toggleButton.OnClicked -= OnToggleClicked;
		base.DisconnectHandler(platformView);
	}

	public static void MapFlyout(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (flyoutView.Flyout != null && handler._paned != null)
		{
			var platformFlyout = (Gtk.Widget)flyoutView.Flyout.ToPlatform(handler.MauiContext);
			handler._paned.SetStartChild(platformFlyout);
		}
	}

	public static void MapDetail(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (flyoutView.Detail != null && handler._paned != null)
		{
			var platformDetail = (Gtk.Widget)flyoutView.Detail.ToPlatform(handler.MauiContext);
			handler._paned.SetEndChild(platformDetail);
		}
	}

	public static void MapIsPresented(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		if (handler._paned == null) return;

		var startChild = handler._paned.GetStartChild();
		if (startChild == null) return;

		if (flyoutView.IsPresented)
		{
			startChild.SetVisible(true);
			handler._paned.SetPosition(handler._lastPosition);
		}
		else
		{
			handler._lastPosition = handler._paned.GetPosition();
			startChild.SetVisible(false);
			handler._paned.SetPosition(0);
		}
	}
}
