using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// Handler for SwipeView. Swipe gestures are primarily mobile; on desktop Linux
/// this acts as a passthrough container rendering the SwipeView's content directly.
/// </summary>
public class SwipeViewHandler : GtkViewHandler<IView, Gtk.Box>
{
	public static new IPropertyMapper<IView, SwipeViewHandler> Mapper =
		new PropertyMapper<IView, SwipeViewHandler>(ViewHandler.ViewMapper)
		{
			["Content"] = MapContent,
			["LeftItems"] = MapSwipeItems,
			["RightItems"] = MapSwipeItems,
			["TopItems"] = MapSwipeItems,
			["BottomItems"] = MapSwipeItems,
		};

	public SwipeViewHandler() : base(Mapper) { }

	protected override Gtk.Box CreatePlatformView()
	{
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		box.SetVexpand(true);
		box.SetHexpand(true);
		return box;
	}

	public static void MapContent(SwipeViewHandler handler, IView view)
	{
		if (view is not SwipeView swipeView || handler.MauiContext == null)
			return;

		if (swipeView.Content != null)
		{
			var platformContent = (Gtk.Widget)swipeView.Content.ToPlatform(handler.MauiContext);
			platformContent.SetVexpand(true);
			platformContent.SetHexpand(true);

			// Remove existing children
			while (handler.PlatformView.GetFirstChild() is Gtk.Widget child)
				handler.PlatformView.Remove(child);

			handler.PlatformView.Append(platformContent);
		}
	}

	public static void MapSwipeItems(SwipeViewHandler handler, IView view)
	{
		// Swipe items are a mobile gesture concept; no-op on desktop Linux.
	}
}
