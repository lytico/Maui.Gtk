using Microsoft.Maui;

namespace Maui.Linux.Handlers;

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
		var font = label.Font;
		var css = string.Empty;

		if (font.Size > 0)
			css += $"font-size: {font.Size}pt; ";

		if (!string.IsNullOrEmpty(font.Family))
			css += $"font-family: \"{font.Family}\"; ";

		if (font.Weight != Microsoft.Maui.FontWeight.Regular)
			css += $"font-weight: {(int)font.Weight}; ";

		if (font.Slant == Microsoft.Maui.FontSlant.Italic)
			css += "font-style: italic; ";

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
}
