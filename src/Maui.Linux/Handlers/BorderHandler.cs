using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class BorderHandler : GtkViewHandler<IBorderView, Platform.GtkLayoutPanel>
{
	public static new IPropertyMapper<IBorderView, BorderHandler> Mapper =
		new PropertyMapper<IBorderView, BorderHandler>(ViewMapper)
		{
			[nameof(IBorderView.Content)] = MapContent,
			[nameof(IBorderView.Background)] = MapBorderBackground,
			[nameof(IBorderView.Stroke)] = MapStroke,
			[nameof(IBorderView.StrokeThickness)] = MapStroke,
		};

	public BorderHandler() : base(Mapper)
	{
	}

	protected override Platform.GtkLayoutPanel CreatePlatformView()
	{
		return new Platform.GtkLayoutPanel();
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		if (VirtualView is ICrossPlatformLayout crossPlatform)
			return crossPlatform.CrossPlatformMeasure(widthConstraint, heightConstraint);

		return base.GetDesiredSize(widthConstraint, heightConstraint);
	}

	public override void PlatformArrange(Rect rect)
	{
		base.PlatformArrange(rect);
		if (VirtualView is ICrossPlatformLayout crossPlatform)
			crossPlatform.CrossPlatformArrange(new Rect(0, 0, rect.Width, rect.Height));
	}

	public static void MapContent(BorderHandler handler, IBorderView border)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		if (border.PresentedContent != null)
		{
			var platformContent = (Gtk.Widget)border.PresentedContent.ToPlatform(handler.MauiContext);
			// Remove existing children
			while (handler.PlatformView.GetFirstChild() is Gtk.Widget child)
				handler.PlatformView.Remove(child);
			handler.PlatformView.Put(platformContent, 0, 0);
			handler.PlatformView.LayoutDirty = true;
		}
	}

	static void MapBorderBackground(BorderHandler handler, IBorderView border)
	{
		ApplyBorderCss(handler);
	}

	static void MapStroke(BorderHandler handler, IBorderView border)
	{
		ApplyBorderCss(handler);
	}

	static void ApplyBorderCss(BorderHandler handler)
	{
		var border = handler.VirtualView;
		var css = string.Empty;

		if (border.Background is SolidPaint solidPaint && solidPaint.Color != null)
			css += $"background-color: {ToGtkColor(solidPaint.Color)}; ";

		if (border.Stroke is SolidPaint strokePaint && strokePaint.Color != null)
		{
			var thickness = Math.Max(1, (int)border.StrokeThickness);
			css += $"border: {thickness}px solid {ToGtkColor(strokePaint.Color)}; ";
		}
		else if (border.StrokeThickness <= 0)
		{
			css += "border: none; ";
		}

		if (!string.IsNullOrEmpty(css))
			handler.ApplyCss(handler.PlatformView, css);
	}
}
