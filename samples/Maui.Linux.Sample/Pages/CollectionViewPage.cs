using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Linux.Sample.Pages;

public class CollectionViewPage : ContentPage
{
	public CollectionViewPage()
	{
		Title = "CollectionView";

		var items = Enumerable.Range(1, 50)
			.Select(i => $"Item {i} — {GetDescription(i)}")
			.ToList();

		var selectedLabel = new Label
		{
			Text = "Tap an item to select it",
			FontSize = 14,
			TextColor = Colors.Gray,
		};

		var countLabel = new Label
		{
			Text = $"Showing {items.Count} items",
			FontSize = 12,
			TextColor = Colors.DodgerBlue,
		};

		var searchBar = new SearchBar { Placeholder = "Filter items..." };
		var filteredItems = new List<string>(items);

		// Use a simple ListView approach since CollectionView handler
		// requires direct handler access for SetItems
		var stackList = new VerticalStackLayout { Spacing = 0 };
		PopulateList(stackList, items, selectedLabel);

		searchBar.TextChanged += (s, e) =>
		{
			filteredItems.Clear();
			if (string.IsNullOrWhiteSpace(e.NewTextValue))
				filteredItems.AddRange(items);
			else
				filteredItems.AddRange(items.Where(i => i.Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase)));

			countLabel.Text = $"Showing {filteredItems.Count} items";
			PopulateList(stackList, filteredItems, selectedLabel);
		};

		Content = new VerticalStackLayout
		{
			Spacing = 8,
			Padding = new Thickness(24),
			Children =
			{
				new Label { Text = "CollectionView", FontSize = 24, FontAttributes = FontAttributes.Bold },
				new BoxView { HeightRequest = 2, Color = Colors.DodgerBlue },
				searchBar,
				countLabel,
				selectedLabel,
				new BoxView { HeightRequest = 1, Color = Colors.LightGray },
				new ScrollView
				{
					HeightRequest = 400,
					Content = stackList,
				}
			}
		};
	}

	void PopulateList(VerticalStackLayout stack, List<string> items, Label selectedLabel)
	{
		stack.Children.Clear();
		foreach (var item in items)
		{
			var label = new Label
			{
				Text = item,
				FontSize = 14,
				Padding = new Thickness(12, 10),
			};

			var border = new Border
			{
				Stroke = Colors.Transparent,
				StrokeThickness = 0,
				Content = label,
			};

			// Simulate tap selection using a Button overlay approach
			var btn = new Button
			{
				Text = item,
				BackgroundColor = Colors.Transparent,
				TextColor = Colors.Black,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.Fill,
			};
			var capturedItem = item;
			btn.Clicked += (s, e) =>
			{
				selectedLabel.Text = $"Selected: {capturedItem}";
				selectedLabel.TextColor = Colors.DodgerBlue;
			};

			stack.Children.Add(btn);
			stack.Children.Add(new BoxView { HeightRequest = 1, Color = Color.FromArgb("#f0f0f0") });
		}
	}

	static string GetDescription(int i) => (i % 5) switch
	{
		0 => "🔴 Important task",
		1 => "🟢 Completed item",
		2 => "🔵 In progress",
		3 => "🟡 Pending review",
		_ => "⚪ Backlog item",
	};
}
