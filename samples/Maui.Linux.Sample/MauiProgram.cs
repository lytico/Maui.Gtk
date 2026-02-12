using Maui.Linux.Hosting;
using Maui.Linux.Essentials.Hosting;
using Microsoft.Maui.Hosting;

namespace Maui.Linux.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp
			.CreateBuilder()
			.UseMauiAppLinux<App>()
			.AddLinuxEssentials();

		return builder.Build();
	}
}
