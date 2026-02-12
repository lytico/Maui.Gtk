using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Maui.Linux.Handlers;

namespace Maui.Linux.Platform;

public abstract class GtkMauiApplication : IPlatformApplication
{
	private Gtk.Application _gtkApp = null!;
	private IApplication _mauiApp = null!;

	public IServiceProvider Services { get; protected set; } = null!;
	public IApplication Application => _mauiApp;

	protected abstract MauiApp CreateMauiApp();

	public void Run(string[] args)
	{
		_gtkApp = Gtk.Application.New("com.maui.linux", Gio.ApplicationFlags.DefaultFlags);

		_gtkApp.OnActivate += OnActivate;
		_gtkApp.OnShutdown += OnShutdown;

		var exitCode = _gtkApp.Run(args);
		Environment.ExitCode = exitCode;
	}

	private void OnActivate(Gio.Application sender, EventArgs args)
	{
		IPlatformApplication.Current = this;

		var mauiApp = CreateMauiApp();

		var rootContext = new GtkMauiContext(mauiApp.Services);
		var applicationContext = rootContext.MakeApplicationScope(this);

		Services = applicationContext.Services;

		_mauiApp = Services.GetRequiredService<IApplication>();

		// Wire up ApplicationHandler
		var appHandler = new ApplicationHandler();
		appHandler.SetMauiContext(applicationContext);
		appHandler.SetVirtualView(_mauiApp);

		// Create the window
		CreatePlatformWindow(applicationContext);

		// Notify subclasses the app is fully started
		OnStarted();
	}

	/// <summary>
	/// Called after the MAUI application and window have been fully initialized.
	/// Override to perform post-startup actions like starting debug agents.
	/// </summary>
	protected virtual void OnStarted() { }

	private void CreatePlatformWindow(GtkMauiContext applicationContext)
	{
		var virtualWindow = _mauiApp.CreateWindow(null);

		var gtkWindow = new Gtk.Window();
		gtkWindow.SetDefaultSize(800, 600);
		gtkWindow.SetTitle(virtualWindow.Title ?? "Maui.Linux");

		var windowContext = applicationContext.MakeWindowScope(gtkWindow);
		windowContext.AddSpecific(gtkWindow);

		var windowHandler = new WindowHandler();
		windowHandler.SetMauiContext(windowContext);
		windowHandler.SetVirtualView(virtualWindow);

		gtkWindow.SetApplication(_gtkApp);
		gtkWindow.Show();

		virtualWindow.Created();
		virtualWindow.Activated();
	}

	private void OnShutdown(Gio.Application sender, EventArgs args)
	{
		_gtkApp.OnActivate -= OnActivate;
		_gtkApp.OnShutdown -= OnShutdown;
	}
}
