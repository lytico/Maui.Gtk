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
}
