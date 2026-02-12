using Maui.Linux.Hosting;
using Microsoft.Maui.Hosting;

namespace MauiLinuxApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiAppLinux<App>();

        return builder.Build();
    }
}
