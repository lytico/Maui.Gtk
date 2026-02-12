using Maui.Linux.Platform;
using Microsoft.Maui.Hosting;

namespace Maui.Linux.Sample;

public class Program : GtkMauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
