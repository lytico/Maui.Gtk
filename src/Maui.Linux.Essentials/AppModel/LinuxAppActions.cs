using Microsoft.Maui.ApplicationModel;

namespace Maui.Linux.Essentials.AppModel;

public class LinuxAppActions : IAppActions
{
	private EventHandler<AppActionEventArgs>? _appActionActivated;

	public bool IsSupported => false;

	public Task<IEnumerable<AppAction>> GetAsync() =>
		Task.FromResult<IEnumerable<AppAction>>(Array.Empty<AppAction>());

	public Task SetAsync(IEnumerable<AppAction> actions) =>
		Task.CompletedTask; // No-op: Linux .desktop file actions are static

	public event EventHandler<AppActionEventArgs>? AppActionActivated
	{
		add => _appActionActivated += value;
		remove => _appActionActivated -= value;
	}
}
