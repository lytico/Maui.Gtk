using Microsoft.Maui;

namespace Maui.Linux.Platform;

/// <summary>
/// Root container for window content. Manages page display within a GTK window.
/// </summary>
public class WindowRootViewContainer : Gtk.Box
{
	private Gtk.Widget? _currentPage;

	public WindowRootViewContainer() : base()
	{
		SetOrientation(Gtk.Orientation.Vertical);
		SetVexpand(true);
		SetHexpand(true);
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
