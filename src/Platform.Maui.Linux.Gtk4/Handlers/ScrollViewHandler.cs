using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class ScrollViewHandler : GtkViewHandler<IScrollView, Gtk.ScrolledWindow>
{
	public static new IPropertyMapper<IScrollView, ScrollViewHandler> Mapper =
		new PropertyMapper<IScrollView, ScrollViewHandler>(ViewMapper)
		{
			[nameof(IScrollView.Content)] = MapContent,
			[nameof(IScrollView.Orientation)] = MapOrientation,
			[nameof(IScrollView.HorizontalScrollBarVisibility)] = MapHorizontalScrollBarVisibility,
			[nameof(IScrollView.VerticalScrollBarVisibility)] = MapVerticalScrollBarVisibility,
		};

	public ScrollViewHandler() : base(Mapper)
	{
	}

	protected override Gtk.ScrolledWindow CreatePlatformView()
	{
		var scrolled = Gtk.ScrolledWindow.New();
		scrolled.SetVexpand(true);
		scrolled.SetHexpand(true);
		return scrolled;
	}

	public static void MapContent(ScrollViewHandler handler, IScrollView scrollView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (scrollView.PresentedContent != null)
		{
			var platformContent = (Gtk.Widget)scrollView.PresentedContent.ToPlatform(handler.MauiContext);
			handler.PlatformView?.SetChild(platformContent);
		}
	}

	public static void MapOrientation(ScrollViewHandler handler, IScrollView scrollView)
	{
		var (hPolicy, vPolicy) = scrollView.Orientation switch
		{
			ScrollOrientation.Horizontal => (Gtk.PolicyType.Automatic, Gtk.PolicyType.Never),
			ScrollOrientation.Vertical => (Gtk.PolicyType.Never, Gtk.PolicyType.Automatic),
			ScrollOrientation.Both => (Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic),
			ScrollOrientation.Neither => (Gtk.PolicyType.Never, Gtk.PolicyType.Never),
			_ => (Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic)
		};
		handler.PlatformView?.SetPolicy(hPolicy, vPolicy);
	}

	public static void MapHorizontalScrollBarVisibility(ScrollViewHandler handler, IScrollView scrollView)
	{
	}

	public static void MapVerticalScrollBarVisibility(ScrollViewHandler handler, IScrollView scrollView)
	{
	}

	public override void PlatformArrange(Rect rect)
	{
		base.PlatformArrange(rect);

		// When the ScrollView is resized, re-measure and re-arrange its content
		// so nested layouts adapt to the new available width.
		if (VirtualView is ICrossPlatformLayout crossPlatform)
		{
			crossPlatform.CrossPlatformMeasure(rect.Width, rect.Height);
			crossPlatform.CrossPlatformArrange(new Rect(0, 0, rect.Width, rect.Height));
		}
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		if (VirtualView is ICrossPlatformLayout crossPlatform)
			return crossPlatform.CrossPlatformMeasure(widthConstraint, heightConstraint);

		return base.GetDesiredSize(widthConstraint, heightConstraint);
	}
}
