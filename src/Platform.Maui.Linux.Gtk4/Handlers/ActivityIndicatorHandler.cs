using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class ActivityIndicatorHandler : GtkViewHandler<IActivityIndicator, Gtk.Spinner>
{
	public static new IPropertyMapper<IActivityIndicator, ActivityIndicatorHandler> Mapper =
		new PropertyMapper<IActivityIndicator, ActivityIndicatorHandler>(ViewMapper)
		{
			[nameof(IActivityIndicator.IsRunning)] = MapIsRunning,
		};

	public ActivityIndicatorHandler() : base(Mapper)
	{
	}

	protected override Gtk.Spinner CreatePlatformView()
	{
		return Gtk.Spinner.New();
	}

	public static void MapIsRunning(ActivityIndicatorHandler handler, IActivityIndicator indicator)
	{
		handler.PlatformView?.SetSpinning(indicator.IsRunning);
	}
}
