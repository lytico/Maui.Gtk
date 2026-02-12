using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Maui.Linux.Sample.Pages;

namespace Maui.Linux.Sample;

class App : Application
{
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainShell());
	}
}

class MainShell : FlyoutPage
{
	private readonly (string name, Func<Page> factory)[] _pages =
	[
		("🏠 Home", () => new HomePage()),
		("🎛️ Controls", () => new ControlsPage()),
		("📅 Pickers & Search", () => new PickersPage()),
		("📐 Layouts", () => new LayoutsPage()),
		("💬 Alerts & Dialogs", () => new AlertsPage()),
		("📋 Collection View", () => new CollectionViewPage()),
		("🎨 Graphics (Cairo)", () => new GraphicsPage()),
		("📱 Device & App Info", () => new DeviceInfoPage()),
		("🔋 Battery & Network", () => new BatteryNetworkPage()),
		("📋 Clipboard & Storage", () => new ClipboardPrefsPage()),
		("🚀 Launch & Share", () => new LaunchSharePage()),
		("🌐 Blazor Hybrid", () => new BlazorPage()),
		("🧭 Navigation", () => new NavigationPage(new NavigationDemoPage())),
		("📑 TabbedPage", () => new TabbedPageDemo()),
		("📂 FlyoutPage", () => new FlyoutPageDemo()),
	];

	public MainShell()
	{
		Title = "Maui.Linux GTK4 Demo";

		var menuStack = new VerticalStackLayout { Spacing = 0 };

		menuStack.Children.Add(new Label
		{
			Text = "🐧 Maui.Linux",
			FontSize = 20,
			FontAttributes = FontAttributes.Bold,
			TextColor = Colors.DodgerBlue,
			Padding = new Thickness(16, 20, 16, 4),
		});
		menuStack.Children.Add(new Label
		{
			Text = "GTK4 Demo App",
			FontSize = 12,
			TextColor = Colors.Gray,
			Padding = new Thickness(16, 0, 16, 12),
		});
		menuStack.Children.Add(new BoxView { HeightRequest = 1, Color = Colors.LightGray });

		foreach (var (name, factory) in _pages)
		{
			var btn = new Button
			{
				Text = name,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.Fill,
			};
			var capturedFactory = factory;
			btn.Clicked += (s, e) =>
			{
				Detail = capturedFactory();
			};
			menuStack.Children.Add(btn);
		}

		Flyout = new ContentPage
		{
			Title = "Menu",
			Content = new ScrollView { Content = menuStack },
		};

		Detail = new HomePage();
		IsPresented = true;
	}
}
