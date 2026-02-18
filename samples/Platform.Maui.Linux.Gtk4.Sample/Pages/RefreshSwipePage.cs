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

		Content = new ScrollView
		{
			Content = new StackLayout
			{
				Padding = new Thickness(16),
				Spacing = 12,
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
					new BoxView { HeightRequest = 16 },
					BuildPointerGestureDemo(),
					new BoxView { HeightRequest = 16 },
					BuildSwipeGestureDemo(),
					new BoxView { HeightRequest = 16 },
					BuildPinchGestureDemo(),
				}
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

	static View BuildPointerGestureDemo()
	{
		var posLabel = new Label { Text = "Move pointer over the box →", FontSize = 13, TextColor = Colors.Gray };
		var stateLabel = new Label { Text = "Outside", FontSize = 14, FontAttributes = FontAttributes.Bold };

		var trackingBox = new BoxView
		{
			Color = Colors.CornflowerBlue,
			WidthRequest = 200,
			HeightRequest = 100,
			HorizontalOptions = LayoutOptions.Start,
		};

		var pointerGesture = new PointerGestureRecognizer();
		pointerGesture.PointerEnteredCommand = new Command(() =>
		{
			stateLabel.Text = "🟢 Inside";
			stateLabel.TextColor = Colors.Green;
			trackingBox.Color = Colors.MediumSeaGreen;
		});
		pointerGesture.PointerMovedCommand = new Command(() =>
		{
			posLabel.Text = "Pointer moving...";
		});
		pointerGesture.PointerExitedCommand = new Command(() =>
		{
			stateLabel.Text = "🔴 Outside";
			stateLabel.TextColor = Colors.Red;
			trackingBox.Color = Colors.CornflowerBlue;
			posLabel.Text = "Move pointer over the box →";
		});
		pointerGesture.PointerPressedCommand = new Command(() =>
		{
			stateLabel.Text = "⬇️ Pressed";
			trackingBox.Color = Colors.DarkSlateBlue;
		});
		pointerGesture.PointerReleasedCommand = new Command(() =>
		{
			stateLabel.Text = "⬆️ Released";
			trackingBox.Color = Colors.MediumSeaGreen;
		});
		trackingBox.GestureRecognizers.Add(pointerGesture);

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children =
			{
				new Label { Text = "Pointer Gesture", FontSize = 16, FontAttributes = FontAttributes.Bold },
				new Label { Text = "Tracks enter/move/exit/press/release on the box below.", FontSize = 12, TextColor = Colors.Gray },
				stateLabel,
				posLabel,
				trackingBox,
			}
		};
	}

	static View BuildSwipeGestureDemo()
	{
		var directionLabel = new Label { Text = "Swipe on the box below (drag quickly)", FontSize = 13, TextColor = Colors.Gray };
		var resultLabel = new Label { Text = "No swipe detected", FontSize = 14 };

		var swipeBox = new BoxView
		{
			Color = Colors.Coral,
			WidthRequest = 250,
			HeightRequest = 100,
			HorizontalOptions = LayoutOptions.Start,
		};

		foreach (var dir in new[] { SwipeDirection.Left, SwipeDirection.Right, SwipeDirection.Up, SwipeDirection.Down })
		{
			var swipe = new SwipeGestureRecognizer { Direction = dir };
			swipe.Swiped += (s, e) =>
			{
				string arrow = e.Direction switch
				{
					SwipeDirection.Left => "⬅️",
					SwipeDirection.Right => "➡️",
					SwipeDirection.Up => "⬆️",
					SwipeDirection.Down => "⬇️",
					_ => "?",
				};
				resultLabel.Text = $"{arrow} Swiped {e.Direction}!";
				resultLabel.TextColor = Colors.DodgerBlue;
			};
			swipeBox.GestureRecognizers.Add(swipe);
		}

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children =
			{
				new Label { Text = "Swipe Gesture", FontSize = 16, FontAttributes = FontAttributes.Bold },
				directionLabel,
				resultLabel,
				swipeBox,
			}
		};
	}

	static View BuildPinchGestureDemo()
	{
		var scaleLabel = new Label { Text = "Scale: 1.00x", FontSize = 14 };
		var statusLabel = new Label { Text = "Use trackpad pinch or Ctrl+scroll to zoom", FontSize = 12, TextColor = Colors.Gray };

		var target = new BoxView
		{
			Color = Colors.MediumSlateBlue,
			WidthRequest = 100,
			HeightRequest = 100,
			HorizontalOptions = LayoutOptions.Center,
		};

		double currentScale = 1.0;
		var pinch = new PinchGestureRecognizer();
		pinch.PinchUpdated += (s, e) =>
		{
			switch (e.Status)
			{
				case GestureStatus.Started:
					statusLabel.Text = "Pinching...";
					break;
				case GestureStatus.Running:
					currentScale = e.Scale;
					target.Scale = Math.Clamp(currentScale, 0.3, 3.0);
					scaleLabel.Text = $"Scale: {target.Scale:F2}x";
					break;
				case GestureStatus.Completed:
					statusLabel.Text = $"Pinch complete at {target.Scale:F2}x";
					break;
				case GestureStatus.Canceled:
					statusLabel.Text = "Pinch canceled";
					target.Scale = 1.0;
					scaleLabel.Text = "Scale: 1.00x";
					break;
			}
		};
		target.GestureRecognizers.Add(pinch);

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children =
			{
				new Label { Text = "Pinch Gesture", FontSize = 16, FontAttributes = FontAttributes.Bold },
				scaleLabel,
				statusLabel,
				target,
			}
		};
	}
}
