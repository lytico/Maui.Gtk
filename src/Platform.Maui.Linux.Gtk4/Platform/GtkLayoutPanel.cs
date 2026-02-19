using Microsoft.Maui;
using Microsoft.Maui.Graphics;

namespace Platform.Maui.Linux.Gtk4.Platform;

/// <summary>
/// A custom GTK4 container that delegates layout to MAUI's cross-platform layout engine.
/// Children are positioned at exact coordinates computed by CrossPlatformMeasure/CrossPlatformArrange.
/// </summary>
public class GtkLayoutPanel : Gtk.Fixed
{
	ICrossPlatformLayout? _crossPlatformLayout;

	/// <summary>
	/// Set to true when children are added/removed so the root tick callback
	/// knows to re-measure and re-arrange even if the window size hasn't changed.
	/// </summary>
	public bool LayoutDirty { get; set; }

	/// <summary>
	/// When true, this panel is driven by an external layout (e.g., CollectionView template)
	/// and should not run its own idle/tick layout passes.
	/// </summary>
	public bool IsExternallyManaged { get; set; }

	/// <summary>
	/// When true, this panel is inside a ScrolledWindow and child height
	/// minimums should not be set (to prevent pushing the window to grow).
	/// </summary>
	public bool IsInsideScrollableContainer { get; set; }

	public GtkLayoutPanel() : base()
	{
		SetHexpand(true);
		SetVexpand(true);
		// Override GTK size negotiation: report 0 minimum so the window
		// can shrink freely. MAUI's layout engine controls actual sizing.
		SetSizeRequest(0, 0);
	}

	public ICrossPlatformLayout? CrossPlatformLayout
	{
		get => _crossPlatformLayout;
		set
		{
			_crossPlatformLayout = value;
			QueueResize();
		}
	}

	public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
	{
		if (_crossPlatformLayout == null)
			return Size.Zero;

		return _crossPlatformLayout.CrossPlatformMeasure(widthConstraint, heightConstraint);
	}

	public Size CrossPlatformArrange(Rect bounds)
	{
		if (_crossPlatformLayout == null)
			return bounds.Size;

		return _crossPlatformLayout.CrossPlatformArrange(bounds);
	}

	public void ArrangeChild(Gtk.Widget child, Rect bounds)
	{
		child.SetSizeRequest((int)bounds.Width, (int)bounds.Height);
		Move(child, bounds.X, bounds.Y);
	}
}
