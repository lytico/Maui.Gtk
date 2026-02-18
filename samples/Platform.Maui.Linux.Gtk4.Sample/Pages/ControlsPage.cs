using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Platform.Maui.Linux.Gtk4.Sample.Pages;

public class ControlsPage : ContentPage
{
	public ControlsPage()
	{
		Title = "Controls";

		int clickCount = 0;
		var clickLabel = new Label { Text = "Clicks: 0", FontSize = 14 };
		var progressBar = new ProgressBar { Progress = 0 };

		var button = new Button { Text = "Click me!" };
		button.Clicked += (s, e) =>
		{
			clickCount++;
			clickLabel.Text = $"Clicks: {clickCount}";
			progressBar.Progress = Math.Min(1.0, clickCount / 20.0);
		};

		var entryEcho = new Label { Text = "Echo: ", FontSize = 14, TextColor = Colors.Gray };
		var entry = new Entry { Placeholder = "Type here..." };
		entry.TextChanged += (s, e) => entryEcho.Text = $"Echo: {e.NewTextValue}";

		var sliderLabel = new Label { Text = "Slider: 50", FontSize = 14 };
		var slider = new Slider(0, 100, 50);
		slider.ValueChanged += (s, e) => sliderLabel.Text = $"Slider: {e.NewValue:F0}";

		var switchLabel = new Label { Text = "Off", FontSize = 14 };
		var toggle = new Switch();
		toggle.Toggled += (s, e) => switchLabel.Text = e.Value ? "On" : "Off";

		var checkLabel = new Label { Text = "Unchecked", FontSize = 14 };
		var checkBox = new CheckBox();
		checkBox.CheckedChanged += (s, e) => checkLabel.Text = e.Value ? "Checked ✓" : "Unchecked";

		var stepperLabel = new Label { Text = "Stepper: 0", FontSize = 14 };
		var stepper = new Stepper { Minimum = 0, Maximum = 50, Increment = 5 };
		stepper.ValueChanged += (s, e) => stepperLabel.Text = $"Stepper: {e.NewValue}";

		var radioLabel = new Label { Text = "Selected: Option A", FontSize = 14, TextColor = Colors.DodgerBlue };
		var radio1 = new RadioButton { Content = "Option A", GroupName = "demo", IsChecked = true };
		var radio2 = new RadioButton { Content = "Option B", GroupName = "demo" };
		var radio3 = new RadioButton { Content = "Option C", GroupName = "demo" };
		radio1.CheckedChanged += (s, e) => { if (e.Value) radioLabel.Text = "Selected: Option A"; };
		radio2.CheckedChanged += (s, e) => { if (e.Value) radioLabel.Text = "Selected: Option B"; };
		radio3.CheckedChanged += (s, e) => { if (e.Value) radioLabel.Text = "Selected: Option C"; };

		Content = new ScrollView
		{
			Content = new VerticalStackLayout
			{
				Spacing = 10,
				Padding = new Thickness(24),
				Children =
				{
					new Label { Text = "Interactive Controls", FontSize = 24, FontAttributes = FontAttributes.Bold },
					new BoxView { HeightRequest = 2, Color = Colors.DodgerBlue },

					// Button + Progress
					SectionHeader("Button & ProgressBar"),
					button,
					clickLabel,
					new Label { Text = "Progress (click 20x to fill):", FontSize = 12, TextColor = Colors.Gray },
					progressBar,

					Separator(),

					// Entry
					SectionHeader("Entry"),
					entry,
					entryEcho,

					Separator(),

					// Editor
					SectionHeader("Editor"),
					new Editor { Placeholder = "Multi-line text editor...", HeightRequest = 80 },

					Separator(),

					// Slider
					SectionHeader("Slider"),
					slider,
					sliderLabel,

					Separator(),

					// Switch
					SectionHeader("Switch"),
					new HorizontalStackLayout
					{
						Spacing = 12,
						Children = { toggle, switchLabel }
					},

					Separator(),

					// CheckBox
					SectionHeader("CheckBox"),
					new HorizontalStackLayout
					{
						Spacing = 12,
						Children = { checkBox, checkLabel }
					},

					Separator(),

					// Stepper
					SectionHeader("Stepper (increment by 5)"),
					stepper,
					stepperLabel,

					Separator(),

					// RadioButtons
					SectionHeader("RadioButton"),
					radio1, radio2, radio3,
					radioLabel,

					Separator(),

					// VisualStateManager
					SectionHeader("VisualStateManager"),
					new Label { Text = "Hover/press buttons below to see VSM state changes:", FontSize = 12, TextColor = Colors.Gray },
					BuildVsmDemo(),
				}
			}
		};
	}

	static Label SectionHeader(string text) => new()
	{
		Text = text,
		FontSize = 16,
		FontAttributes = FontAttributes.Bold,
		TextColor = Colors.DarkSlateGray,
	};

	static BoxView Separator() => new() { HeightRequest = 1, Color = Colors.LightGray };

	static View BuildVsmDemo()
	{
		var stateLabel = new Label { Text = "State: Normal", FontSize = 14 };

		var vsmButton = new Button
		{
			Text = "Hover or Press Me",
			BackgroundColor = Colors.CornflowerBlue,
			TextColor = Colors.White,
			Padding = new Thickness(20, 12),
		};

		// Define visual states
		var normalState = new VisualState { Name = "Normal" };
		normalState.Setters.Add(new Setter { Property = Button.BackgroundColorProperty, Value = Colors.CornflowerBlue });
		normalState.Setters.Add(new Setter { Property = Button.ScaleProperty, Value = 1.0 });

		var pointerOverState = new VisualState { Name = "PointerOver" };
		pointerOverState.Setters.Add(new Setter { Property = Button.BackgroundColorProperty, Value = Colors.DodgerBlue });
		pointerOverState.Setters.Add(new Setter { Property = Button.ScaleProperty, Value = 1.05 });

		var pressedState = new VisualState { Name = "Pressed" };
		pressedState.Setters.Add(new Setter { Property = Button.BackgroundColorProperty, Value = Colors.DarkBlue });
		pressedState.Setters.Add(new Setter { Property = Button.ScaleProperty, Value = 0.95 });

		var disabledState = new VisualState { Name = "Disabled" };
		disabledState.Setters.Add(new Setter { Property = Button.BackgroundColorProperty, Value = Colors.LightGray });

		var focusedState = new VisualState { Name = "Focused" };
		focusedState.Setters.Add(new Setter { Property = Button.BackgroundColorProperty, Value = Colors.MediumSlateBlue });

		var group = new VisualStateGroup { Name = "CommonStates" };
		group.States.Add(normalState);
		group.States.Add(pointerOverState);
		group.States.Add(pressedState);
		group.States.Add(disabledState);
		group.States.Add(focusedState);

		VisualStateManager.SetVisualStateGroups(vsmButton, [group]);

		// Track state changes for the label
		vsmButton.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(Button.BackgroundColor))
			{
				var color = vsmButton.BackgroundColor;
				string state = color == Colors.CornflowerBlue ? "Normal"
					: color == Colors.DodgerBlue ? "PointerOver"
					: color == Colors.DarkBlue ? "Pressed"
					: color == Colors.LightGray ? "Disabled"
					: color == Colors.MediumSlateBlue ? "Focused"
					: "Unknown";
				stateLabel.Text = $"State: {state}";
			}
		};

		// Toggle enabled/disabled
		var toggleBtn = new Button { Text = "Toggle Enabled", BackgroundColor = Colors.Gray, TextColor = Colors.White };
		toggleBtn.Clicked += (s, e) =>
		{
			vsmButton.IsEnabled = !vsmButton.IsEnabled;
			toggleBtn.Text = vsmButton.IsEnabled ? "Toggle Enabled" : "Toggle Disabled";
		};

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children = { stateLabel, vsmButton, toggleBtn }
		};
	}
}
