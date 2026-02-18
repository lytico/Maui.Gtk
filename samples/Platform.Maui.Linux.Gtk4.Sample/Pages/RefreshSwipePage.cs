using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Platform.Maui.Linux.Gtk4.Sample.Pages;

/// <summary>
/// Demonstrates RefreshView (pull-to-refresh / refresh button) and
/// SwipeView (passthrough container on desktop Linux).
/// </summary>
public class RefreshSwipePage : ContentPage
{
	readonly StackLayout _itemsStack;
	readonly Label _statusLabel;
	int _refreshCount;
	bool _isRefreshing;

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

		_itemsStack = new StackLayout { Spacing = 2 };
		LoadItems();

		var itemsScroll = new ScrollView
		{
			Content = _itemsStack,
			VerticalOptions = LayoutOptions.FillAndExpand,
			HeightRequest = 250,
		};

		RefreshView refreshView = null!;
		refreshView = new RefreshView
		{
			Content = itemsScroll,
			RefreshColor = Colors.DodgerBlue,
			Command = new Command(async () =>
			{
				// Guard against re-entrant refresh from rapid clicks
				if (_isRefreshing) return;
				_isRefreshing = true;
				try
				{
					await Task.Delay(1200);
					_refreshCount++;
					LoadItems();
					_statusLabel.Text = $"Refreshed {_refreshCount} time(s)";
				}
				finally
				{
					_isRefreshing = false;
					refreshView.IsRefreshing = false;
				}
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
		var rng = new Random();

		// Reuse existing labels to avoid GTK widget reparenting issues
		for (int i = 0; i < 15; i++)
		{
			var text = $"  Item {i + 1} (value: {rng.Next(100, 999)})";
			if (i < _itemsStack.Children.Count && _itemsStack.Children[i] is Label existing)
			{
				existing.Text = text;
			}
			else
			{
				_itemsStack.Children.Add(new Label
				{
					Text = text,
					TextColor = Colors.Black,
					Padding = new Thickness(8, 6),
					BackgroundColor = (i + 1) % 2 == 0 ? Color.FromRgba(0.93, 0.93, 0.93, 1) : Color.FromRgba(0.97, 0.97, 0.97, 1),
				});
			}
		}
	}
}
