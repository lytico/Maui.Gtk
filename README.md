# Maui.Linux

A community-driven .NET MAUI backend for Linux, powered by **GTK4**. Run your .NET MAUI applications natively on Linux desktops with GTK4 rendering via [GirCore](https://github.com/gircore/gir.core) bindings.

> **Status:** Early / experimental — contributions and feedback are welcome!

## Features

- **Native GTK4 rendering** — MAUI controls map to real GTK4 widgets.
- **Blazor Hybrid support** — Host Blazor components inside a native GTK window using WebKitGTK.
- **Broad control coverage** — Label, Button, Entry, Editor, CheckBox, Switch, Slider, ProgressBar, ActivityIndicator, Image, Picker, DatePicker, TimePicker, Stepper, RadioButton, SearchBar, ScrollView, Border, Frame, ImageButton, WebView, CollectionView, GraphicsView, Shapes, and more.
- **Layout support** — StackLayout, Grid, FlexLayout, AbsoluteLayout via a custom `GtkLayoutPanel`.
- **Navigation** — NavigationPage, TabbedPage, and FlyoutPage handlers.
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

## Getting Started

### 1. Build the solution

```bash
dotnet restore
dotnet build
```

### 2. Run the sample app

```bash
dotnet run --project samples/Maui.Linux.Sample
```

Or the Blazor Hybrid sample:

```bash
dotnet run --project samples/Maui.Linux.BlazorSample
```

### 3. Use in your own project

Add a project reference to `Maui.Linux` (and optionally `Maui.Linux.BlazorWebView`), then set up your entry point:

**Program.cs**

```csharp
using Maui.Linux.Platform;
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
using Maui.Linux.Hosting;
using Microsoft.Maui.Hosting;

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
```

## Project Structure

```
Maui.Linux.slnx                          # Solution file
├── src/
│   ├── Maui.Linux/                       # Core MAUI backend
│   │   ├── Handlers/                     # GTK4 handler implementations for MAUI controls
│   │   ├── Hosting/                      # AppHostBuilderExtensions (UseMauiAppLinux)
│   │   └── Platform/                     # GTK application, context, layout, theming
│   └── Maui.Linux.BlazorWebView/         # BlazorWebView support via WebKitGTK
├── samples/
│   ├── Maui.Linux.Sample/                # Standard MAUI sample app
│   └── Maui.Linux.BlazorSample/          # Blazor Hybrid sample app
└── docs/                                 # Documentation (TBD)
```

## Key Packages

| Package | Purpose |
|---|---|
| `GirCore.Gtk-4.0` | GObject introspection bindings for GTK4 |
| `GirCore.WebKit-6.0` | WebKitGTK bindings (Blazor support) |
| `Microsoft.Maui.Controls` | .NET MAUI framework |
| `Microsoft.AspNetCore.Components.WebView.Maui` | Blazor Hybrid infrastructure |

## License

This project is licensed under the [MIT License](LICENSE).
