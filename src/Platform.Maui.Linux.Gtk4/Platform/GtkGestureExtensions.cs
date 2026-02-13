using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Platform;

/// <summary>
/// Gesture recognizer support for GTK4.
/// Maps MAUI gesture recognizers to GTK4 gesture controllers.
/// </summary>
public static class GtkGestureExtensions
{
	/// <summary>
	/// Attaches MAUI gesture recognizers to a GTK widget.
	/// </summary>
	public static void AttachGestures(Gtk.Widget widget, IView view)
	{
		if (view is not Microsoft.Maui.Controls.View mauiView)
			return;

		foreach (var recognizer in mauiView.GestureRecognizers)
		{
			switch (recognizer)
			{
				case Microsoft.Maui.Controls.TapGestureRecognizer tapRecognizer:
					AttachTapGesture(widget, tapRecognizer);
					break;
				case Microsoft.Maui.Controls.PanGestureRecognizer panRecognizer:
					AttachPanGesture(widget, panRecognizer);
					break;
				case Microsoft.Maui.Controls.PointerGestureRecognizer pointerRecognizer:
					AttachPointerGesture(widget, pointerRecognizer);
					break;
			}
		}
	}

	private static void AttachTapGesture(Gtk.Widget widget, Microsoft.Maui.Controls.TapGestureRecognizer recognizer)
	{
		var gesture = Gtk.GestureClick.New();
		gesture.OnReleased += (_, args) =>
		{
			recognizer.Command?.Execute(recognizer.CommandParameter);
		};
		widget.AddController(gesture);
	}

	private static void AttachPanGesture(Gtk.Widget widget, Microsoft.Maui.Controls.PanGestureRecognizer recognizer)
	{
		var gesture = Gtk.GestureDrag.New();
		gesture.OnDragUpdate += (_, args) =>
		{
			// Pan gesture update - args contain offset
		};
		widget.AddController(gesture);
	}

	private static void AttachPointerGesture(Gtk.Widget widget, Microsoft.Maui.Controls.PointerGestureRecognizer recognizer)
	{
		var motion = Gtk.EventControllerMotion.New();
		motion.OnEnter += (_, args) =>
		{
			recognizer.PointerEnteredCommand?.Execute(recognizer.PointerEnteredCommandParameter);
		};
		motion.OnLeave += (_, _) =>
		{
			recognizer.PointerExitedCommand?.Execute(recognizer.PointerExitedCommandParameter);
		};
		widget.AddController(motion);
	}
}
