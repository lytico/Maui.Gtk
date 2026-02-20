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
			[nameof(IWindow.Width)] = MapWidth,
			[nameof(IWindow.Height)] = MapHeight,
			[nameof(IWindow.X)] = MapX,
			[nameof(IWindow.Y)] = MapY,
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

		platformView.OnCloseRequest += OnCloseRequest;
		platformView.OnNotify += OnNotifyIsActive;
	}

	protected override void DisconnectHandler(Gtk.Window platformView)
	{
		platformView.OnCloseRequest -= OnCloseRequest;
		platformView.OnNotify -= OnNotifyIsActive;
		base.DisconnectHandler(platformView);
	}

	private bool OnCloseRequest(Gtk.Window sender, EventArgs args)
	{
		if (VirtualView != null)
		{
			GtkMauiApplication.Current.UnregisterWindow(VirtualView);
			VirtualView.Destroying();
		}
		return false; // allow GTK to destroy the window
	}

	private void OnNotifyIsActive(GObject.Object sender, GObject.Object.NotifySignalArgs args)
	{
		if (args.Pspec.GetName() != "is-active" || VirtualView == null) return;

		if (PlatformView?.GetIsActive() == true)
			VirtualView.Activated();
		else
			VirtualView.Deactivated();
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

		// Apply MenuBar from the content page
		if (handler.PlatformView != null && window.Content is Microsoft.Maui.Controls.Page page)
		{
			GtkMenuBarManager.ApplyToWindow(handler.PlatformView, page);
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

	public static void MapWidth(WindowHandler handler, IWindow window)
	{
		if (handler.PlatformView == null || window.Width < 0) return;
		handler.PlatformView.GetDefaultSize(out _, out var h);
		handler.PlatformView.SetDefaultSize((int)window.Width, h > 0 ? h : 600);
	}

	public static void MapHeight(WindowHandler handler, IWindow window)
	{
		if (handler.PlatformView == null || window.Height < 0) return;
		handler.PlatformView.GetDefaultSize(out var w, out _);
		handler.PlatformView.SetDefaultSize(w > 0 ? w : 800, (int)window.Height);
	}

	public static void MapX(WindowHandler handler, IWindow window)
	{
		// GTK4 on Wayland does not support setting window position.
		// On X11, this would require platform-specific code.
	}

	public static void MapY(WindowHandler handler, IWindow window)
	{
		// GTK4 on Wayland does not support setting window position.
		// On X11, this would require platform-specific code.
	}
}
