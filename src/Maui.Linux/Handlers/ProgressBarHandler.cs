using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class ProgressBarHandler : GtkViewHandler<IProgress, Gtk.ProgressBar>
{
	public static new IPropertyMapper<IProgress, ProgressBarHandler> Mapper =
		new PropertyMapper<IProgress, ProgressBarHandler>(ViewMapper)
		{
			[nameof(IProgress.Progress)] = MapProgress,
		};

	public ProgressBarHandler() : base(Mapper)
	{
	}

	protected override Gtk.ProgressBar CreatePlatformView()
	{
		return Gtk.ProgressBar.New();
	}

	public static void MapProgress(ProgressBarHandler handler, IProgress progress)
	{
		handler.PlatformView?.SetFraction(progress.Progress);
	}
}
