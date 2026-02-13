using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class EditorHandler : GtkViewHandler<IEditor, Gtk.TextView>
{
	public static new IPropertyMapper<IEditor, EditorHandler> Mapper =
		new PropertyMapper<IEditor, EditorHandler>(ViewMapper)
		{
			[nameof(IEditor.Text)] = MapText,
			[nameof(IEditor.IsReadOnly)] = MapIsReadOnly,
			[nameof(IEditor.TextColor)] = MapTextColor,
			[nameof(IEditor.Font)] = MapFont,
			[nameof(IEditor.Placeholder)] = MapPlaceholder,
		};

	public EditorHandler() : base(Mapper)
	{
	}

	protected override Gtk.TextView CreatePlatformView()
	{
		var textView = Gtk.TextView.New();
		textView.SetWrapMode(Gtk.WrapMode.Word);
		return textView;
	}

	public static void MapText(EditorHandler handler, IEditor editor)
	{
		var buffer = handler.PlatformView?.GetBuffer();
		if (buffer != null)
		{
			buffer.GetStartIter(out var start);
			buffer.GetEndIter(out var end);
			var currentText = buffer.GetText(start, end, false);
			if (currentText != editor.Text)
				buffer.SetText(editor.Text ?? string.Empty, -1);
		}
	}

	public static void MapIsReadOnly(EditorHandler handler, IEditor editor)
	{
		handler.PlatformView?.SetEditable(!editor.IsReadOnly);
	}

	public static void MapTextColor(EditorHandler handler, IEditor editor)
	{
		if (editor.TextColor != null)
			handler.ApplyCss(handler.PlatformView, $"color: {ToGtkColor(editor.TextColor)};");
	}

	public static void MapFont(EditorHandler handler, IEditor editor)
	{
		var font = editor.Font;
		var css = string.Empty;
		if (font.Size > 0) css += $"font-size: {font.Size}pt; ";
		if (!string.IsNullOrEmpty(font.Family)) css += $"font-family: \"{font.Family}\"; ";
		if (!string.IsNullOrEmpty(css)) handler.ApplyCss(handler.PlatformView, css);
	}

	public static void MapPlaceholder(EditorHandler handler, IEditor editor)
	{
		// GTK TextView doesn't have built-in placeholder; would need overlay label
	}
}
