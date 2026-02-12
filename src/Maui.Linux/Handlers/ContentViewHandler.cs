using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class ContentViewHandler : GtkViewHandler<IContentView, Gtk.Box>
{
	public static new IPropertyMapper<IContentView, ContentViewHandler> Mapper =
		new PropertyMapper<IContentView, ContentViewHandler>(ViewMapper)
		{
			[nameof(IContentView.Content)] = MapContent,
		};

	public ContentViewHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		box.SetVexpand(true);
		box.SetHexpand(true);
		return box;
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

	public static void MapContent(ContentViewHandler handler, IContentView contentView)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		var box = handler.PlatformView;
		while (box.GetFirstChild() != null)
			box.Remove(box.GetFirstChild()!);

		if (contentView.PresentedContent != null)
		{
			var platformContent = (Gtk.Widget)contentView.PresentedContent.ToPlatform(handler.MauiContext);
			platformContent.SetVexpand(true);
			platformContent.SetHexpand(true);
			box.Append(platformContent);
		}
	}
}
