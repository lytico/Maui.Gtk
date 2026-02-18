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
		("🐚 Shell Navigation", () => new ShellDemoPage()),
	];

	Label? _selectedLabel;

	public MainShell()
	{
		Title = "Platform.Maui.Linux.Gtk4 GTK4 Demo";

		var menuStack = new VerticalStackLayout { Spacing = 0 };

		// Header
		menuStack.Children.Add(new VerticalStackLayout
		{
			Padding = new Thickness(16, 20, 16, 12),
			Spacing = 2,
			Children =
			{
				new Label
				{
					Text = "🐧 Platform.Maui.Linux.Gtk4",
					FontSize = 18,
					FontAttributes = FontAttributes.Bold,
					TextColor = Colors.DodgerBlue,
				},
				new Label
				{
					Text = "GTK4 Demo App",
					FontSize = 12,
					TextColor = Colors.Gray,
				},
			}
		});

		bool first = true;
		foreach (var (name, factory) in _pages)
		{
			var label = new Label
			{
				Text = name,
				FontSize = 14,
				Padding = new Thickness(16, 10),
			};

			var tapGesture = new TapGestureRecognizer();
			var capturedFactory = factory;
			var capturedLabel = label;
			tapGesture.Tapped += (s, e) =>
			{
				// Update selection highlight
				if (_selectedLabel != null)
				{
					_selectedLabel.BackgroundColor = Colors.Transparent;
					_selectedLabel.FontAttributes = FontAttributes.None;
				}
				capturedLabel.BackgroundColor = Color.FromRgba(0.3, 0.5, 0.8, 0.25);
				capturedLabel.FontAttributes = FontAttributes.Bold;
				_selectedLabel = capturedLabel;

				var page = capturedFactory();
				if (page is ContentPage cp)
					Detail = new NavigationPage(cp);
				else
					Detail = page;
			};
			label.GestureRecognizers.Add(tapGesture);

			// Select the first item (Home) by default
			if (first)
			{
				label.BackgroundColor = Color.FromRgba(0.3, 0.5, 0.8, 0.25);
				label.FontAttributes = FontAttributes.Bold;
				_selectedLabel = label;
				first = false;
			}

			menuStack.Children.Add(label);
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
