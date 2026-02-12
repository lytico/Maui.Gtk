using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Maui.Linux.Platform;
using ILayout = Microsoft.Maui.ILayout;

namespace Maui.Linux.Handlers;

public class LayoutHandler : GtkViewHandler<ILayout, GtkLayoutPanel>, ILayoutHandler
{
	public static new IPropertyMapper<ILayout, LayoutHandler> Mapper =
		new PropertyMapper<ILayout, LayoutHandler>(ViewMapper)
		{
			[nameof(ILayout.Background)] = MapBackground,
			[nameof(ILayout.ClipsToBounds)] = MapClipsToBounds,
		};

	public static CommandMapper<ILayout, LayoutHandler> CommandMapper = new(ViewCommandMapper)
	{
		[nameof(ILayoutHandler.Add)] = MapAdd,
		[nameof(ILayoutHandler.Remove)] = MapRemove,
		[nameof(ILayoutHandler.Clear)] = MapClear,
		[nameof(ILayoutHandler.Insert)] = MapInsert,
		[nameof(ILayoutHandler.Update)] = MapUpdate,
		[nameof(ILayoutHandler.UpdateZIndex)] = MapUpdateZIndex,
	};

	public LayoutHandler() : base(Mapper, CommandMapper)
	{
	}

	public LayoutHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
		: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
	{
	}

	protected override GtkLayoutPanel CreatePlatformView()
	{
		return new GtkLayoutPanel();
	}

	public override void SetVirtualView(IView view)
	{
		base.SetVirtualView(view);

		// MAUI doesn't automatically call Add for pre-existing children.
		// We must add them manually when the handler is first connected.
		var layout = (ILayout)view;
		for (int i = 0; i < layout.Count; i++)
		{
			Add(layout[i]);
		}

		// Trigger initial layout after GTK has allocated sizes
		GLib.Functions.IdleAdd(0, () =>
		{
			if (VirtualView != null && PlatformView != null)
			{
				var (width, height) = GetAvailableSize(PlatformView);
				PlatformView.CrossPlatformMeasure(width, height);
				PlatformView.CrossPlatformArrange(new Rect(0, 0, width, height));
			}
			return false;
		});
	}

	protected override void ConnectHandler(GtkLayoutPanel platformView)
	{
		base.ConnectHandler(platformView);

		if (VirtualView is ICrossPlatformLayout layout)
			platformView.CrossPlatformLayout = layout;

		// Only the outermost layout installs a tick callback to drive
		// measure/arrange. Any layout that has a GtkLayoutPanel ancestor
		// (even through Viewports, ScrolledWindows, etc.) is nested and
		// will be measured/arranged by its parent's layout pass.
		GLib.Functions.IdleAdd(0, () =>
		{
			if (HasAncestorLayoutPanel(platformView))
				return false; // nested — parent drives layout

			int lastW = 0, lastH = 0;
			platformView.AddTickCallback((widget, clock) =>
			{
				if (VirtualView == null) return false;

				var (w, h) = GetAvailableSize(widget);

				if (w > 1 && h > 1 && (w != lastW || h != lastH || platformView.LayoutDirty))
				{
					lastW = w;
					lastH = h;
					platformView.LayoutDirty = false;
					platformView.CrossPlatformMeasure(w, h);
					platformView.CrossPlatformArrange(new Rect(0, 0, w, h));
				}
				return true;
			});
			return false;
		});
	}

	/// <summary>
	/// Walks up the GTK widget tree to check if any ancestor is a GtkLayoutPanel.
	/// If so, this panel is nested and should be driven by the parent layout pass.
	/// </summary>
	private static bool HasAncestorLayoutPanel(Gtk.Widget widget)
	{
		var current = widget.GetParent();
		while (current != null && current is not Gtk.Window)
		{
			if (current is GtkLayoutPanel)
				return true;
			current = current.GetParent();
		}
		return false;
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		return PlatformView.CrossPlatformMeasure(widthConstraint, heightConstraint);
	}

	public override void PlatformArrange(Rect rect)
	{
		// Size and position the GtkLayoutPanel itself within its parent
		base.PlatformArrange(rect);

		// Arrange children relative to the panel (origin at 0,0)
		PlatformView.CrossPlatformArrange(new Rect(0, 0, rect.Width, rect.Height));
	}

	/// <summary>
	/// Returns the available size for this layout panel by walking up to
	/// the nearest constraining ancestor. Gtk.Fixed and Gtk.Viewport grow
	/// to fit their children, so we skip them to avoid a feedback loop.
	/// </summary>
	private static (int width, int height) GetAvailableSize(Gtk.Widget widget)
	{
		Gtk.Widget? current = widget.GetParent();
		while (current != null)
		{
			if (current is Gtk.Window window)
			{
				var ww = window.GetAllocatedWidth();
				var wh = window.GetAllocatedHeight();
				if (ww > 1 && wh > 1) return (ww, wh);
				break;
			}

			// Skip containers that expand to fit children (Fixed, Viewport)
			// and look for ones that impose a size constraint.
			if (current is not Gtk.Fixed && current is not Gtk.Viewport)
			{
				var cw = current.GetAllocatedWidth();
				var ch = current.GetAllocatedHeight();
				if (cw > 1 && ch > 1) return (cw, ch);
			}

			current = current.GetParent();
		}
		return (800, 600);
	}

	public void Add(IView child)
	{
		_ = MauiContext ?? throw new InvalidOperationException("MauiContext not set.");
		try
		{
			var platformChild = (Gtk.Widget)child.ToPlatform(MauiContext);
			PlatformView.Put(platformChild, 0, 0);
			MarkLayoutDirty();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Maui.Linux] Failed to add {child.GetType().Name}: {ex.Message}");
		}
	}

	public void Remove(IView child)
	{
		if (child.Handler?.PlatformView is Gtk.Widget widget)
		{
			PlatformView.Remove(widget);
			MarkLayoutDirty();
		}
	}

	public void Insert(int index, IView child)
	{
		Add(child);
	}

	public void Clear()
	{
		while (PlatformView.GetFirstChild() is Gtk.Widget child)
			PlatformView.Remove(child);
		MarkLayoutDirty();
	}

	public void Update(int index, IView child)
	{
		Remove(child);
		Add(child);
	}

	/// <summary>
	/// Walk up the widget tree to the root GtkLayoutPanel and set its LayoutDirty flag
	/// so the tick callback re-measures and re-arranges on the next frame.
	/// </summary>
	void MarkLayoutDirty()
	{
		PlatformView.LayoutDirty = true;

		// Propagate to ancestor layout panels (root tick callback drives layout)
		Gtk.Widget? current = PlatformView.GetParent();
		while (current != null)
		{
			if (current is GtkLayoutPanel panel)
			{
				panel.LayoutDirty = true;
			}
			current = current.GetParent();
		}
	}

	public void UpdateZIndex(IView child)
	{
		// GTK4 Fixed doesn't have z-ordering, children render in order added
	}

	public static void MapBackground(LayoutHandler handler, ILayout layout)
	{
	}

	public static void MapClipsToBounds(LayoutHandler handler, ILayout layout)
	{
		handler.PlatformView?.SetOverflow(layout.ClipsToBounds ? Gtk.Overflow.Hidden : Gtk.Overflow.Visible);
	}

	public static void MapAdd(LayoutHandler handler, ILayout layout, object? arg)
	{
		if (arg is LayoutHandlerUpdate args)
			handler.Add(args.View);
	}

	public static void MapRemove(LayoutHandler handler, ILayout layout, object? arg)
	{
		if (arg is LayoutHandlerUpdate args)
			handler.Remove(args.View);
	}

	public static void MapInsert(LayoutHandler handler, ILayout layout, object? arg)
	{
		if (arg is LayoutHandlerUpdate args)
			handler.Insert(args.Index, args.View);
	}

	public static void MapClear(LayoutHandler handler, ILayout layout, object? arg)
	{
		handler.Clear();
	}

	public static void MapUpdate(LayoutHandler handler, ILayout layout, object? arg)
	{
		if (arg is LayoutHandlerUpdate args)
			handler.Update(args.Index, args.View);
	}

	public static void MapUpdateZIndex(LayoutHandler handler, ILayout layout, object? arg)
	{
		if (arg is IView view)
			handler.UpdateZIndex(view);
	}
}
