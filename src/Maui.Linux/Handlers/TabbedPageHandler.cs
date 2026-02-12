using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class TabbedPageHandler : GtkViewHandler<ITabbedView, Gtk.Notebook>
{
	public static new IPropertyMapper<ITabbedView, TabbedPageHandler> Mapper =
		new PropertyMapper<ITabbedView, TabbedPageHandler>(ViewMapper)
		{
		};

	public TabbedPageHandler() : base(Mapper)
	{
	}

	protected override Gtk.Notebook CreatePlatformView()
	{
		var notebook = Gtk.Notebook.New();
		notebook.SetVexpand(true);
		notebook.SetHexpand(true);
		return notebook;
	}
}
