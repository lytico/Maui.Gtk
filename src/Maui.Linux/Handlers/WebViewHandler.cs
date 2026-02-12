using Microsoft.Maui;

namespace Maui.Linux.Handlers;

public class WebViewHandler : GtkViewHandler<IWebView, Gtk.Box>
{
	public static new IPropertyMapper<IWebView, WebViewHandler> Mapper =
		new PropertyMapper<IWebView, WebViewHandler>(ViewMapper)
		{
			[nameof(IWebView.Source)] = MapSource,
		};

	public WebViewHandler() : base(Mapper)
	{
	}

	protected override Gtk.Box CreatePlatformView()
	{
		// Placeholder - full WebKit.WebView integration requires GirCore.WebKit-6.0
		// which is in the BlazorWebView project
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		var label = Gtk.Label.New("WebView placeholder - requires WebKitGTK");
		box.Append(label);
		return box;
	}

	public static void MapSource(WebViewHandler handler, IWebView webView)
	{
		// WebView source loading would use WebKit.WebView.LoadUri()
	}
}
