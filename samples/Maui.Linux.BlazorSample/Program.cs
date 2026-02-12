using Maui.Linux.Platform;
using Maui.Linux.BlazorWebView;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using MauiDevFlow.Agent.Gtk;

namespace Maui.Linux.BlazorSample;

public class Program : GtkMauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnStarted()
	{
#if DEBUG
		(Application as Application)?.StartDevFlowAgent();
#endif
	}

	[STAThread]
	public static void Main(string[] args)
	{
		// Must be called before any WebKit types are used
		GtkBlazorWebView.InitializeWebKit();

		var app = new Program();
		app.Run(args);
	}
}
