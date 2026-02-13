# Platform.Maui.Linux.Gtk4

A community-driven .NET MAUI backend for Linux, powered by **GTK4**. Run your .NET MAUI applications natively on Linux desktops with GTK4 rendering via [GirCore](https://github.com/gircore/gir.core) bindings.

> **Status:** Early / experimental — contributions and feedback are welcome!

https://github.com/user-attachments/assets/039f1695-3cd0-4b0b-ad11-dce304d0cdce

## Features

- **Native GTK4 rendering** — MAUI controls map to real GTK4 widgets.
- **Blazor Hybrid support** — Host Blazor components inside a native GTK window using WebKitGTK.
- **Broad control coverage** — Label, Button, Entry, Editor, CheckBox, Switch, Slider, ProgressBar, ActivityIndicator, Image, Picker, DatePicker, TimePicker, Stepper, RadioButton, SearchBar, ScrollView, Border, Frame, ImageButton, WebView, CollectionView, GraphicsView, Shapes, and more.
- **Layout support** — StackLayout, Grid, FlexLayout, AbsoluteLayout via a custom `GtkLayoutPanel`.
- **Navigation** — NavigationPage, TabbedPage, and FlyoutPage handlers.
- **Alerts & Dialogs** — DisplayAlert, DisplayActionSheet, and DisplayPromptAsync via native GTK4 windows.
- **Essentials** — Clipboard, Preferences, DeviceInfo, AppInfo, Connectivity, and more.
- **Cairo-based graphics** — `GraphicsView` draws via the Microsoft.Maui.Graphics Cairo backend.
- **Theming** — Automatic light/dark theme detection through `GtkThemeManager`.

## Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 10.0+ |
| GTK 4 libraries | 4.x (system package) |
| WebKitGTK *(Blazor only)* | 6.x (system package) |

### Install GTK4 & WebKitGTK (Debian / Ubuntu)

```bash
sudo apt install libgtk-4-dev libwebkitgtk-6.0-dev
```

### Install GTK4 & WebKitGTK (Fedora)

```bash
sudo dnf install gtk4-devel webkitgtk6.0-devel
```

## Quick Start

### Option 1: Use the template (recommended)

```bash
# Install the template
dotnet new install Platform.Maui.Linux.Gtk4.Templates

# Create a new Linux MAUI app
dotnet new maui-linux-gtk4 -n MyApp.Linux
cd MyApp.Linux
dotnet run
```

### Option 2: Add to an existing project manually

Add the NuGet package:

```bash
dotnet add package Platform.Maui.Linux.Gtk4 --prerelease
dotnet add package Platform.Maui.Linux.Gtk4.Essentials --prerelease   # optional
```

Then set up your entry point:

**Program.cs**

```csharp
using Platform.Maui.Linux.Gtk4.Platform;
using Microsoft.Maui.Hosting;

public class Program : GtkMauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public static void Main(string[] args)
    {
        var app = new Program();
        app.Run(args);
    }
}
```

**MauiProgram.cs**

```csharp
using Platform.Maui.Linux.Gtk4.Hosting;
using Microsoft.Maui.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiAppLinuxGtk4<App>();

        return builder.Build();
    }
}
```

## Adding Linux to a Multi-Targeted MAUI App

Since there is no official `-linux` TFM (Target Framework Moniker) from Microsoft, MAUI projects can't conditionally include the Linux backend via `TargetFrameworks` the way they do for Android/iOS/Windows. Instead, use the **"Linux head project"** pattern:

```
MyApp/                              ← Your existing multi-targeted MAUI project
├── MyApp.csproj                       (net10.0-android;net10.0-ios;...)
├── App.cs
├── MainPage.xaml
├── ViewModels/
├── Services/
└── Platforms/
    ├── Android/
    ├── iOS/
    └── ...

MyApp.Linux/                        ← New Linux-specific project
├── MyApp.Linux.csproj                 (net10.0, references Platform.Maui.Linux.Gtk4)
├── Program.cs                         (GtkMauiApplication entry point)
└── MauiProgram.cs                     (builder.UseMauiAppLinuxGtk4<App>())
```

### Setup

1. **Create the Linux head project** next to your MAUI project:

```bash
dotnet new maui-linux-gtk4 -n MyApp.Linux
```

2. **Reference your shared code** — add a project reference from `MyApp.Linux.csproj` to your MAUI project (or a shared class library):

```xml
<!-- MyApp.Linux.csproj -->
<ItemGroup>
  <ProjectReference Include="../MyApp/MyApp.csproj" />
</ItemGroup>
```

3. **Run on Linux:**

```bash
dotnet run --project MyApp.Linux
```

### Why a separate project?

The platform-specific TFMs (`net10.0-android`, `net10.0-ios`, etc.) are powered by .NET workloads that Microsoft ships. Creating a custom `net10.0-linux` TFM would require building and distributing a full .NET workload — complex infrastructure that's unnecessary for most use cases.

The separate project approach is the same pattern used by [OpenMaui](https://github.com/open-maui/maui-linux-gtk4) and [MauiAvalonia](https://github.com/wieslawsoltes/MauiAvalonia). It works with standard `dotnet build`/`dotnet run`, is NuGet-distributable, and keeps your existing MAUI project unchanged.

## Building from Source

```bash
git clone https://github.com/Redth/Platform.Maui.Linux.Gtk4.git
cd Platform.Maui.Linux.Gtk4
dotnet restore
dotnet build
```

### Run the sample app

```bash
# Sample app (includes native controls, Blazor Hybrid, essentials, and more)
dotnet run --project samples/Platform.Maui.Linux.Gtk4.Sample
```

## Project Structure

```
Platform.Maui.Linux.Gtk4.slnx                          # Solution file
├── src/
│   ├── Platform.Maui.Linux.Gtk4/                       # Core MAUI backend
│   │   ├── Handlers/                     # GTK4 handler implementations
│   │   ├── Hosting/                      # AppHostBuilderExtensions (UseMauiAppLinuxGtk4)
│   │   └── Platform/                     # GTK application, context, layout, theming
│   ├── Platform.Maui.Linux.Gtk4.Essentials/            # MAUI Essentials for Linux (clipboard, etc.)
│   └── Platform.Maui.Linux.Gtk4.BlazorWebView/         # BlazorWebView support via WebKitGTK
├── samples/
│   └── Platform.Maui.Linux.Gtk4.Sample/                # Sample app (controls, Blazor, essentials)
├── templates/                            # dotnet new templates
└── docs/                                 # Documentation
```

## NuGet Packages

| Package | Purpose |
|---|---|
| `Platform.Maui.Linux.Gtk4` | Core GTK4 backend — handlers, hosting, platform services |
| `Platform.Maui.Linux.Gtk4.Essentials` | MAUI Essentials (clipboard, preferences, device info, etc.) |
| `Platform.Maui.Linux.Gtk4.BlazorWebView` | Blazor Hybrid support via WebKitGTK |
| `Platform.Maui.Linux.Gtk4.Templates` | `dotnet new` project templates |

## Key Dependencies

| Package | Purpose |
|---|---|
| `GirCore.Gtk-4.0` | GObject introspection bindings for GTK4 |
| `GirCore.WebKit-6.0` | WebKitGTK bindings (Blazor support) |
| `Microsoft.Maui.Controls` | .NET MAUI framework |
| `Tmds.DBus.Protocol` | D-Bus client for Linux platform services |

## License

This project is licensed under the [MIT License](LICENSE).
