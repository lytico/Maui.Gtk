using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Platform;

/// <summary>
/// Theme/dark mode support for GTK4.
/// Maps MAUI AppTheme to GTK4 settings.
/// </summary>
public static class GtkThemeManager
{
	static bool _monitoring;

	/// <summary>
	/// Sets the GTK4 application theme based on MAUI AppTheme.
	/// </summary>
	public static void SetTheme(AppTheme theme)
	{
		var settings = Gtk.Settings.GetDefault();
		if (settings == null)
			return;

		settings.GtkApplicationPreferDarkTheme = theme == AppTheme.Dark;
	}

	/// <summary>
	/// Gets the current GTK4 theme as a MAUI AppTheme.
	/// </summary>
	public static AppTheme GetCurrentTheme()
	{
		var settings = Gtk.Settings.GetDefault();
		if (settings == null)
			return AppTheme.Unspecified;

		return settings.GtkApplicationPreferDarkTheme ? AppTheme.Dark : AppTheme.Light;
	}

	/// <summary>
	/// Starts monitoring GTK settings for system theme changes.
	/// Fires IApplication.ThemeChanged() when the system theme switches.
	/// </summary>
	public static void StartMonitoring()
	{
		if (_monitoring) return;
		_monitoring = true;

		var settings = Gtk.Settings.GetDefault();
		if (settings == null) return;

		settings.OnNotify += (sender, args) =>
		{
			if (args.Pspec.GetName() == "gtk-application-prefer-dark-theme" ||
				args.Pspec.GetName() == "gtk-theme-name")
			{
				var app = IPlatformApplication.Current as GtkMauiApplication;
				(app?.Application as IApplication)?.ThemeChanged();
			}
		};
	}

	/// <summary>
	/// Applies custom CSS to the entire application.
	/// </summary>
	public static void ApplyCustomCss(string css)
	{
		var provider = Gtk.CssProvider.New();
		provider.LoadFromString(css);
		var display = Gdk.Display.GetDefault();
		if (display != null)
		{
			Gtk.StyleContext.AddProviderForDisplay(display, provider, 800);
		}
	}
}
