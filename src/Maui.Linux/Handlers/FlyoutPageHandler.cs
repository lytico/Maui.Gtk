using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class FlyoutPageHandler : GtkViewHandler<IFlyoutView, Gtk.Paned>
{
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

	protected override Gtk.Paned CreatePlatformView()
	{
		var paned = Gtk.Paned.New(Gtk.Orientation.Horizontal);
		paned.SetPosition(250); // default flyout width
		paned.SetVexpand(true);
		paned.SetHexpand(true);
		paned.SetResizeStartChild(false);
		paned.SetShrinkStartChild(false);
		paned.SetResizeEndChild(true);
		paned.SetShrinkEndChild(false);
		return paned;
	}

	public static void MapFlyout(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (flyoutView.Flyout != null)
		{
			var platformFlyout = (Gtk.Widget)flyoutView.Flyout.ToPlatform(handler.MauiContext);
			handler.PlatformView?.SetStartChild(platformFlyout);
		}
	}

	public static void MapDetail(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (flyoutView.Detail != null)
		{
			var platformDetail = (Gtk.Widget)flyoutView.Detail.ToPlatform(handler.MauiContext);
			handler.PlatformView?.SetEndChild(platformDetail);
		}
	}

	public static void MapIsPresented(FlyoutPageHandler handler, IFlyoutView flyoutView)
	{
		var startChild = handler.PlatformView?.GetStartChild();
		startChild?.SetVisible(flyoutView.IsPresented);
	}
}
