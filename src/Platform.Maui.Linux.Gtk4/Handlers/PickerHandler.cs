using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class PickerHandler : GtkViewHandler<IPicker, Gtk.DropDown>
{
	public static new IPropertyMapper<IPicker, PickerHandler> Mapper =
		new PropertyMapper<IPicker, PickerHandler>(ViewMapper)
		{
			[nameof(IPicker.Title)] = MapTitle,
			[nameof(IPicker.SelectedIndex)] = MapSelectedIndex,
			[nameof(IPicker.Items)] = MapItems,
		};

	public PickerHandler() : base(Mapper)
	{
	}

	protected override Gtk.DropDown CreatePlatformView()
	{
		var stringList = Gtk.StringList.New(Array.Empty<string>());
		return Gtk.DropDown.New(stringList, null);
	}

	protected override void ConnectHandler(Gtk.DropDown platformView)
	{
		base.ConnectHandler(platformView);
		platformView.OnNotify += OnSelectedChanged;
	}

	protected override void DisconnectHandler(Gtk.DropDown platformView)
	{
		platformView.OnNotify -= OnSelectedChanged;
		base.DisconnectHandler(platformView);
	}

	void OnSelectedChanged(GObject.Object sender, GObject.Object.NotifySignalArgs args)
	{
		if (args.Pspec.GetName() == "selected" && VirtualView != null)
			VirtualView.SelectedIndex = (int)PlatformView.GetSelected();
	}

	public static void MapTitle(PickerHandler handler, IPicker picker)
	{
		// GTK DropDown doesn't have a direct title; could use a label
	}

	public static void MapSelectedIndex(PickerHandler handler, IPicker picker)
	{
		if (picker.SelectedIndex >= 0)
			handler.PlatformView?.SetSelected((uint)picker.SelectedIndex);
	}

	public static void MapItems(PickerHandler handler, IPicker picker)
	{
		var items = picker.Items?.ToArray() ?? Array.Empty<string>();
		var stringList = Gtk.StringList.New(items);
		handler.PlatformView?.SetModel(stringList);
	}
}
