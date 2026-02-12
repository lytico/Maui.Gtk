using Maui.Linux.Hosting;
using Maui.Linux.Essentials.Hosting;
using Maui.Linux.BlazorWebView;
#if MAUIDEVFLOW
using MauiDevFlow.Agent.Gtk;
using MauiDevFlow.Blazor.Gtk;
#endif
using Microsoft.Extensions.DependencyInjection;
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

		builder.Services.AddBlazorWebView();
		builder.Services.AddLinuxBlazorWebView();

		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler<Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView, BlazorWebViewHandler>();
		});

#if MAUIDEVFLOW
		builder.AddMauiDevFlowAgent();
		builder.AddMauiBlazorDevFlowTools();
#endif

		return builder.Build();
	}
}
