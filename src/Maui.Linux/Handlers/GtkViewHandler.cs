using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

namespace Maui.Linux.Handlers;

/// <summary>
/// Base handler for all GTK-backed MAUI views.
/// Bridges MAUI's layout coordinates to GTK widget allocation.
/// </summary>
public abstract class GtkViewHandler<TVirtualView, TPlatformView> : ViewHandler<TVirtualView, TPlatformView>
	where TVirtualView : class, IView
	where TPlatformView : Gtk.Widget
{
	public static IPropertyMapper<IView, GtkViewHandler<TVirtualView, TPlatformView>> ViewMapper =
		new PropertyMapper<IView, GtkViewHandler<TVirtualView, TPlatformView>>(ViewHandler.ViewMapper)
		{
			[nameof(IView.Background)] = MapBackground,
			[nameof(IView.Opacity)] = MapOpacity,
			[nameof(IView.Visibility)] = MapVisibility,
			[nameof(IView.IsEnabled)] = MapIsEnabled,
			[nameof(IView.Semantics)] = MapSemantics,
			[nameof(IView.AutomationId)] = MapAutomationId,
		};

	protected GtkViewHandler(IPropertyMapper mapper, CommandMapper? commandMapper = null)
		: base(mapper, commandMapper)
	{
	}

	public override void PlatformArrange(Rect rect)
	{
		var platformView = PlatformView;
		if (platformView == null)
			return;

		platformView.SetSizeRequest((int)rect.Width, (int)rect.Height);

		// Position the widget inside its parent GtkLayoutPanel (Gtk.Fixed)
		if (platformView.GetParent() is Platform.GtkLayoutPanel layoutPanel)
		{
			layoutPanel.Move(platformView, rect.X, rect.Y);
		}
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		var platformView = PlatformView;
		if (platformView == null)
			return Size.Zero;

		// Clear any previous SetSizeRequest so Measure returns the natural size,
		// not the previously-arranged minimum.
		platformView.SetSizeRequest(-1, -1);

		// Measure horizontal natural size
		platformView.MeasureNative(Gtk.Orientation.Horizontal, -1, out var minWidth, out var natWidth, out _, out _);

		var width = Math.Min(natWidth, widthConstraint);

		// If MAUI set an explicit request, use it
		if (VirtualView is Microsoft.Maui.Controls.VisualElement ve)
		{
			if (ve.WidthRequest >= 0)
				width = Math.Min(ve.WidthRequest, widthConstraint);
		}

		// Measure vertical size with the actual width constraint so wrapping
		// widgets (e.g. labels with SetWrap) report the correct wrapped height.
		int forWidth = (int)width;
		platformView.MeasureNative(Gtk.Orientation.Vertical, forWidth, out var minHeight, out var natHeight, out _, out _);

		var height = Math.Min(natHeight, heightConstraint);

		if (VirtualView is Microsoft.Maui.Controls.VisualElement ve2)
		{
			if (ve2.HeightRequest >= 0)
				height = Math.Min(ve2.HeightRequest, heightConstraint);
		}

		return new Size(Math.Max(1, width), Math.Max(1, height));
	}

	static void MapBackground(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		if (view.Background is SolidPaint solidPaint && solidPaint.Color != null)
		{
			handler.ApplyCss(handler.PlatformView, $"background-color: {ToGtkColor(solidPaint.Color)};");
		}
	}

	static void MapOpacity(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		handler.PlatformView?.SetOpacity(view.Opacity);
	}

	static void MapVisibility(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		handler.PlatformView?.SetVisible(view.Visibility == Visibility.Visible);
	}

	static void MapIsEnabled(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		handler.PlatformView?.SetSensitive(view.IsEnabled);
	}

	/// <summary>
	/// Maps MAUI SemanticProperties to GTK4 accessibility.
	/// GTK4 uses the Gtk.Accessible interface with AT-SPI on Linux.
	/// </summary>
	static void MapSemantics(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		var widget = handler.PlatformView;
		if (widget == null || view.Semantics == null)
			return;

		var semantics = view.Semantics;

		// Use the GTK widget name as a11y label (read by screen readers via AT-SPI)
		if (!string.IsNullOrEmpty(semantics.Description))
		{
			// Gtk.Widget tooltip serves as accessible description
			widget.SetTooltipText(semantics.Description);
		}

		if (!string.IsNullOrEmpty(semantics.Hint))
		{
			widget.SetTooltipText(semantics.Hint);
		}
	}

	/// <summary>
	/// Maps MAUI AutomationId to GTK widget name (used by test frameworks like Dogtail).
	/// </summary>
	static void MapAutomationId(GtkViewHandler<TVirtualView, TPlatformView> handler, IView view)
	{
		if (handler.PlatformView != null && view is Microsoft.Maui.Controls.VisualElement ve
			&& !string.IsNullOrEmpty(ve.AutomationId))
		{
			handler.PlatformView.SetName(ve.AutomationId);
		}
	}

	protected void ApplyCss(Gtk.Widget? widget, string css)
	{
		if (widget == null)
			return;

		var provider = Gtk.CssProvider.New();
		provider.LoadFromString($"* {{ {css} }}");
		Gtk.StyleContext.AddProviderForDisplay(
			Gdk.Display.GetDefault()!,
			provider,
			Gtk.Constants.STYLE_PROVIDER_PRIORITY_APPLICATION);
	}

	protected static string ToGtkColor(Color color)
	{
		return $"rgba({(int)(color.Red * 255)},{(int)(color.Green * 255)},{(int)(color.Blue * 255)},{color.Alpha})";
	}
}

// Convenience overload for measuring GTK widgets
internal static class GtkWidgetMeasureExtensions
{
	public static void Measure(this Gtk.Widget widget, out int minWidth, out int natWidth, out int minHeight, out int natHeight)
	{
		widget.MeasureNative(Gtk.Orientation.Horizontal, -1, out minWidth, out natWidth, out _, out _);
		widget.MeasureNative(Gtk.Orientation.Vertical, -1, out minHeight, out natHeight, out _, out _);
	}

	public static void MeasureNative(this Gtk.Widget widget, Gtk.Orientation orientation, int forSize,
		out int minimum, out int natural, out int minimumBaseline, out int naturalBaseline)
	{
		widget.Measure(orientation, forSize, out minimum, out natural, out minimumBaseline, out naturalBaseline);
	}
}
