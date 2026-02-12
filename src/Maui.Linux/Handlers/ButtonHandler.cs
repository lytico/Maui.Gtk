using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class ButtonHandler : GtkViewHandler<IButton, Gtk.Button>
{
	public static new IPropertyMapper<IButton, ButtonHandler> Mapper =
		new PropertyMapper<IButton, ButtonHandler>(ViewMapper)
		{
			[nameof(ITextButton.Text)] = MapText,
			[nameof(ITextButton.TextColor)] = MapTextColor,
			[nameof(IButton.Padding)] = MapPadding,
		};

	public ButtonHandler() : base(Mapper)
	{
	}

	protected override Gtk.Button CreatePlatformView()
	{
		return Gtk.Button.New();
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

	public static void MapText(ButtonHandler handler, IButton button)
	{
		if (button is ITextButton textButton)
			handler.PlatformView?.SetLabel(textButton.Text ?? string.Empty);
	}

	public static void MapTextColor(ButtonHandler handler, IButton button)
	{
		if (button is ITextStyle textStyle && textStyle.TextColor != null)
			handler.ApplyCss(handler.PlatformView, $"color: {ToGtkColor(textStyle.TextColor)};");
	}

	public static void MapPadding(ButtonHandler handler, IButton button)
	{
		var p = button.Padding;
		handler.PlatformView?.SetMarginStart((int)p.Left);
		handler.PlatformView?.SetMarginEnd((int)p.Right);
		handler.PlatformView?.SetMarginTop((int)p.Top);
		handler.PlatformView?.SetMarginBottom((int)p.Bottom);
	}
}
