using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class BorderHandler : GtkViewHandler<IBorderView, Gtk.Frame>
{
	public static new IPropertyMapper<IBorderView, BorderHandler> Mapper =
		new PropertyMapper<IBorderView, BorderHandler>(ViewMapper)
		{
			[nameof(IBorderView.Content)] = MapContent,
		};

	public BorderHandler() : base(Mapper)
	{
	}

	protected override Gtk.Frame CreatePlatformView()
	{
		return Gtk.Frame.New(null);
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		if (VirtualView is ICrossPlatformLayout crossPlatform)
			return crossPlatform.CrossPlatformMeasure(widthConstraint, heightConstraint);

		return base.GetDesiredSize(widthConstraint, heightConstraint);
	}

	public override void PlatformArrange(Rect rect)
	{
		base.PlatformArrange(rect);
		if (VirtualView is ICrossPlatformLayout crossPlatform)
			crossPlatform.CrossPlatformArrange(rect);
	}

	public static void MapContent(BorderHandler handler, IBorderView border)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (border.PresentedContent != null)
		{
			var platformContent = (Gtk.Widget)border.PresentedContent.ToPlatform(handler.MauiContext);
			handler.PlatformView?.SetChild(platformContent);
		}
	}
}
