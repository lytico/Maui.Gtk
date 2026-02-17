using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Platform.Maui.Linux.Gtk4.Handlers;
using System.Reflection;

namespace Platform.Maui.Linux.Gtk4.Platform;

public abstract class GtkMauiApplication : IPlatformApplication
{
	private const string DefaultApplicationId = "com.maui.linux";
	private const string MauiApplicationIdMetadataKey = "MauiApplicationId";
	private Gtk.Application _gtkApp = null!;
	private IApplication _mauiApp = null!;
	private string _desktopEntryName = "MAUI App";

	public IServiceProvider Services { get; protected set; } = null!;
	public IApplication Application => _mauiApp;
	protected virtual string ApplicationId => ResolveApplicationId() ?? DefaultApplicationId;
	protected virtual bool CreateDesktopEntry => true;
	protected virtual string DesktopEntryName => _desktopEntryName;

	protected abstract MauiApp CreateMauiApp();

	public void Run(string[] args)
	{
		var applicationId = string.IsNullOrWhiteSpace(ApplicationId) ? null : ApplicationId;
		_gtkApp = Gtk.Application.New(applicationId, Gio.ApplicationFlags.DefaultFlags);

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
		InvokeLifecycleEvents<GtkApplicationActivated>(del => del(sender));

		_mauiApp = Services.GetRequiredService<IApplication>();
		InvokeLifecycleEvents<GtkMauiApplicationCreated>(del => del(_mauiApp));

		// Wire up ApplicationHandler
		var appHandler = new ApplicationHandler();
		appHandler.SetMauiContext(applicationContext);
		appHandler.SetVirtualView(_mauiApp);

		// Create the window
		CreatePlatformWindow(applicationContext);
		EnsureDesktopEntry();

		// Notify subclasses the app is fully started
		OnStarted();
	}

	private string? ResolveApplicationId()
	{
		if (TryGetApplicationId(GetType().Assembly, out var applicationId))
			return applicationId;

		var entryAssembly = Assembly.GetEntryAssembly();
		if (entryAssembly != null
			&& !ReferenceEquals(entryAssembly, GetType().Assembly)
			&& TryGetApplicationId(entryAssembly, out applicationId))
		{
			return applicationId;
		}

		return null;
	}

	private static bool TryGetApplicationId(Assembly assembly, out string applicationId)
	{
		foreach (var metadata in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
		{
			if (!string.Equals(metadata.Key, MauiApplicationIdMetadataKey, StringComparison.Ordinal))
				continue;

			if (string.IsNullOrWhiteSpace(metadata.Value))
				continue;

			applicationId = metadata.Value;
			return true;
		}

		applicationId = string.Empty;
		return false;
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
		var windowTitle = virtualWindow.Title ?? "Platform.Maui.Linux.Gtk4";
		_desktopEntryName = windowTitle;
		gtkWindow.SetTitle(windowTitle);

		var windowContext = applicationContext.MakeWindowScope(gtkWindow);
		windowContext.AddSpecific(gtkWindow);

		var windowHandler = new WindowHandler();
		windowHandler.SetMauiContext(windowContext);
		windowHandler.SetVirtualView(virtualWindow);

		gtkWindow.SetApplication(_gtkApp);
		GtkDesktopIntegration.ApplyAppIcon(gtkWindow, AppContext.BaseDirectory);
		gtkWindow.Show();
		InvokeLifecycleEvents<GtkWindowCreated>(del => del(gtkWindow));

		virtualWindow.Created();
		virtualWindow.Activated();
	}

	private void EnsureDesktopEntry()
	{
		if (!CreateDesktopEntry || string.IsNullOrWhiteSpace(ApplicationId))
			return;

		GtkDesktopIntegration.EnsureDesktopEntry(ApplicationId, DesktopEntryName, AppContext.BaseDirectory);
	}

	private void OnShutdown(Gio.Application sender, EventArgs args)
	{
		InvokeLifecycleEvents<GtkApplicationShutdown>(del => del(sender));
		_gtkApp.OnActivate -= OnActivate;
		_gtkApp.OnShutdown -= OnShutdown;
	}

	private void InvokeLifecycleEvents<TDelegate>(Action<TDelegate> action)
		where TDelegate : Delegate
	{
		if (Services == null)
			return;

		var lifecycleService = Services.GetService(typeof(ILifecycleEventService)) as ILifecycleEventService;
		lifecycleService?.InvokeEvents(typeof(TDelegate).Name, action);
	}
}
