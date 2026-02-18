using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Platform;

/// <summary>
/// Root container for window content. Manages page display within a GTK window.
/// Supports an optional menu bar above the page content.
/// </summary>
public class WindowRootViewContainer : Gtk.Box
{
	private Gtk.Widget? _currentPage;
	private Gtk.Widget? _menuBar;

	public WindowRootViewContainer() : base()
	{
		SetOrientation(Gtk.Orientation.Vertical);
		SetVexpand(true);
		SetHexpand(true);
	}

	public void SetMenuBar(Gtk.Widget menuBar)
	{
		if (_menuBar != null)
			Remove(_menuBar);

		_menuBar = menuBar;
		// Menu bar goes at the top, before the page content
		Prepend(menuBar);
	}

	public void AddPage(Gtk.Widget page)
	{
		if (_currentPage != null)
		{
			Remove(_currentPage);
		}

		_currentPage = page;
		Append(page);
	}

	public void RemovePage(Gtk.Widget page)
	{
		if (_currentPage == page)
		{
			Remove(page);
			_currentPage = null;
		}
	}
}
