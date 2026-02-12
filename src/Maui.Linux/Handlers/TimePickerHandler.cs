using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class TimePickerHandler : GtkViewHandler<ITimePicker, Gtk.Box>
{
	public static new IPropertyMapper<ITimePicker, TimePickerHandler> Mapper =
		new PropertyMapper<ITimePicker, TimePickerHandler>(ViewMapper)
		{
			[nameof(ITimePicker.Time)] = MapTime,
			[nameof(ITimePicker.Format)] = MapFormat,
		};

	public TimePickerHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		var box = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		var button = Gtk.Button.NewWithLabel(DateTime.Now.ToShortTimeString());
		box.Append(button);
		return box;
	}

	public static void MapTime(TimePickerHandler handler, ITimePicker timePicker)
	{
		var button = handler.PlatformView?.GetFirstChild() as Gtk.Button;
		var dt = DateTime.Today.Add(timePicker.Time ?? TimeSpan.Zero);
		button?.SetLabel(dt.ToString(timePicker.Format ?? "t"));
	}

	public static void MapFormat(TimePickerHandler handler, ITimePicker timePicker)
	{
		MapTime(handler, timePicker);
	}
}
