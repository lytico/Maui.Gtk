using System.Numerics;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;
using Microsoft.Maui.Handlers;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// Handler for GraphicsView using Gtk.DrawingArea with Cairo-backed ICanvas.
/// Renders MAUI.Graphics drawing commands via Cairo.
/// </summary>
public class GraphicsViewHandler : GtkViewHandler<IGraphicsView, Gtk.DrawingArea>
{
	public static new IPropertyMapper<IGraphicsView, GraphicsViewHandler> Mapper =
		new PropertyMapper<IGraphicsView, GraphicsViewHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IGraphicsView.Drawable)] = MapDrawable,
		};

	public static CommandMapper<IGraphicsView, GraphicsViewHandler> GfxCommandMapper = new(ViewCommandMapper)
	{
		[nameof(IGraphicsView.Invalidate)] = MapInvalidate,
	};

	public GraphicsViewHandler() : base(Mapper, GfxCommandMapper) { }

	protected override Gtk.DrawingArea CreatePlatformView()
	{
		var area = Gtk.DrawingArea.New();
		area.SetDrawFunc(OnDraw);
		return area;
	}

	private void OnDraw(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
	{
		if (VirtualView?.Drawable == null)
			return;

		var canvas = new CairoCanvas(cr);
		var dirtyRect = new RectF(0, 0, width, height);
		VirtualView.Drawable.Draw(canvas, dirtyRect);
	}

	public static void MapInvalidate(GraphicsViewHandler handler, IGraphicsView view, object? arg)
	{
		handler.PlatformView?.QueueDraw();
	}

	public static void MapDrawable(GraphicsViewHandler handler, IGraphicsView view)
	{
		handler.PlatformView?.QueueDraw();
	}
}

/// <summary>
/// Minimal ICanvas implementation backed by Cairo.
/// Supports the core drawing operations needed for MAUI.Graphics.
/// </summary>
internal class CairoCanvas : ICanvas
{
	private readonly Cairo.Context _cr;
	private float _strokeSize = 1;
	private Color _strokeColor = Colors.Black;
	private Color _fillColor = Colors.White;
	private Color _fontColor = Colors.Black;
	private float _fontSize = 14;
	private float _alpha = 1;

	public CairoCanvas(Cairo.Context cr)
	{
		_cr = cr;
	}

	public float StrokeSize { get => _strokeSize; set => _strokeSize = value; }
	public float MiterLimit { get; set; } = 10;
	public LineCap StrokeLineCap { get; set; } = LineCap.Butt;
	public LineJoin StrokeLineJoin { get; set; } = LineJoin.Miter;
	public Color StrokeColor { get => _strokeColor; set => _strokeColor = value ?? Colors.Black; }
	public Color FillColor { get => _fillColor; set => _fillColor = value ?? Colors.Transparent; }
	public Color FontColor { get => _fontColor; set => _fontColor = value ?? Colors.Black; }
	public float FontSize { get => _fontSize; set => _fontSize = value; }
	public IFont Font { get; set; } = Microsoft.Maui.Graphics.Font.Default;
	public float Alpha { get => _alpha; set => _alpha = Math.Clamp(value, 0, 1); }
	public bool Antialias { get; set; } = true;
	public float DisplayScale { get; set; } = 1;
	public BlendMode BlendMode { get; set; } = BlendMode.Normal;
	public float[] StrokeDashPattern { get; set; } = [];
	public float StrokeDashOffset { get; set; }

	public void DrawLine(float x1, float y1, float x2, float y2)
	{
		ApplyStroke();
		_cr.MoveTo(x1, y1);
		_cr.LineTo(x2, y2);
		_cr.Stroke();
	}

	public void DrawRectangle(float x, float y, float width, float height)
	{
		ApplyStroke();
		_cr.Rectangle(x, y, width, height);
		_cr.Stroke();
	}

	public void FillRectangle(float x, float y, float width, float height)
	{
		ApplyFill();
		_cr.Rectangle(x, y, width, height);
		_cr.Fill();
	}

	public void DrawRoundedRectangle(float x, float y, float width, float height, float cornerRadius)
	{
		ApplyStroke();
		RoundedRectPath(x, y, width, height, cornerRadius);
		_cr.Stroke();
	}

	public void FillRoundedRectangle(float x, float y, float width, float height, float cornerRadius)
	{
		ApplyFill();
		RoundedRectPath(x, y, width, height, cornerRadius);
		_cr.Fill();
	}

	public void DrawEllipse(float x, float y, float width, float height)
	{
		ApplyStroke();
		EllipsePath(x, y, width, height);
		_cr.Stroke();
	}

	public void FillEllipse(float x, float y, float width, float height)
	{
		ApplyFill();
		EllipsePath(x, y, width, height);
		_cr.Fill();
	}

	public void DrawCircle(float centerX, float centerY, float radius)
		=> DrawEllipse(centerX - radius, centerY - radius, radius * 2, radius * 2);

	public void FillCircle(float centerX, float centerY, float radius)
		=> FillEllipse(centerX - radius, centerY - radius, radius * 2, radius * 2);

	public void DrawArc(float x, float y, float width, float height, float startAngle, float endAngle, bool clockwise, bool closed)
	{
		ApplyStroke();
		double cx = x + width / 2, cy = y + height / 2;
		double rx = width / 2, ry = height / 2;
		_cr.Save();
		_cr.Translate(cx, cy);
		_cr.Scale(rx, ry);
		if (clockwise)
			_cr.Arc(0, 0, 1, startAngle * Math.PI / 180, endAngle * Math.PI / 180);
		else
			_cr.ArcNegative(0, 0, 1, startAngle * Math.PI / 180, endAngle * Math.PI / 180);
		if (closed) _cr.ClosePath();
		_cr.Restore();
		_cr.Stroke();
	}

	public void FillArc(float x, float y, float width, float height, float startAngle, float endAngle, bool clockwise)
	{
		ApplyFill();
		double cx = x + width / 2, cy = y + height / 2;
		double rx = width / 2, ry = height / 2;
		_cr.Save();
		_cr.Translate(cx, cy);
		_cr.Scale(rx, ry);
		if (clockwise)
			_cr.Arc(0, 0, 1, startAngle * Math.PI / 180, endAngle * Math.PI / 180);
		else
			_cr.ArcNegative(0, 0, 1, startAngle * Math.PI / 180, endAngle * Math.PI / 180);
		_cr.ClosePath();
		_cr.Restore();
		_cr.Fill();
	}

	public void DrawString(string value, float x, float y, HorizontalAlignment horizontalAlignment)
	{
		ApplyFontColor();
		var fontName = Font?.Name ?? "Sans";
		_cr.SelectFontFace(fontName, Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
		_cr.SetFontSize(_fontSize);
		_cr.MoveTo(x, y + _fontSize);
		_cr.ShowText(value);
	}

	public void DrawString(string value, float x, float y, float width, float height,
		HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment,
		TextFlow textFlow = TextFlow.ClipBounds, float lineSpacingAdjustment = 0)
	{
		DrawString(value, x, y + height / 2, horizontalAlignment);
	}

	public void DrawText(IAttributedText value, float x, float y, float width, float height)
	{
		DrawString(value.Text, x, y, width, height, HorizontalAlignment.Left, VerticalAlignment.Top);
	}

	public SizeF GetStringSize(string value, IFont font, float fontSize)
	{
		return new SizeF(value.Length * fontSize * 0.6f, fontSize * 1.2f);
	}

	public SizeF GetStringSize(string value, IFont font, float fontSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
	{
		return GetStringSize(value, font, fontSize);
	}

	public void DrawPath(PathF path)
	{
		ApplyStroke();
		DrawPathInternal(path);
		_cr.Stroke();
	}

	public void FillPath(PathF path, WindingMode windingMode = WindingMode.NonZero)
	{
		ApplyFill();
		DrawPathInternal(path);
		_cr.FillRule = (windingMode == WindingMode.NonZero ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd);
		_cr.Fill();
	}

	public void ClipPath(PathF path, WindingMode windingMode = WindingMode.NonZero)
	{
		DrawPathInternal(path);
		_cr.FillRule = (windingMode == WindingMode.NonZero ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd);
		_cr.Clip();
	}

	public void ClipRectangle(float x, float y, float width, float height)
	{
		_cr.Rectangle(x, y, width, height);
		_cr.Clip();
	}

	public void SubtractFromClip(float x, float y, float width, float height) { }

	public void DrawImage(IImage image, float x, float y, float width, float height) { }

	public void Rotate(float degrees, float x, float y)
	{
		_cr.Translate(x, y);
		_cr.Rotate(degrees * Math.PI / 180);
		_cr.Translate(-x, -y);
	}

	public void Rotate(float degrees) => Rotate(degrees, 0, 0);
	public void Scale(float sx, float sy) => _cr.Scale(sx, sy);
	public void Translate(float tx, float ty) => _cr.Translate(tx, ty);
	public void ConcatenateTransform(Matrix3x2 transform) { }

	public void SaveState() => _cr.Save();
	public bool RestoreState()
	{
		_cr.Restore();
		return true;
	}
	public void ResetState()
	{
		_strokeSize = 1;
		_strokeColor = Colors.Black;
		_fillColor = Colors.White;
		_fontColor = Colors.Black;
		_fontSize = 14;
		_alpha = 1;
	}

	public bool RestrictToClipBounds { get; set; }

	public void SetShadow(SizeF offset, float blur, Color color) { }
	public void SetFillPaint(Paint paint, RectF rectangle)
	{
		if (paint is SolidPaint solidPaint && solidPaint.Color != null)
			FillColor = solidPaint.Color;
	}

	private void ApplyStroke()
	{
		_cr.SetSourceRgba(_strokeColor.Red, _strokeColor.Green, _strokeColor.Blue, _strokeColor.Alpha * _alpha);
		_cr.LineWidth = _strokeSize;
	}

	private void ApplyFill()
	{
		_cr.SetSourceRgba(_fillColor.Red, _fillColor.Green, _fillColor.Blue, _fillColor.Alpha * _alpha);
	}

	private void ApplyFontColor()
	{
		_cr.SetSourceRgba(_fontColor.Red, _fontColor.Green, _fontColor.Blue, _fontColor.Alpha * _alpha);
	}

	private void EllipsePath(float x, float y, float width, float height)
	{
		_cr.Save();
		_cr.Translate(x + width / 2, y + height / 2);
		_cr.Scale(width / 2, height / 2);
		_cr.Arc(0, 0, 1, 0, 2 * Math.PI);
		_cr.Restore();
	}

	private void RoundedRectPath(float x, float y, float w, float h, float r)
	{
		r = Math.Min(r, Math.Min(w / 2, h / 2));
		_cr.NewPath();
		_cr.Arc(x + w - r, y + r, r, -Math.PI / 2, 0);
		_cr.Arc(x + w - r, y + h - r, r, 0, Math.PI / 2);
		_cr.Arc(x + r, y + h - r, r, Math.PI / 2, Math.PI);
		_cr.Arc(x + r, y + r, r, Math.PI, 3 * Math.PI / 2);
		_cr.ClosePath();
	}

	private void DrawPathInternal(PathF path)
	{
		_cr.NewPath();
		var points = path.Points?.ToArray();
		if (points == null || points.Length == 0)
			return;

		_cr.MoveTo(points[0].X, points[0].Y);
		for (int i = 1; i < points.Length; i++)
		{
			_cr.LineTo(points[i].X, points[i].Y);
		}
		if (path.Closed)
			_cr.ClosePath();
	}
}
