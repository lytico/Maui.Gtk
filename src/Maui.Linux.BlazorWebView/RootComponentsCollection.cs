using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components.Web;

namespace Maui.Linux.BlazorWebView;

public class RootComponentsCollection : ObservableCollection<RootComponent>
{
	internal JSComponentConfigurationStore JSComponents { get; } = new();
}
