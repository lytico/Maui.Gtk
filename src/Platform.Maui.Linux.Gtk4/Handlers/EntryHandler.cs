using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class EntryHandler : GtkViewHandler<IEntry, Gtk.Entry>
{
	public static new IPropertyMapper<IEntry, EntryHandler> Mapper =
		new PropertyMapper<IEntry, EntryHandler>(ViewMapper)
		{
			[nameof(IEntry.Text)] = MapText,
			[nameof(IEntry.Placeholder)] = MapPlaceholder,
			[nameof(IEntry.IsPassword)] = MapIsPassword,
			[nameof(IEntry.MaxLength)] = MapMaxLength,
			[nameof(IEntry.IsReadOnly)] = MapIsReadOnly,
			[nameof(IEntry.TextColor)] = MapTextColor,
			[nameof(IEntry.Font)] = MapFont,
			[nameof(IEntry.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
		};

	public EntryHandler() : base(Mapper)
	{
	}

	protected override Gtk.Entry CreatePlatformView()
	{
		return Gtk.Entry.New();
	}

	protected override void ConnectHandler(Gtk.Entry platformView)
	{
		base.ConnectHandler(platformView);
		platformView.OnChanged += OnTextChanged;
	}

	protected override void DisconnectHandler(Gtk.Entry platformView)
	{
		platformView.OnChanged -= OnTextChanged;
		base.DisconnectHandler(platformView);
	}

	void OnTextChanged(Gtk.Editable sender, EventArgs args)
	{
		if (VirtualView != null)
			VirtualView.Text = sender.GetText();
	}

	public static void MapText(EntryHandler handler, IEntry entry)
	{
		if (handler.PlatformView?.GetText() != entry.Text)
			handler.PlatformView?.SetText(entry.Text ?? string.Empty);
	}

	public static void MapPlaceholder(EntryHandler handler, IEntry entry)
	{
		handler.PlatformView?.SetPlaceholderText(entry.Placeholder ?? string.Empty);
	}

	public static void MapIsPassword(EntryHandler handler, IEntry entry)
	{
		handler.PlatformView?.SetVisibility(!entry.IsPassword);
	}

	public static void MapMaxLength(EntryHandler handler, IEntry entry)
	{
		handler.PlatformView?.SetMaxLength(entry.MaxLength);
	}

	public static void MapIsReadOnly(EntryHandler handler, IEntry entry)
	{
		handler.PlatformView?.SetEditable(!entry.IsReadOnly);
	}

	public static void MapTextColor(EntryHandler handler, IEntry entry)
	{
		if (entry.TextColor != null)
			handler.ApplyCss(handler.PlatformView, $"color: {ToGtkColor(entry.TextColor)};");
	}

	public static void MapFont(EntryHandler handler, IEntry entry)
	{
		var css = handler.BuildFontCss(entry.Font);
		if (!string.IsNullOrEmpty(css)) handler.ApplyCss(handler.PlatformView, css);
	}

	public static void MapHorizontalTextAlignment(EntryHandler handler, IEntry entry)
	{
		handler.PlatformView?.SetAlignment(entry.HorizontalTextAlignment switch
		{
			TextAlignment.Start => 0f,
			TextAlignment.Center => 0.5f,
			TextAlignment.End => 1f,
			_ => 0f
		});
	}
}
