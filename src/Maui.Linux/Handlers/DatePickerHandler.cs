using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class DatePickerHandler : GtkViewHandler<IDatePicker, Gtk.Box>
{
	public static new IPropertyMapper<IDatePicker, DatePickerHandler> Mapper =
		new PropertyMapper<IDatePicker, DatePickerHandler>(ViewMapper)
		{
			[nameof(IDatePicker.Date)] = MapDate,
			[nameof(IDatePicker.Format)] = MapFormat,
		};

	public DatePickerHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		// Use a button that opens a calendar popover
		var box = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		var button = Gtk.Button.NewWithLabel(DateTime.Today.ToShortDateString());
		box.Append(button);
		return box;
	}

	public static void MapDate(DatePickerHandler handler, IDatePicker datePicker)
	{
		var button = handler.PlatformView?.GetFirstChild() as Gtk.Button;
		var date = datePicker.Date ?? DateTime.Today;
		button?.SetLabel(date.ToString(datePicker.Format ?? "d"));
	}

	public static void MapFormat(DatePickerHandler handler, IDatePicker datePicker)
	{
		MapDate(handler, datePicker);
	}
}
