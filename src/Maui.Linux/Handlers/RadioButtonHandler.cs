using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class RadioButtonHandler : GtkViewHandler<IRadioButton, Gtk.CheckButton>
{
	static readonly Dictionary<string, WeakReference<Gtk.CheckButton>> _groupLeaders = new();

	public static new IPropertyMapper<IRadioButton, RadioButtonHandler> Mapper =
		new PropertyMapper<IRadioButton, RadioButtonHandler>(ViewMapper)
		{
			[nameof(IRadioButton.IsChecked)] = MapIsChecked,
			[nameof(IRadioButton.Content)] = MapContent,
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
		UpdateGroup();
	}

	protected override void DisconnectHandler(Gtk.CheckButton platformView)
	{
		platformView.OnToggled -= OnToggled;
		base.DisconnectHandler(platformView);
	}

	void UpdateGroup()
	{
		if (VirtualView is not Microsoft.Maui.Controls.RadioButton rb || string.IsNullOrEmpty(rb.GroupName))
			return;

		if (_groupLeaders.TryGetValue(rb.GroupName, out var leaderRef) &&
			leaderRef.TryGetTarget(out var leader) && leader != PlatformView)
		{
			PlatformView.SetGroup(leader);
		}
		else
		{
			_groupLeaders[rb.GroupName] = new WeakReference<Gtk.CheckButton>(PlatformView);
		}
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

	public static void MapContent(RadioButtonHandler handler, IRadioButton radioButton)
	{
		if (radioButton.Content is string text)
			handler.PlatformView?.SetLabel(text);
	}
}
