using Maui.Linux.Platform;
using Maui.Linux.BlazorWebView;
using Microsoft.Maui.Hosting;

namespace Maui.Linux.BlazorSample;

public class Program : GtkMauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	[STAThread]
	public static void Main(string[] args)
	{
		// Must be called before any WebKit types are used
		GtkBlazorWebView.InitializeWebKit();

		var app = new Program();
		app.Run(args);
	}
}
