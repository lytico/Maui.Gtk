using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class RadioButtonHandler : GtkViewHandler<IRadioButton, Gtk.CheckButton>
{
	public static new IPropertyMapper<IRadioButton, RadioButtonHandler> Mapper =
		new PropertyMapper<IRadioButton, RadioButtonHandler>(ViewMapper)
		{
			[nameof(IRadioButton.IsChecked)] = MapIsChecked,
		};

	public RadioButtonHandler() : base(Mapper)
	{
	}

	protected override Gtk.CheckButton CreatePlatformView()
	{
		return Gtk.CheckButton.New();
	}

	protected override void ConnectHandler(Gtk.CheckButton platformView)
	{
		base.ConnectHandler(platformView);
		platformView.OnToggled += OnToggled;
	}

	protected override void DisconnectHandler(Gtk.CheckButton platformView)
	{
		platformView.OnToggled -= OnToggled;
		base.DisconnectHandler(platformView);
	}

	void OnToggled(Gtk.CheckButton sender, EventArgs args)
	{
		if (VirtualView != null)
			VirtualView.IsChecked = sender.GetActive();
	}

	public static void MapIsChecked(RadioButtonHandler handler, IRadioButton radioButton)
	{
		if (handler.PlatformView?.GetActive() != radioButton.IsChecked)
			handler.PlatformView?.SetActive(radioButton.IsChecked);
	}
}
