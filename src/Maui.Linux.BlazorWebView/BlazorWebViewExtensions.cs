using Microsoft.Extensions.DependencyInjection;

namespace Maui.Linux.BlazorWebView;

public static class BlazorWebViewExtensions
{
	/// <summary>
	/// Adds Linux-specific BlazorWebView services.
	/// </summary>
	public static IServiceCollection AddLinuxBlazorWebView(this IServiceCollection services)
	{
		// Register any BlazorWebView-specific services here
		return services;
	}
}
