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

class MainShell : ContentPage
{
	private View? _pageContent;
	private readonly VerticalStackLayout _root;

	public MainShell()
	{
		Title = "Maui.Linux GTK4 Demo";

		var pageFactories = new (string name, Func<ContentPage> factory)[]
		{
			("🏠 Home", () => new HomePage()),
			("🎛️ Controls", () => new ControlsPage()),
			("📅 Pickers & Search", () => new PickersPage()),
			("📐 Layouts", () => new LayoutsPage()),
			("📋 Collection View", () => new CollectionViewPage()),
			("🎨 Graphics (Cairo)", () => new GraphicsPage()),
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

	void ShowPage(ContentPage page)
	{
		if (_pageContent != null)
			_root.Children.Remove(_pageContent);

		_pageContent = page.Content;
		if (_pageContent != null)
			_root.Children.Add(_pageContent);
	}
}
