using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Maui.Linux.Platform;

namespace Maui.Linux.Handlers;

public class WindowHandler : ElementHandler<IWindow, Gtk.Window>
{
	public static IPropertyMapper<IWindow, WindowHandler> Mapper =
		new PropertyMapper<IWindow, WindowHandler>(ElementHandler.ElementMapper)
		{
			[nameof(IWindow.Title)] = MapTitle,
			[nameof(IWindow.Content)] = MapContent,
		};

	public static CommandMapper<IWindow, WindowHandler> CommandMapper = new(ElementCommandMapper)
	{
	};

	public WindowHandler() : base(Mapper, CommandMapper)
	{
	}

	public WindowHandler(IPropertyMapper? mapper) : base(mapper ?? Mapper, CommandMapper)
	{
	}

	protected override Gtk.Window CreatePlatformElement()
	{
		return MauiContext?.Services.GetService(typeof(Gtk.Window)) as Gtk.Window
			?? new Gtk.Window();
	}

	protected override void ConnectHandler(Gtk.Window platformView)
	{
		base.ConnectHandler(platformView);

		if (platformView.GetChild() == null)
		{
			platformView.SetChild(new WindowRootViewContainer());
		}
	}

	public static void MapTitle(WindowHandler handler, IWindow window)
	{
		handler.PlatformView?.SetTitle(window.Title ?? string.Empty);
	}

	public static void MapContent(WindowHandler handler, IWindow window)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

		var child = handler.PlatformView?.GetChild();
		if (child is WindowRootViewContainer container && window.Content != null)
		{
			var platformContent = (Gtk.Widget)window.Content.ToPlatform(handler.MauiContext);
			container.AddPage(platformContent);
		}
	}
}
