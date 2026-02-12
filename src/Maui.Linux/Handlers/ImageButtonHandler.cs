using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class ImageButtonHandler : GtkViewHandler<IImageButton, Gtk.Button>
{
	public static new IPropertyMapper<IImageButton, ImageButtonHandler> Mapper =
		new PropertyMapper<IImageButton, ImageButtonHandler>(ViewMapper)
		{
			[nameof(IImageButton.Padding)] = MapPadding,
		};

	public ImageButtonHandler() : base(Mapper)
	{
	}

	protected override Gtk.Button CreatePlatformView()
	{
		var button = Gtk.Button.New();
		var image = Gtk.Image.New();
		button.SetChild(image);
		return button;
	}

	protected override void ConnectHandler(Gtk.Button platformView)
	{
		base.ConnectHandler(platformView);
		platformView.OnClicked += OnClicked;
	}

	protected override void DisconnectHandler(Gtk.Button platformView)
	{
		platformView.OnClicked -= OnClicked;
		base.DisconnectHandler(platformView);
	}

	void OnClicked(Gtk.Button sender, EventArgs args)
	{
		VirtualView?.Clicked();
		VirtualView?.Released();
	}

	public static void MapPadding(ImageButtonHandler handler, IImageButton imageButton)
	{
		var p = imageButton.Padding;
		handler.PlatformView?.SetMarginStart((int)p.Left);
		handler.PlatformView?.SetMarginEnd((int)p.Right);
		handler.PlatformView?.SetMarginTop((int)p.Top);
		handler.PlatformView?.SetMarginBottom((int)p.Bottom);
	}
}
