using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Platform.Maui.Linux.Gtk4.Sample.Pages;

public class ResourceAssetsPage : ContentPage
{
	readonly Label _imageStatus;
	readonly Label _fontStatus;

	public ResourceAssetsPage()
	{
		Title = "Resource Image + Font";
		_imageStatus = new Label { FontSize = 12, TextColor = Colors.Gray };
		_fontStatus = new Label { FontSize = 12, TextColor = Colors.Gray };

		var refreshButton = new Button { Text = "Refresh Resource Status" };
		refreshButton.Clicked += (s, e) => UpdateStatus();

		Content = new ScrollView
		{
			Content = new VerticalStackLayout
			{
				Padding = new Thickness(24),
				Spacing = 12,
				Children =
				{
					new Label
					{
						Text = "🖼️ MAUI Resource Demo",
						FontSize = 28,
						FontAttributes = FontAttributes.Bold,
					},
					new Label
					{
						Text = "ImageSource and font alias below come from Resources/* items.",
						FontSize = 14,
						TextColor = Colors.Gray,
					},
					new Border
					{
						Stroke = Colors.LightGray,
						Padding = new Thickness(12),
						Content = new Image
						{
							Source = "dotnet_bot.png",
							HeightRequest = 170,
							Aspect = Aspect.AspectFit,
						},
					},
					_imageStatus,
					new Label { Text = "Default system font preview", FontSize = 20 },
					new Label
					{
						Text = "OpenSansRegular font alias preview",
						FontSize = 20,
						FontFamily = "OpenSansRegular",
					},
					_fontStatus,
					refreshButton,
				}
			}
		};

		UpdateStatus();
	}

	void UpdateStatus()
	{
		var baseDirectory = AppContext.BaseDirectory;
		var imagePath = Path.Combine(baseDirectory, "dotnet_bot.png");
		var fontPath = Path.Combine(baseDirectory, "OpenSans-Regular.ttf");

		_imageStatus.Text = $"dotnet_bot.png exists: {File.Exists(imagePath)}";
		_fontStatus.Text = $"OpenSans-Regular.ttf exists: {File.Exists(fontPath)}";
	}
}
