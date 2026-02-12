using Maui.Linux.Platform;
using Maui.Linux.BlazorWebView;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
#if MAUIDEVFLOW
using MauiDevFlow.Agent.Gtk;
#endif

namespace Maui.Linux.Sample;

public class Program : GtkMauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnStarted()
	{
#if MAUIDEVFLOW
		(Application as Application)?.StartDevFlowAgent();
#endif
	}

	public static void Main(string[] args)
	{
		GtkBlazorWebView.InitializeWebKit();

		var app = new Program();
		app.Run(args);
	}
}
