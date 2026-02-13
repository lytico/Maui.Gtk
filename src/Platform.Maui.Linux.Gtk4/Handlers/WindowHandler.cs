using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Platform.Maui.Linux.Gtk4.Platform;

namespace Platform.Maui.Linux.Gtk4.Handlers;

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

		// Ensure AlertManager.Subscribe() is called so DI-registered
		// IAlertManagerSubscription gets picked up for DisplayAlert etc.
		if (window is Microsoft.Maui.Controls.Window mauiWindow)
		{
			try
			{
				var amProp = typeof(Microsoft.Maui.Controls.Window).GetProperty("AlertManager",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var alertManager = amProp?.GetValue(mauiWindow);
				var subscribe = alertManager?.GetType().GetMethod("Subscribe",
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				subscribe?.Invoke(alertManager, null);
			}
			catch { }
		}
	}
}
