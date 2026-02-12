using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class PageHandler : GtkViewHandler<IContentView, Gtk.Box>
{
	public static new IPropertyMapper<IContentView, PageHandler> Mapper =
		new PropertyMapper<IContentView, PageHandler>(ViewMapper)
		{
			[nameof(IContentView.Content)] = MapContent,
		};

	public PageHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		box.SetVexpand(true);
		box.SetHexpand(true);

		// Wrap content in a ScrolledWindow so pages can scroll
		var scrolled = Gtk.ScrolledWindow.New();
		scrolled.SetVexpand(true);
		scrolled.SetHexpand(true);
		scrolled.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic);
		box.Append(scrolled);

		return box;
	}

	public static void MapContent(PageHandler handler, IContentView page)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		// Find the ScrolledWindow inside our Box
		var box = handler.PlatformView;
		var scrolled = box.GetFirstChild() as Gtk.ScrolledWindow;
		if (scrolled == null)
			return;

		if (page.PresentedContent != null)
		{
			var platformContent = (Gtk.Widget)page.PresentedContent.ToPlatform(handler.MauiContext);
			platformContent.SetVexpand(true);
			platformContent.SetHexpand(true);
			scrolled.SetChild(platformContent);
		}

		// Propagate layout dirty to ancestor layout panels
		Gtk.Widget? current = box.GetParent();
		while (current != null)
		{
			if (current is Platform.GtkLayoutPanel panel)
			{
				panel.LayoutDirty = true;
			}
			current = current.GetParent();
		}
	}

	public override void SetVirtualView(IView view)
	{
		base.SetVirtualView(view);

		// Trigger content mapping
		Mapper.UpdateProperties(this, VirtualView);
	}
}
