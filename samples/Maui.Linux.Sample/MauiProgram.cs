using Maui.Linux.Hosting;
using Microsoft.Maui.Hosting;

namespace Maui.Linux.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp
			.CreateBuilder()
			.UseMauiAppLinux<App>();

		return builder.Build();
	}
}
