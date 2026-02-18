using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Platform.Maui.Linux.Gtk4.Sample.Pages;

namespace Platform.Maui.Linux.Gtk4.Sample;

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
		("🧩 XAML Runtime", () => new XamlRuntimePage()),
		("🖼️ Resource Image + Font + Icon", () => new ResourceAssetsPage()),
		("🎛️ Controls", () => new ControlsPage()),
		("📅 Pickers & Search", () => new PickersPage()),
		("📐 Layouts", () => new LayoutsPage()),
		("💬 Alerts & Dialogs", () => new AlertsPage()),
		("📋 Collection View", () => new CollectionViewPage()),
		("✏️ Text Input Styling", () => new TextInputStylingPage()),
		("🍔 Menu & Toolbar", () => new MenuBarPage()),
		("🔄 Refresh & Swipe", () => new RefreshSwipePage()),
		("🎠 Carousel & Indicators", () => new CarouselIndicatorPage()),
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
		Title = "Platform.Maui.Linux.Gtk4 GTK4 Demo";

		var menuStack = new VerticalStackLayout { Spacing = 0 };

		menuStack.Children.Add(new Label
		{
			Text = "🐧 Platform.Maui.Linux.Gtk4",
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
				var page = capturedFactory();
				// Wrap non-NavigationPage detail pages in a NavigationPage
				// so they get a nav bar with the hamburger toggle
				if (page is ContentPage cp)
					Detail = new NavigationPage(cp);
				else
					Detail = page;
			};
			menuStack.Children.Add(btn);
		}

		Flyout = new ContentPage
		{
			Title = "Menu",
			Content = new ScrollView { Content = menuStack },
		};

		Detail = new NavigationPage(new HomePage());
		IsPresented = true;
	}
}
