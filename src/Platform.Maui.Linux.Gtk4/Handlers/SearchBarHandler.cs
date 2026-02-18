using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

public class SearchBarHandler : GtkViewHandler<ISearchBar, Gtk.SearchEntry>
{
	public static new IPropertyMapper<ISearchBar, SearchBarHandler> Mapper =
		new PropertyMapper<ISearchBar, SearchBarHandler>(ViewMapper)
		{
			[nameof(ISearchBar.Text)] = MapText,
			[nameof(ISearchBar.Placeholder)] = MapPlaceholder,
			[nameof(ISearchBar.TextColor)] = MapTextColor,
			[nameof(ITextStyle.Font)] = MapFont,
			[nameof(ISearchBar.CharacterSpacing)] = MapCharacterSpacing,
			[nameof(ISearchBar.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
			[nameof(ISearchBar.IsReadOnly)] = MapIsReadOnly,
			[nameof(ISearchBar.MaxLength)] = MapMaxLength,
			[nameof(ISearchBar.PlaceholderColor)] = MapPlaceholderColor,
			[nameof(ISearchBar.CancelButtonColor)] = MapCancelButtonColor,
			[nameof(ISearchBar.IsSpellCheckEnabled)] = MapIsSpellCheckEnabled,
			[nameof(ISearchBar.IsTextPredictionEnabled)] = MapIsTextPredictionEnabled,
			[nameof(ITextAlignment.VerticalTextAlignment)] = MapVerticalTextAlignment,
		};

	public SearchBarHandler() : base(Mapper)
	{
	}

	protected override Gtk.SearchEntry CreatePlatformView()
	{
		return Gtk.SearchEntry.New();
	}

	protected override void ConnectHandler(Gtk.SearchEntry platformView)
	{
		base.ConnectHandler(platformView);
		platformView.OnSearchChanged += OnSearchChanged;
	}

	protected override void DisconnectHandler(Gtk.SearchEntry platformView)
	{
		platformView.OnSearchChanged -= OnSearchChanged;
		base.DisconnectHandler(platformView);
	}

	void OnSearchChanged(Gtk.SearchEntry sender, EventArgs args)
	{
		if (VirtualView != null)
			VirtualView.Text = sender.GetText();
	}

	public static void MapText(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (handler.PlatformView?.GetText() != searchBar.Text)
			handler.PlatformView?.SetText(searchBar.Text ?? string.Empty);
	}

	public static void MapPlaceholder(SearchBarHandler handler, ISearchBar searchBar)
	{
		handler.PlatformView?.SetPlaceholderText(searchBar.Placeholder ?? string.Empty);
	}

	public static void MapTextColor(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (searchBar.TextColor != null)
			handler.ApplyCss(handler.PlatformView, $"color: {ToGtkColor(searchBar.TextColor)};");
	}

	public static void MapFont(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (searchBar is not ITextStyle textStyle)
			return;

		var css = handler.BuildFontCss(textStyle.Font);
		if (!string.IsNullOrEmpty(css))
			handler.ApplyCss(handler.PlatformView, css);
	}

	public static void MapCharacterSpacing(SearchBarHandler handler, ISearchBar searchBar)
	{
		handler.ApplyCss(handler.PlatformView, $"letter-spacing: {searchBar.CharacterSpacing}px;");
	}

	public static void MapHorizontalTextAlignment(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (handler.PlatformView == null) return;
		var align = searchBar.HorizontalTextAlignment switch
		{
			TextAlignment.Start => "left",
			TextAlignment.Center => "center",
			TextAlignment.End => "right",
			_ => "left"
		};
		handler.ApplyCss(handler.PlatformView, $"text {{ text-align: {align}; }}");
	}

	public static void MapIsReadOnly(SearchBarHandler handler, ISearchBar searchBar)
	{
		handler.PlatformView?.SetEditable(!searchBar.IsReadOnly);
	}

	public static void MapMaxLength(SearchBarHandler handler, ISearchBar searchBar)
	{
		// GTK4 SearchEntry doesn't expose SetMaxLength; enforce via MaxWidthChars as a hint.
		if (handler.PlatformView == null) return;
		if (searchBar.MaxLength > 0)
			handler.PlatformView.SetMaxWidthChars(searchBar.MaxLength);
	}

	public static void MapPlaceholderColor(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (searchBar.PlaceholderColor != null)
			handler.ApplyCssWithSelector(handler.PlatformView, "* > text > placeholder",
				$"color: {ToGtkColor(searchBar.PlaceholderColor)};");
	}

	public static void MapCancelButtonColor(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (searchBar.CancelButtonColor != null)
			handler.ApplyCssWithSelector(handler.PlatformView, "* > image:last-child",
				$"color: {ToGtkColor(searchBar.CancelButtonColor)};");
	}

	public static void MapIsSpellCheckEnabled(SearchBarHandler handler, ISearchBar searchBar)
	{
		// GTK SearchEntry does not have built-in spell-check; intentional no-op.
	}

	public static void MapIsTextPredictionEnabled(SearchBarHandler handler, ISearchBar searchBar)
	{
		// GTK SearchEntry does not support text prediction; intentional no-op.
	}

	public static void MapVerticalTextAlignment(SearchBarHandler handler, ISearchBar searchBar)
	{
		if (handler.PlatformView == null || searchBar is not ITextAlignment ta) return;
		handler.PlatformView.SetValign(ta.VerticalTextAlignment switch
		{
			TextAlignment.Start => Gtk.Align.Start,
			TextAlignment.Center => Gtk.Align.Center,
			TextAlignment.End => Gtk.Align.End,
			_ => Gtk.Align.Center
		});
	}
}
