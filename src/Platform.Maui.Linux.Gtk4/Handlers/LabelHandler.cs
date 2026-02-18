using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class LabelHandler : GtkViewHandler<ILabel, Gtk.Label>
{
	public static new IPropertyMapper<ILabel, LabelHandler> Mapper =
		new PropertyMapper<ILabel, LabelHandler>(ViewMapper)
		{
			[nameof(ILabel.Text)] = MapText,
			[nameof(ILabel.TextColor)] = MapTextColor,
			[nameof(ILabel.Font)] = MapFont,
			[nameof(ILabel.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
			[nameof(ILabel.Padding)] = MapPadding,
			[nameof(ILabel.TextDecorations)] = MapTextDecorations,
			[nameof(ILabel.CharacterSpacing)] = MapCharacterSpacing,
			[nameof(ILabel.LineHeight)] = MapLineHeight,
			[nameof(ITextAlignment.VerticalTextAlignment)] = MapVerticalTextAlignment,
		};

	public LabelHandler() : base(Mapper)
	{
	}

	protected override Gtk.Label CreatePlatformView()
	{
		var label = Gtk.Label.New(string.Empty);
		label.SetWrap(true);
		label.SetXalign(0);
		return label;
	}

	public static void MapText(LabelHandler handler, ILabel label)
	{
		handler.PlatformView?.SetText(label.Text ?? string.Empty);
	}

	public static void MapTextColor(LabelHandler handler, ILabel label)
	{
		if (label.TextColor != null)
		{
			handler.ApplyCss(handler.PlatformView, $"color: {ToGtkColor(label.TextColor)};");
		}
	}

	public static void MapFont(LabelHandler handler, ILabel label)
	{
		var css = handler.BuildFontCss(label.Font);

		if (!string.IsNullOrEmpty(css))
			handler.ApplyCss(handler.PlatformView, css);
	}

	public static void MapHorizontalTextAlignment(LabelHandler handler, ILabel label)
	{
		handler.PlatformView?.SetXalign(label.HorizontalTextAlignment switch
		{
			TextAlignment.Start => 0f,
			TextAlignment.Center => 0.5f,
			TextAlignment.End => 1f,
			_ => 0f
		});
	}

	public static void MapPadding(LabelHandler handler, ILabel label)
	{
		var padding = label.Padding;
		handler.PlatformView?.SetMarginStart((int)padding.Left);
		handler.PlatformView?.SetMarginEnd((int)padding.Right);
		handler.PlatformView?.SetMarginTop((int)padding.Top);
		handler.PlatformView?.SetMarginBottom((int)padding.Bottom);
	}

	public static void MapTextDecorations(LabelHandler handler, ILabel label)
	{
		if (label.TextDecorations.HasFlag(TextDecorations.Underline))
		{
			handler.ApplyCss(handler.PlatformView, "text-decoration: underline;");
		}
		else if (label.TextDecorations.HasFlag(TextDecorations.Strikethrough))
		{
			handler.ApplyCss(handler.PlatformView, "text-decoration: line-through;");
		}
	}

	public static void MapCharacterSpacing(LabelHandler handler, ILabel label)
	{
		handler.ApplyCss(handler.PlatformView, $"letter-spacing: {label.CharacterSpacing}px;");
	}

	public static void MapLineHeight(LabelHandler handler, ILabel label)
	{
		if (label.LineHeight > 0)
			handler.ApplyCss(handler.PlatformView, $"line-height: {label.LineHeight};");
	}

	public static void MapVerticalTextAlignment(LabelHandler handler, ILabel label)
	{
		if (handler.PlatformView == null) return;
		handler.PlatformView.SetYalign(label.VerticalTextAlignment switch
		{
			TextAlignment.Start => 0f,
			TextAlignment.Center => 0.5f,
			TextAlignment.End => 1f,
			_ => 0.5f
		});
	}
}
