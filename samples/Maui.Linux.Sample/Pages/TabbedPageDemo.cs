using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Linux.Sample.Pages;

public class TabbedPageDemo : TabbedPage
{
	public TabbedPageDemo()
	{
		Title = "TabbedPage Demo";

		var backBtn = new Button
		{
			Text = "← Back to Demo Menu",
			BackgroundColor = Colors.DodgerBlue,
			TextColor = Colors.White,
			FontSize = 13,
			HorizontalOptions = LayoutOptions.Start,
		};
		backBtn.Clicked += (s, e) => App.Instance?.NavigateHome();

		Children.Add(new ContentPage
		{
			Title = "🏠 Overview",
			Content = new VerticalStackLayout
			{
				Spacing = 12,
				Padding = new Thickness(24),
				Children =
				{
					backBtn,
					new Label { Text = "TabbedPage", FontSize = 24, FontAttributes = FontAttributes.Bold },
					new Label { Text = "This is a real MAUI TabbedPage rendered with a GTK4 Notebook widget. Each tab is a separate ContentPage.", FontSize = 14, TextColor = Colors.Gray },
					new BoxView { HeightRequest = 2, Color = Colors.DodgerBlue },
					new Label { Text = "✅ Native GTK4 tab rendering", FontSize = 14 },
					new Label { Text = "✅ Automatic tab label from Page.Title", FontSize = 14 },
					new Label { Text = "✅ Tab switching syncs with MAUI CurrentPage", FontSize = 14 },
					new Label { Text = "✅ Dynamic page collection support", FontSize = 14 },
				}
			}
		});

		Children.Add(new ContentPage
		{
			Title = "⚙️ Settings",
			Content = new VerticalStackLayout
			{
				Spacing = 12,
				Padding = new Thickness(24),
				Children =
				{
					new Label { Text = "Settings", FontSize = 20, FontAttributes = FontAttributes.Bold },
					new BoxView { HeightRequest = 1, Color = Colors.LightGray },
					BuildSettingRow("Dark Mode", new Switch()),
					BuildSettingRow("Notifications", new Switch { IsToggled = true }),
					BuildSettingRow("Volume", new Slider { Minimum = 0, Maximum = 100, Value = 75 }),
					BuildSettingRow("Language", new Picker { ItemsSource = new[] { "English", "Spanish", "French", "German" }, SelectedIndex = 0 }),
				}
			}
		});

		Children.Add(new ContentPage
		{
			Title = "📊 Stats",
			Content = new VerticalStackLayout
			{
				Spacing = 12,
				Padding = new Thickness(24),
				Children =
				{
					new Label { Text = "Statistics", FontSize = 20, FontAttributes = FontAttributes.Bold },
					new BoxView { HeightRequest = 1, Color = Colors.LightGray },
					BuildStatRow("Projects", "12", Colors.DodgerBlue),
					BuildStatRow("Tasks Done", "847", Colors.MediumSeaGreen),
					BuildStatRow("Open Issues", "23", Colors.Orange),
					BuildStatRow("Contributors", "6", Colors.MediumOrchid),
					new BoxView { HeightRequest = 1, Color = Colors.LightGray },
					new ProgressBar { Progress = 0.73 },
					new Label { Text = "73% sprint completion", FontSize = 12, TextColor = Colors.Gray },
				}
			}
		});
	}

	static View BuildSettingRow(string label, View control)
	{
		return new HorizontalStackLayout
		{
			Spacing = 12,
			Children =
			{
				new Label { Text = label, FontSize = 15, VerticalTextAlignment = TextAlignment.Center, WidthRequest = 120 },
				control,
			}
		};
	}

	static View BuildStatRow(string label, string value, Color color)
	{
		return new HorizontalStackLayout
		{
			Spacing = 12,
			Padding = new Thickness(0, 4),
			Children =
			{
				new Border
				{
					StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
					BackgroundColor = color,
					Padding = new Thickness(12, 8),
					StrokeThickness = 0,
					Content = new Label { Text = value, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White },
				},
				new Label { Text = label, FontSize = 15, VerticalTextAlignment = TextAlignment.Center },
			}
		};
	}
}
