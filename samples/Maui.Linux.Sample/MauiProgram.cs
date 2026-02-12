using Maui.Linux.Hosting;
using Maui.Linux.Essentials.Hosting;
using MauiDevFlow.Agent.Gtk;
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

#if DEBUG
		builder.AddMauiDevFlowAgent();
#endif

		return builder.Build();
	}
}
