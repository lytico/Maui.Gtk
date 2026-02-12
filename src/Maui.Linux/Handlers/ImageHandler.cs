using Microsoft.Maui;
using IImage = Microsoft.Maui.IImage;

namespace Maui.Linux.Handlers;

public class ImageHandler : GtkViewHandler<IImage, Gtk.Picture>
{
	public static new IPropertyMapper<IImage, ImageHandler> Mapper =
		new PropertyMapper<IImage, ImageHandler>(ViewMapper)
		{
			[nameof(IImage.Aspect)] = MapAspect,
		};

	public ImageHandler() : base(Mapper)
	{
	}

	protected override Gtk.Picture CreatePlatformView()
	{
		return Gtk.Picture.New();
	}

	public static void MapAspect(ImageHandler handler, IImage image)
	{
		handler.PlatformView?.SetContentFit(image.Aspect switch
		{
			Aspect.AspectFit => Gtk.ContentFit.Contain,
			Aspect.AspectFill => Gtk.ContentFit.Cover,
			Aspect.Fill => Gtk.ContentFit.Fill,
			_ => Gtk.ContentFit.Contain
		});
	}
}
