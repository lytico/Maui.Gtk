using Platform.Maui.Linux.Gtk4.Hosting;
using Platform.Maui.Linux.Gtk4.Essentials.Hosting;
using Platform.Maui.Linux.Gtk4.BlazorWebView;
#if MAUIDEVFLOW
using MauiDevFlow.Agent.Gtk;
using MauiDevFlow.Blazor.Gtk;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Platform.Maui.Linux.Gtk4.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp
			.CreateBuilder()
			.UseMauiAppLinuxGtk4<App>()
			.AddLinuxGtk4Essentials();

		builder.Services.AddBlazorWebView();
		builder.Services.AddLinuxGtk4BlazorWebView();

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
