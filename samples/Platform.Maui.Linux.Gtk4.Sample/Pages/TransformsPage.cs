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

					// --- Clip ---
					new Label { Text = "Clip (Geometry Clipping)", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 12, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 20,
						Children =
						{
							new BoxView
							{
								Color = Colors.CornflowerBlue,
								WidthRequest = 80, HeightRequest = 80,
								Clip = new Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry
								{
									CornerRadius = new CornerRadius(16),
									Rect = new Rect(0, 0, 80, 80),
								},
							},
							new BoxView
							{
								Color = Colors.Coral,
								WidthRequest = 80, HeightRequest = 80,
								Clip = new Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry
								{
									CornerRadius = new CornerRadius(40),
									Rect = new Rect(0, 0, 80, 80),
								},
							},
							new BoxView
							{
								Color = Colors.MediumSeaGreen,
								WidthRequest = 80, HeightRequest = 80,
								Clip = new Microsoft.Maui.Controls.Shapes.EllipseGeometry
								{
									Center = new Point(40, 40),
									RadiusX = 40,
									RadiusY = 40,
								},
							},
							new BoxView
							{
								Color = Colors.Orchid,
								WidthRequest = 80, HeightRequest = 80,
								Clip = new Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry
								{
									CornerRadius = new CornerRadius(20, 0, 20, 0),
									Rect = new Rect(0, 0, 80, 80),
								},
							},
						}
					},
					new Label { Text = "Round 16px / Circle / Ellipse / Diagonal corners", FontSize = 11, TextColor = Colors.Gray },

					// --- Gradient Brushes ---
					new Label { Text = "Gradient Brushes", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 12, 0, 0) },
					new HorizontalStackLayout
					{
						Spacing = 16,
						Children =
						{
							new Frame
							{
								WidthRequest = 120, HeightRequest = 80, Padding = 0,
								CornerRadius = 8, BorderColor = Colors.Transparent, HasShadow = false,
								Background = new LinearGradientBrush
								{
									StartPoint = new Point(0, 0), EndPoint = new Point(1, 1),
									GradientStops = { new GradientStop(Colors.DodgerBlue, 0), new GradientStop(Colors.MediumPurple, 1) },
								},
								Content = new Label { Text = "Linear ↘", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center },
							},
							new Frame
							{
								WidthRequest = 120, HeightRequest = 80, Padding = 0,
								CornerRadius = 8, BorderColor = Colors.Transparent, HasShadow = false,
								Background = new LinearGradientBrush
								{
									StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5),
									GradientStops = { new GradientStop(Colors.OrangeRed, 0), new GradientStop(Colors.Gold, 0.5f), new GradientStop(Colors.LimeGreen, 1) },
								},
								Content = new Label { Text = "3-Stop →", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center },
							},
							new Frame
							{
								WidthRequest = 120, HeightRequest = 80, Padding = 0,
								CornerRadius = 8, BorderColor = Colors.Transparent, HasShadow = false,
								Background = new RadialGradientBrush
								{
									Center = new Point(0.5, 0.5), Radius = 0.6,
									GradientStops = { new GradientStop(Colors.White, 0), new GradientStop(Colors.Coral, 0.6f), new GradientStop(Colors.DarkRed, 1) },
								},
								Content = new Label { Text = "Radial", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center },
							},
							new Frame
							{
								WidthRequest = 120, HeightRequest = 80, Padding = 0,
								CornerRadius = 40, BorderColor = Colors.Transparent, HasShadow = false,
								Background = new LinearGradientBrush
								{
									StartPoint = new Point(0, 0), EndPoint = new Point(0, 1),
									GradientStops = { new GradientStop(Colors.DeepSkyBlue, 0), new GradientStop(Colors.DeepPink, 1) },
								},
								Content = new Label { Text = "Rounded", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center },
							},
						}
					},
					new Label { Text = "Linear diagonal / 3-stop horizontal / Radial / Rounded pill", FontSize = 11, TextColor = Colors.Gray },

					// --- ZIndex ---
					new Label { Text = "ZIndex (draw order)", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 12, 0, 0) },
					BuildZIndexDemo(),
					new Label { Text = "Blue (Z=1) → Coral (Z=2) → Green (Z=3, on top)", FontSize = 11, TextColor = Colors.Gray },
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

	static View BuildZIndexDemo()
	{
		var layout = new AbsoluteLayout { HeightRequest = 110 };

		var b1 = new BoxView { Color = Colors.CornflowerBlue, WidthRequest = 80, HeightRequest = 80, ZIndex = 1 };
		var b2 = new BoxView { Color = Colors.Coral, WidthRequest = 80, HeightRequest = 80, ZIndex = 2 };
		var b3 = new BoxView { Color = Colors.MediumSeaGreen, WidthRequest = 80, HeightRequest = 80, ZIndex = 3 };

		layout.Children.Add(b1);
		AbsoluteLayout.SetLayoutBounds(b1, new Rect(0, 0, 80, 80));
		layout.Children.Add(b2);
		AbsoluteLayout.SetLayoutBounds(b2, new Rect(30, 10, 80, 80));
		layout.Children.Add(b3);
		AbsoluteLayout.SetLayoutBounds(b3, new Rect(60, 20, 80, 80));

		return layout;
	}
}
