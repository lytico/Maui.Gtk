using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace Maui.Linux.Handlers;

public class NavigationPageHandler : GtkViewHandler<IStackNavigationView, Gtk.Stack>
{
	public static new IPropertyMapper<IStackNavigationView, NavigationPageHandler> Mapper =
		new PropertyMapper<IStackNavigationView, NavigationPageHandler>(ViewMapper)
		{
		};

	public static CommandMapper<IStackNavigationView, NavigationPageHandler> CommandMapper = new(ViewCommandMapper)
	{
		[nameof(IStackNavigation.RequestNavigation)] = MapRequestNavigation,
	};

	public NavigationPageHandler() : base(Mapper, CommandMapper)
	{
	}

	protected override Gtk.Stack CreatePlatformView()
	{
		var stack = Gtk.Stack.New();
		stack.SetTransitionType(Gtk.StackTransitionType.SlideLeftRight);
		stack.SetTransitionDuration(250);
		stack.SetVexpand(true);
		stack.SetHexpand(true);
		return stack;
	}

	public static void MapRequestNavigation(NavigationPageHandler handler, IStackNavigationView view, object? arg)
	{
		if (arg is NavigationRequest request)
		{
			handler.HandleNavigationRequest(request);
		}
	}

	void HandleNavigationRequest(NavigationRequest request)
	{
		_ = MauiContext ?? throw new InvalidOperationException("MauiContext not set.");

		var stack = PlatformView;

		// Add new pages
		foreach (var page in request.NavigationStack)
		{
			var name = page.GetHashCode().ToString();
			if (stack.GetChildByName(name) == null)
			{
				var platformPage = (Gtk.Widget)page.ToPlatform(MauiContext);
				stack.AddNamed(platformPage, name);
			}
		}

		// Show the top page
		if (request.NavigationStack.Count > 0)
		{
			var topPage = request.NavigationStack[^1];
			var name = topPage.GetHashCode().ToString();
			stack.SetVisibleChildName(name);
		}
	}
}
