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
				var (width, height) = GetWindowSize(PlatformView);
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

		// Only the root layout monitors window resize and content changes
		GLib.Functions.IdleAdd(0, () =>
		{
			if (platformView.GetParent() is Platform.GtkLayoutPanel)
				return false; // nested layout — parent drives layout

			int lastW = 0, lastH = 0, lastChildCount = -1;
			platformView.AddTickCallback((widget, clock) =>
			{
				if (VirtualView == null) return false;

				var (w, h) = GetWindowSize(widget);
				int childCount = (VirtualView as ILayout)?.Count ?? 0;

				if (w > 1 && h > 1 && (w != lastW || h != lastH || childCount != lastChildCount))
				{
					lastW = w;
					lastH = h;
					lastChildCount = childCount;
					platformView.CrossPlatformMeasure(w, h);
					platformView.CrossPlatformArrange(new Rect(0, 0, w, h));
				}
				return true;
			});
			return false;
		});
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		return PlatformView.CrossPlatformMeasure(widthConstraint, heightConstraint);
	}

	public override void PlatformArrange(Rect rect)
	{
		PlatformView.CrossPlatformArrange(rect);
	}

	private static (int width, int height) GetWindowSize(Gtk.Widget widget)
	{
		Gtk.Widget? current = widget;
		while (current != null && current is not Gtk.Window)
			current = current.GetParent();

		if (current is Gtk.Window window)
		{
			var w = window.GetAllocatedWidth();
			var h = window.GetAllocatedHeight();
			if (w > 1 && h > 1) return (w, h);
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
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Maui.Linux] Failed to add {child.GetType().Name}: {ex.Message}");
		}
	}

	public void Remove(IView child)
	{
		if (child.Handler?.PlatformView is Gtk.Widget widget)
			PlatformView.Remove(widget);
	}

	public void Insert(int index, IView child)
	{
		Add(child);
	}

	public void Clear()
	{
		while (PlatformView.GetFirstChild() is Gtk.Widget child)
			PlatformView.Remove(child);
	}

	public void Update(int index, IView child)
	{
		Remove(child);
		Add(child);
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
