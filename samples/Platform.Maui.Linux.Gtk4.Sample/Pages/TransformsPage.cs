using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Platform.Maui.Linux.Gtk4.Sample.Pages;

class TransformsPage : ContentPage
{
	public TransformsPage()
	{
		Title = "Transforms & Effects";

		Content = new ScrollView
		{
			Content = new VerticalStackLayout
			{
				Spacing = 20,
				Padding = new Thickness(24),
				Children =
				{
					new Label { Text = "Transforms & Effects", FontSize = 22, FontAttributes = FontAttributes.Bold },

					// --- Rotation ---
					new Label { Text = "Rotation", FontSize = 16, FontAttributes = FontAttributes.Bold },
					new HorizontalStackLayout
					{
						Spacing = 20,
						Children =
						{
							MakeBox(Colors.CornflowerBlue, "0°", rotation: 0),
							MakeBox(Colors.Coral, "15°", rotation: 15),
							MakeBox(Colors.MediumSeaGreen, "45°", rotation: 45),
							MakeBox(Colors.Orchid, "90°", rotation: 90),
						}
					},

					// --- Scale ---
					new Label { Text = "Scale", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 8, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 30,
						Children =
						{
							MakeBox(Colors.SteelBlue, "0.5x", scale: 0.5),
							MakeBox(Colors.Tomato, "1.0x", scale: 1.0),
							MakeBox(Colors.Gold, "1.5x", scale: 1.5),
						}
					},

					// --- ScaleX / ScaleY ---
					new Label { Text = "ScaleX / ScaleY", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 8, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 30,
						Children =
						{
							MakeBox(Colors.Peru, "ScaleX=1.5", scaleX: 1.5),
							MakeBox(Colors.Teal, "ScaleY=1.5", scaleY: 1.5),
						}
					},

					// --- Translation ---
					new Label { Text = "Translation", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 16, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 20,
						HeightRequest = 80,
						Children =
						{
							MakeBox(Colors.DodgerBlue, "No shift"),
							MakeBox(Colors.OrangeRed, "X+20 Y+10", translationX: 20, translationY: 10),
							MakeBox(Colors.MediumPurple, "X-10 Y+20", translationX: -10, translationY: 20),
						}
					},

					// --- Combined ---
					new Label { Text = "Combined (Rotate + Scale + Translate)", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 8, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 40,
						HeightRequest = 100,
						Children =
						{
							MakeBox(Colors.DeepPink, "All", rotation: 30, scale: 1.3, translationX: 10, translationY: 5),
						}
					},

					// --- Shadow ---
					new Label { Text = "Shadow (box-shadow)", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 8, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 30,
						Children =
						{
							new BoxView
							{
								Color = Colors.White,
								WidthRequest = 80, HeightRequest = 60,
								Shadow = new Shadow
								{
									Brush = new SolidColorBrush(Colors.Black),
									Offset = new Point(4, 4),
									Radius = 8,
									Opacity = 0.4f,
								}
							},
							new Button
							{
								Text = "Shadow Btn",
								BackgroundColor = Colors.CornflowerBlue,
								TextColor = Colors.White,
								Shadow = new Shadow
								{
									Brush = new SolidColorBrush(Colors.DarkBlue),
									Offset = new Point(3, 3),
									Radius = 6,
									Opacity = 0.5f,
								}
							},
							new Label
							{
								Text = "Shadow Label",
								FontSize = 18,
								FontAttributes = FontAttributes.Bold,
								Padding = new Thickness(12, 8),
								BackgroundColor = Colors.LightYellow,
								Shadow = new Shadow
								{
									Brush = new SolidColorBrush(Colors.Orange),
									Offset = new Point(2, 2),
									Radius = 4,
									Opacity = 0.6f,
								}
							},
						}
					},

					// --- InputTransparent ---
					new Label { Text = "InputTransparent", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 12, 0, 0) },
					BuildInputTransparentDemo(),

					// --- AnchorX / AnchorY ---
					new Label { Text = "AnchorX/Y (transform origin)", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 12, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 30,
						Children =
						{
							MakeBox(Colors.Crimson, "TopLeft", rotation: 30, anchorX: 0, anchorY: 0),
							MakeBox(Colors.ForestGreen, "Center", rotation: 30, anchorX: 0.5, anchorY: 0.5),
							MakeBox(Colors.RoyalBlue, "BotRight", rotation: 30, anchorX: 1, anchorY: 1),
						}
					},
				}
			}
		};
	}

	static BoxView MakeBox(Color color, string tooltip,
		double rotation = 0, double scale = 1, double scaleX = 1, double scaleY = 1,
		double translationX = 0, double translationY = 0,
		double anchorX = 0.5, double anchorY = 0.5)
	{
		return new BoxView
		{
			Color = color,
			WidthRequest = 60,
			HeightRequest = 60,
			Rotation = rotation,
			Scale = scale,
			ScaleX = scaleX,
			ScaleY = scaleY,
			TranslationX = translationX,
			TranslationY = translationY,
			AnchorX = anchorX,
			AnchorY = anchorY,
		};
	}

	static View BuildInputTransparentDemo()
	{
		var resultLabel = new Label { Text = "Click the buttons below:", FontSize = 14 };

		var normalBtn = new Button
		{
			Text = "Normal (clickable)",
			BackgroundColor = Colors.MediumSeaGreen,
			TextColor = Colors.White,
		};
		normalBtn.Clicked += (s, e) => resultLabel.Text = "✅ Normal button clicked!";

		var transparentBtn = new Button
		{
			Text = "InputTransparent",
			BackgroundColor = Colors.LightGray,
			TextColor = Colors.Gray,
			InputTransparent = true,
		};
		transparentBtn.Clicked += (s, e) => resultLabel.Text = "❌ This should NOT fire!";

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children =
			{
				resultLabel,
				new HorizontalStackLayout
				{
					Spacing = 12,
					Children = { normalBtn, transparentBtn }
				},
			}
		};
	}
}
