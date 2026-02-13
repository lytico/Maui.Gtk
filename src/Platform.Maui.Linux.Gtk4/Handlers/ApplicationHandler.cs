using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class ApplicationHandler : ElementHandler<IApplication, Gtk.Application>
{
	public static IPropertyMapper<IApplication, ApplicationHandler> Mapper =
		new PropertyMapper<IApplication, ApplicationHandler>(ElementHandler.ElementMapper)
		{
		};

	public ApplicationHandler() : base(Mapper)
	{
	}

	protected override Gtk.Application CreatePlatformElement()
	{
		// The Gtk.Application is created by GtkMauiApplication,
		// we use a placeholder since MAUI requires a platform element
		return Gtk.Application.New(null, Gio.ApplicationFlags.DefaultFlags);
	}
}
