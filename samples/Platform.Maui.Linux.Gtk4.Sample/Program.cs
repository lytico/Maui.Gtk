using Platform.Maui.Linux.Gtk4.Platform;
using Platform.Maui.Linux.Gtk4.BlazorWebView;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
#if MAUIDEVFLOW
using MauiDevFlow.Agent.Gtk;
#endif

namespace Platform.Maui.Linux.Gtk4.Sample;

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
