using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// Handler for IShapeView (used internally by BoxView and Shape controls).
/// Renders shapes via Gtk.DrawingArea with Cairo.
/// </summary>
public class ShapeViewHandler : GtkViewHandler<IShapeView, Gtk.DrawingArea>
{
	public static new IPropertyMapper<IShapeView, ShapeViewHandler> Mapper =
		new PropertyMapper<IShapeView, ShapeViewHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IShapeView.Shape)] = MapShape,
			[nameof(IShapeView.Fill)] = MapFill,
			[nameof(IShapeView.Stroke)] = MapStroke,
			[nameof(IShapeView.StrokeThickness)] = MapStrokeThickness,
			[nameof(IShapeView.Aspect)] = MapAspect,
			[nameof(IShapeView.StrokeDashOffset)] = MapStrokeDash,
			[nameof(IShapeView.StrokeDashPattern)] = MapStrokeDash,
			[nameof(IShapeView.StrokeLineCap)] = MapStrokeLineCap,
			[nameof(IShapeView.StrokeLineJoin)] = MapStrokeLineJoin,
			[nameof(IShapeView.StrokeMiterLimit)] = MapStrokeMiterLimit,
		};

	public ShapeViewHandler() : base(Mapper) { }

	protected override Gtk.DrawingArea CreatePlatformView()
	{
		var area = Gtk.DrawingArea.New();
		area.SetDrawFunc(OnDraw);
		return area;
	}

	private void OnDraw(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
	{
		if (VirtualView == null)
			return;

		// Fill
		if (VirtualView.Fill is SolidPaint fillPaint && fillPaint.Color != null)
		{
			var c = fillPaint.Color;
			cr.SetSourceRgba(c.Red, c.Green, c.Blue, c.Alpha);
			cr.Rectangle(0, 0, width, height);
			cr.Fill();
		}

		// Stroke
		if (VirtualView.Stroke is SolidPaint strokePaint && strokePaint.Color != null)
		{
			var c = strokePaint.Color;
			cr.SetSourceRgba(c.Red, c.Green, c.Blue, c.Alpha);
			cr.LineWidth = VirtualView.StrokeThickness;

			// Apply dash pattern
			if (VirtualView.StrokeDashPattern is { Length: > 0 } dashPattern)
			{
				var dashes = new double[dashPattern.Length];
				for (int i = 0; i < dashPattern.Length; i++)
					dashes[i] = dashPattern[i] * VirtualView.StrokeThickness;
				cr.SetDash(dashes, VirtualView.StrokeDashOffset * VirtualView.StrokeThickness);
			}

			// Apply line cap
			cr.LineCap = VirtualView.StrokeLineCap switch
			{
				LineCap.Round => Cairo.LineCap.Round,
				LineCap.Square => Cairo.LineCap.Square,
				_ => Cairo.LineCap.Butt
			};

			// Apply line join
			cr.LineJoin = VirtualView.StrokeLineJoin switch
			{
				LineJoin.Round => Cairo.LineJoin.Round,
				LineJoin.Bevel => Cairo.LineJoin.Bevel,
				_ => Cairo.LineJoin.Miter
			};

			cr.MiterLimit = VirtualView.StrokeMiterLimit;

			cr.Rectangle(0, 0, width, height);
			cr.Stroke();
		}
	}

	public static void MapShape(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapFill(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapStroke(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapStrokeThickness(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapAspect(ShapeViewHandler handler, IShapeView view) { }
	public static void MapStrokeDash(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapStrokeLineCap(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapStrokeLineJoin(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
	public static void MapStrokeMiterLimit(ShapeViewHandler handler, IShapeView view) => handler.PlatformView?.QueueDraw();
}
