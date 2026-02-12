using Microsoft.Maui.Hosting;
using Maui.Linux.Hosting;
using Maui.Linux.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Linux.BlazorSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiAppLinux<App>();
		builder.Services.AddBlazorWebView();
		builder.Services.AddLinuxBlazorWebView();

		// Register our Linux BlazorWebView handler
		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler<Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView, BlazorWebViewHandler>();
		});

		return builder.Build();
	}
}
