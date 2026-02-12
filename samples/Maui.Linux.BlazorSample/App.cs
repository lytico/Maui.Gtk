using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Controls;
using MauiBlazorWebView = Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView;

namespace Maui.Linux.BlazorSample;

public class App : Application
{
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage());
	}
}

public class MainPage : ContentPage
{
	public MainPage()
	{
		Title = "Maui.Linux Blazor Hybrid";

		var blazorWebView = new MauiBlazorWebView
		{
			HostPage = "wwwroot/index.html",
			HeightRequest = 400,
		};
		blazorWebView.RootComponents.Add(
			new RootComponent
			{
				Selector = "#app",
				ComponentType = typeof(Pages.Index),
			});

		Content = new VerticalStackLayout
		{
			Spacing = 8,
			Padding = new Thickness(16),
			Children =
			{
				new Label
				{
					Text = "Blazor Hybrid on Linux (WebKitGTK)",
					FontSize = 20,
					HorizontalTextAlignment = TextAlignment.Center,
				},
				blazorWebView,
			}
		};
	}
}
