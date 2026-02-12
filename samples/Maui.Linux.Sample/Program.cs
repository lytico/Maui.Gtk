using Maui.Linux.Platform;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using MauiDevFlow.Agent.Gtk;

namespace Maui.Linux.Sample;

public class Program : GtkMauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnStarted()
	{
#if DEBUG
		(Application as Application)?.StartDevFlowAgent();
#endif
	}

	public static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
