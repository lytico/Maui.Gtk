using Microsoft.Maui.Controls;

namespace Platform.Maui.Linux.Gtk4.Platform;

/// <summary>
/// Builds a GTK4 PopoverMenuBar from MAUI MenuBarItem/MenuFlyoutItem collections.
/// Also builds a HeaderBar with ToolbarItem buttons.
/// </summary>
public static class GtkMenuBarManager
{
	/// <summary>
	/// Creates a Gtk.PopoverMenuBar from MAUI MenuBarItems.
	/// Returns null if there are no menu items.
	/// </summary>
	public static Gtk.PopoverMenuBar? BuildMenuBar(IList<MenuBarItem>? menuBarItems)
	{
		if (menuBarItems == null || menuBarItems.Count == 0)
			return null;

		var menuModel = Gio.Menu.New();

		foreach (var menuBarItem in menuBarItems)
		{
			var submenu = Gio.Menu.New();

			foreach (var element in menuBarItem)
			{
				if (element is MenuFlyoutItem flyoutItem)
				{
					var item = Gio.MenuItem.New(flyoutItem.Text, null);
					submenu.AppendItem(item);
				}
				else if (element is MenuFlyoutSeparator)
				{
					// Gio.Menu sections act as separators
					var section = Gio.Menu.New();
					submenu.AppendSection(null, section);
				}
			}

			menuModel.AppendSubmenu(menuBarItem.Text, submenu);
		}

		return Gtk.PopoverMenuBar.NewFromModel(menuModel);
	}

	/// <summary>
	/// Creates toolbar buttons for MAUI ToolbarItems and adds them to a HeaderBar.
	/// </summary>
	public static Gtk.HeaderBar? BuildHeaderBar(IList<ToolbarItem>? toolbarItems, string? title = null)
	{
		if (toolbarItems == null || toolbarItems.Count == 0)
			return null;

		var headerBar = Gtk.HeaderBar.New();

		foreach (var item in toolbarItems)
		{
			var button = Gtk.Button.NewWithLabel(item.Text ?? string.Empty);

			if (!string.IsNullOrEmpty(item.IconImageSource?.ToString()))
				button.SetIconName(item.IconImageSource.ToString());

			var capturedItem = item;
			button.OnClicked += (_, _) =>
			{
				if (capturedItem.Command?.CanExecute(capturedItem.CommandParameter) == true)
					capturedItem.Command.Execute(capturedItem.CommandParameter);
			};

			button.SetSensitive(item.IsEnabled);

			if (item.Order == ToolbarItemOrder.Primary)
				headerBar.PackEnd(button);
			else
				headerBar.PackStart(button);
		}

		return headerBar;
	}

	/// <summary>
	/// Applies menu bar and toolbar items to a GTK window.
	/// Menu bar goes into the WindowRootViewContainer; toolbar into the window titlebar.
	/// </summary>
	public static void ApplyToWindow(Gtk.Window window, Page? page)
	{
		if (page == null)
			return;

		var rootContainer = window.GetChild() as WindowRootViewContainer;

		// Apply MenuBar
		if (page.MenuBarItems.Count > 0 && rootContainer != null)
		{
			var menuBar = BuildMenuBar(page.MenuBarItems);
			if (menuBar != null)
				rootContainer.SetMenuBar(menuBar);
		}

		// Apply ToolbarItems
		if (page.ToolbarItems.Count > 0)
		{
			var headerBar = BuildHeaderBar(page.ToolbarItems, page.Title);
			if (headerBar != null)
				window.SetTitlebar(headerBar);
		}
	}
}
