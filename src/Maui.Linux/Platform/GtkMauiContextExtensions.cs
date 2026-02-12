using Microsoft.Maui;

namespace Maui.Linux.Platform;

public static class GtkMauiContextExtensions
{
	public static GtkMauiContext MakeApplicationScope(this GtkMauiContext context, IPlatformApplication platformApp)
	{
		var appContext = new GtkMauiContext(context.Services);
		appContext.AddSpecific<IPlatformApplication>(platformApp);
		return appContext;
	}

	public static GtkMauiContext MakeWindowScope(this GtkMauiContext context, Gtk.Window platformWindow)
	{
		var windowContext = new GtkMauiContext(context.Services);
		windowContext.AddWeakSpecific(platformWindow);
		return windowContext;
	}
}
