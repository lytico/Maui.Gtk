using System;

namespace Microsoft.Maui.Graphics.Platform.Gtk;

public static class GraphicsExtensions {
    public static Rect ToRect (this Gdk.Rectangle it)
        => new Rect (it.X, it.Y, it.Width, it.Height);

    public static RectF ToRectF (this Gdk.Rectangle it)
        => new RectF (it.X, it.Y, it.Width, it.Height);

    public static Gdk.Rectangle ToNative (this Rect it)
        => new Gdk.Rectangle {
            X = (int)it.X,
            Y = (int)it.Y,
            Width = (int)it.Width,
            Height = (int)it.Height
        };

    public static Gdk.Rectangle ToNative (this RectF it)
        => new Gdk.Rectangle {
            X = (int)it.X,
            Y = (int)it.Y,
            Width = (int)it.Width,
            Height = (int)it.Height
        };

    public static Point ToPoint (this Gdk.Point it)
        => new Point (it.X, it.Y);

    public static PointF ToPointF (this Gdk.Point it)
        => new PointF (it.X, it.Y);


    public static Gdk.Point ToNative (this Point it)
        => new Gdk.Point ((int)it.X, (int)it.Y);

    public static Gdk.Point ToNative (this PointF it)
        => new Gdk.Point ((int)it.X, (int)it.Y);

    public static Size ToSize (this Gdk.Size it)
        => new Size (it.Width, it.Height);

    public static SizeF ToSizeF (this Gdk.Size it)
        => new SizeF (it.Width, it.Height);

    public static Gdk.Size ToNative (this Size it)
        => new Gdk.Size ((int)it.Width, (int)it.Height);

    public static Gdk.Size ToNative (this SizeF it)
        => new Gdk.Size ((int)it.Width, (int)it.Height);

    public static double ScaledFromPango (this int it)
        => Math.Ceiling (it / (double)Pango.Constants.SCALE);

    public static float ScaledFromPangoF (this int it)
        => (float)Math.Ceiling (it / (double)Pango.Constants.SCALE);

    public static int ScaledToPango (this double it)
        => (int)Math.Ceiling (it * Pango.Constants.SCALE);

    public static int ScaledToPango (this float it)
        => (int)Math.Ceiling (it * Pango.Constants.SCALE);

    public static int ScaledToPango (this int it)
        => (int)Math.Ceiling ((double)it * Pango.Constants.SCALE);
}