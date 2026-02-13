using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
using Platform.Maui.Linux.Gtk4.Handlers;
using Platform.Maui.Linux.Gtk4.Platform;

namespace Platform.Maui.Linux.Gtk4.Hosting;

public static partial class AppHostBuilderExtensions
{
	public static MauiAppBuilder UseMauiAppLinuxGtk4<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(
		this MauiAppBuilder builder)
		where TApp : class, IApplication
	{
		builder.UseMauiApp<TApp>();
		builder.SetupDefaults();
		return builder;
	}

	static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlersCollection)
	{
		handlersCollection.AddHandler<Application, ApplicationHandler>();
		handlersCollection.AddHandler<Microsoft.Maui.Controls.Window, WindowHandler>();
		handlersCollection.AddHandler<ContentPage, PageHandler>();
		handlersCollection.AddHandler<Layout, LayoutHandler>();
		handlersCollection.AddHandler<ContentView, ContentViewHandler>();
		handlersCollection.AddHandler<Label, LabelHandler>();
		handlersCollection.AddHandler<Button, ButtonHandler>();
		handlersCollection.AddHandler<Entry, EntryHandler>();
		handlersCollection.AddHandler<Editor, EditorHandler>();
		handlersCollection.AddHandler<CheckBox, CheckBoxHandler>();
		handlersCollection.AddHandler<Switch, SwitchHandler>();
		handlersCollection.AddHandler<Slider, SliderHandler>();
		handlersCollection.AddHandler<ProgressBar, ProgressBarHandler>();
		handlersCollection.AddHandler<ActivityIndicator, ActivityIndicatorHandler>();
		handlersCollection.AddHandler<Image, ImageHandler>();
		handlersCollection.AddHandler<Picker, PickerHandler>();
		handlersCollection.AddHandler<DatePicker, DatePickerHandler>();
		handlersCollection.AddHandler<TimePicker, TimePickerHandler>();
		handlersCollection.AddHandler<Stepper, StepperHandler>();
		handlersCollection.AddHandler<RadioButton, RadioButtonHandler>();
		handlersCollection.AddHandler<SearchBar, SearchBarHandler>();
		handlersCollection.AddHandler<ScrollView, ScrollViewHandler>();
		handlersCollection.AddHandler<Border, BorderHandler>();
#pragma warning disable CS0618
		handlersCollection.AddHandler<Frame, FrameHandler>();
#pragma warning restore CS0618
		handlersCollection.AddHandler<ImageButton, ImageButtonHandler>();
		handlersCollection.AddHandler<WebView, WebViewHandler>();
		handlersCollection.AddHandler<NavigationPage, NavigationPageHandler>();
		handlersCollection.AddHandler<TabbedPage, TabbedPageHandler>();
		handlersCollection.AddHandler<FlyoutPage, FlyoutPageHandler>();

		// Phase 6: Advanced handlers
		handlersCollection.AddHandler<CollectionView, CollectionViewHandler>();
		handlersCollection.AddHandler<GraphicsView, GraphicsViewHandler>();

		// BoxView / Shapes (MAUI routes BoxView through ShapeView internally)
#pragma warning disable CS0618
		handlersCollection.AddHandler<BoxView, BoxViewHandler>();
#pragma warning restore CS0618
		handlersCollection.AddHandler(typeof(Microsoft.Maui.IShapeView), typeof(ShapeViewHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Rectangle), typeof(ShapeHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Ellipse), typeof(ShapeHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Line), typeof(ShapeHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Path), typeof(ShapeHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Polygon), typeof(ShapeHandler));
		handlersCollection.AddHandler(typeof(Microsoft.Maui.Controls.Shapes.Polyline), typeof(ShapeHandler));

		return handlersCollection;
	}

	static MauiAppBuilder SetupDefaults(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<IDispatcherProvider>(svc => new GtkDispatcherProvider());

		// Register GTK alert/dialog handler for DisplayAlert/ActionSheet/Prompt
		GtkAlertManager.Register(builder.Services);

		builder.Services.AddScoped(svc =>
		{
			var provider = svc.GetRequiredService<IDispatcherProvider>();
			if (DispatcherProvider.SetCurrent(provider))
				svc.GetService<ILogger<Dispatcher>>()?.LogWarning("Replaced an existing DispatcherProvider.");

			return Dispatcher.GetForCurrentThread()!;
		});

		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddMauiControlsHandlers();
		});

		return builder;
	}
}
