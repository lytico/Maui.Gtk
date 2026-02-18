using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace Platform.Maui.Linux.Gtk4.Sample.Pages;

/// <summary>
/// Demonstrates RefreshView (pull-to-refresh / refresh button) and
/// SwipeView (passthrough container on desktop Linux).
/// </summary>
public class RefreshSwipePage : ContentPage
{
	readonly ObservableCollection<string> _items = new();
	readonly Label _statusLabel;
	int _refreshCount;

	public RefreshSwipePage()
	{
		Title = "Refresh & Swipe";

		_statusLabel = new Label
		{
			Text = "Tap the refresh button above the list to reload items",
			HorizontalOptions = LayoutOptions.Center,
			TextColor = Colors.DimGray,
			Margin = new Thickness(0, 8),
		};

		LoadItems();

		var listView = new ListView
		{
			ItemsSource = _items,
			VerticalOptions = LayoutOptions.FillAndExpand,
		};

		RefreshView refreshView = null!;
		refreshView = new RefreshView
		{
			Content = listView,
			RefreshColor = Colors.DodgerBlue,
			Command = new Command(async () =>
			{
				await Task.Delay(1200);
				_refreshCount++;
				LoadItems();
				refreshView.IsRefreshing = false;
				_statusLabel.Text = $"Refreshed {_refreshCount} time(s)";
			}),
		};

		// Wrap some items in SwipeView to verify passthrough rendering
		var swipeDemo = new SwipeView
		{
			Content = new Frame
			{
				BackgroundColor = Colors.LightYellow,
				CornerRadius = 8,
				Padding = new Thickness(16),
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = "SwipeView Container",
							FontAttributes = FontAttributes.Bold,
							FontSize = 16,
						},
						new Label
						{
							Text = "On mobile, you could swipe to reveal actions. " +
							       "On desktop Linux, the content renders normally as a passthrough.",
							TextColor = Colors.DimGray,
						},
					}
				}
			},
		};

		Content = new StackLayout
		{
			Padding = new Thickness(16),
			Children =
			{
				new Label
				{
					Text = "RefreshView & SwipeView Demo",
					FontSize = 22,
					FontAttributes = FontAttributes.Bold,
					Margin = new Thickness(0, 0, 0, 8),
				},
				_statusLabel,
				refreshView,
				new BoxView { HeightRequest = 16 },
				swipeDemo,
			}
		};
	}

	void LoadItems()
	{
		_items.Clear();
		var rng = new Random();
		for (int i = 1; i <= 15; i++)
			_items.Add($"Item {i} (value: {rng.Next(100, 999)})");
	}
}
