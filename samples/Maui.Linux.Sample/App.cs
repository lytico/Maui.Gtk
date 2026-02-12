using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Maui.Linux.Sample.Pages;

namespace Maui.Linux.Sample;

class App : Application
{
	public static App? Instance { get; private set; }
	private Window? _window;

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Instance = this;
		_window = new Window(new MainShell());
		return _window;
	}

	public void NavigateTo(Page page)
	{
		if (_window != null)
			_window.Page = page;
	}

	public void NavigateHome()
	{
		if (_window != null)
			_window.Page = new MainShell();
	}
}

class MainShell : ContentPage
{
	private View? _pageContent;
	private readonly VerticalStackLayout _root;

	public MainShell()
	{
		Title = "Maui.Linux GTK4 Demo";

		var pageFactories = new (string name, Func<Page> factory)[]
		{
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
			("📑 TabbedPage", () => new TabbedPageDemo()),
			("📂 FlyoutPage", () => new FlyoutPageDemo()),
		};

		var picker = new Picker { Title = "Select a demo page" };
		foreach (var (name, _) in pageFactories)
			picker.Items.Add(name);
		picker.SelectedIndex = 0;

		picker.SelectedIndexChanged += (s, e) =>
		{
			if (picker.SelectedIndex >= 0 && picker.SelectedIndex < pageFactories.Length)
				ShowPage(pageFactories[picker.SelectedIndex].factory());
		};

		_root = new VerticalStackLayout
		{
			Spacing = 8,
			Padding = new Thickness(24),
			Children =
			{
				new Label
				{
					Text = "🐧 Maui.Linux GTK4 Demo",
					FontSize = 24,
					FontAttributes = FontAttributes.Bold,
					TextColor = Colors.DodgerBlue,
				},
				new Label
				{
					Text = "Navigate to a demo page:",
					FontSize = 14,
					TextColor = Colors.Gray,
				},
				picker,
				new BoxView { HeightRequest = 2, Color = Colors.DodgerBlue },
			}
		};

		// Start with home
		ShowPage(new HomePage());

		Content = _root;
	}

	void ShowPage(Page page)
	{
		if (page is ContentPage contentPage)
		{
			// Embed content inline
			if (_pageContent != null)
				_root.Children.Remove(_pageContent);

			_pageContent = contentPage.Content;
			if (_pageContent != null)
				_root.Children.Add(_pageContent);
		}
		else
		{
			// TabbedPage, FlyoutPage — must be window root
			App.Instance?.NavigateTo(page);
		}
	}
}
