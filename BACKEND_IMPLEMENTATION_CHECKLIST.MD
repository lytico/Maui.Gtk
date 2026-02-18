# Platform.Maui.Linux.Gtk4 — Backend Implementation Checklist

Implementation status of the .NET MAUI backend for Linux using GTK4. Adapted from the generic MAUI backend checklist.

> ✅ = Implemented  |  ⚠️ = Partial  |  ❌ = Not implemented  |  N/A = Not applicable to Linux/GTK4

---

## 1. Core Infrastructure

### Platform Abstractions
- [x] **Platform View Type** — `Gtk.Widget` as the native view abstraction via `GtkViewHandler<TVirtualView, TPlatformView>`
- [x] **Platform Context** — `GtkMauiContext` with DI scope, handler factory, window scope management
- [x] **Dispatcher** — `GtkDispatcherProvider` using `GLib.Functions.IdleAdd` for UI thread marshalling
- [x] **Event System** — GTK4 signal/event model for user interactions (clicks, input, gestures)
- [x] **Handler Factory Integration** — All handlers registered via `ConfigureMauiHandlers` in `AppHostBuilderExtensions`
- [x] **App Host Builder Extension** — `UseLinuxGtkHandlers()` wires up all handlers and services

### Rendering Pipeline
- [x] **View Renderer** — `GtkViewHandler` base traverses virtual view tree, creates/connects GTK widgets
- [x] **Property Change Propagation** — `IPropertyMapper` re-maps views when `IView` property changes fire
- [x] **Child Synchronization** — `GtkLayoutPanel` handles add/remove/reorder of child views
- [x] **Style/Attribute Application** — CSS-based styling via `ApplyCss()` helper for colors, fonts, sizes

### Native Interop
- [x] **GTK4 Signal Handling** — Native GTK signal connections for widget events
- [x] **Gesture Controllers** — `Gtk.GestureClick`, `Gtk.GestureDrag`, `Gtk.EventControllerMotion`
- [x] **Keyboard Event Handling** — GTK `Gtk.EventControllerKey` for Entry/Editor key events
- [x] **Scroll Handling** — `Gtk.ScrolledWindow` for scroll containers

---

## 2. Application & Window

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Application** | Done | App lifecycle via `GtkMauiApplication`, `Gtk.Application` integration |
| ✅ **Window** | Done | Title, Width/Height mapping, default size 1024×768, min 800×600 |

---

## 3. Pages

| Page | Status | Notes |
|------|--------|-------|
| ✅ **ContentPage** | Done | Content rendering, Title, MenuBarItems via `GtkMenuBarManager` |
| ✅ **NavigationPage** | Done | Push/pop stack, back button, title bar, ToolbarItems propagation |
| ✅ **TabbedPage** | Done | `Gtk.Notebook` tab bar, tab selection, content switching |
| ✅ **FlyoutPage** | Done | `Gtk.Paned` flyout panel with toggle, resizable divider |
| ✅ **Shell** | Done | Flyout via `Gtk.ListBox`, tab bars via `Gtk.Notebook`, ShellItem/Section/Content hierarchy |

---

## 4. Layouts

| Layout | Status | Notes |
|--------|--------|-------|
| ✅ **VerticalStackLayout** | Done | `GtkLayoutPanel` with vertical `CrossPlatformMeasure`/`CrossPlatformArrange` |
| ✅ **HorizontalStackLayout** | Done | `GtkLayoutPanel` with horizontal layout |
| ✅ **Grid** | Done | Row/column definitions (Absolute, Star, Auto), spans, spacing |
| ✅ **FlexLayout** | Done | Handled by generic `LayoutHandler` — MAUI cross-platform layout engine does measure/arrange |
| ✅ **AbsoluteLayout** | Done | Handled by generic `LayoutHandler` — absolute and proportional positioning |
| ✅ **ScrollView** | Done | `Gtk.ScrolledWindow` with orientation support |
| ✅ **ContentView** | Done | Simple content wrapper |
| ✅ **Border** | Done | Stroke, StrokeThickness, StrokeShape, padding, content via Cairo/CSS |
| ✅ **Frame** | Done | Legacy border with CornerRadius, HasShadow, padding |

---

## 5. Basic Controls

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Label** | Done | Text, TextColor, TextAlignment (H & V), LineBreakMode, MaxLines, LineHeight, CharacterSpacing, TextDecorations, FontAttributes |
| ✅ **Button** | Done | Text, Command, Clicked, BackgroundColor, TextColor, BorderColor, BorderWidth, CornerRadius, CharacterSpacing, font properties |
| ✅ **ImageButton** | Done | Source, Command, Clicked, BackgroundColor, BorderColor, CornerRadius, padding |
| ✅ **Entry** | Done | Text, Placeholder, PlaceholderColor, IsPassword, Keyboard, ReturnType, ReturnCommand, TextChanged, CharacterSpacing, ClearButtonVisibility, MaxLength |
| ✅ **Editor** | Done | Text, Placeholder, PlaceholderColor, TextChanged, AutoSize, CharacterSpacing, MaxLength |
| ✅ **Switch** | Done | `Gtk.Switch` — IsToggled, OnColor, ThumbColor, Toggled event |
| ✅ **CheckBox** | Done | `Gtk.CheckButton` — IsChecked, Color, CheckedChanged |
| ✅ **RadioButton** | Done | `Gtk.CheckButton` with group — IsChecked, GroupName, Content, mutual exclusion |
| ✅ **Slider** | Done | `Gtk.Scale` — Value, Min, Max, MinimumTrackColor, MaximumTrackColor, ThumbColor |
| ✅ **Stepper** | Done | `Gtk.SpinButton` — Value, Min, Max, Increment, ValueChanged |
| ✅ **ProgressBar** | Done | `Gtk.ProgressBar` — Progress, ProgressColor |
| ✅ **ActivityIndicator** | Done | `Gtk.Spinner` — IsRunning, Color |
| ✅ **BoxView** | Done | Color, CornerRadius, WidthRequest, HeightRequest |
| ✅ **Image** | Done | Source (URL, file, stream), Aspect (AspectFit, AspectFill, Fill) |

---

## 6. Input & Selection Controls

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Picker** | Done | `Gtk.DropDown` — ItemsSource, SelectedIndex, SelectedItem, SelectedIndexChanged |
| ✅ **DatePicker** | Done | Date, MinimumDate, MaximumDate, Format, DateSelected |
| ✅ **TimePicker** | Done | Time, Format |
| ✅ **SearchBar** | Done | `Gtk.SearchEntry` — Text, Placeholder, SearchCommand, SearchButtonPressed, TextChanged, MaxLength |

---

## 7. Collection Controls

| Control | Status | Notes |
|---------|--------|-------|
| ⚠️ **CollectionView** | Partial | `Gtk.ListView` — ItemsSource, Header/Footer, SelectionMode, SelectedItem, ItemTemplate/DataTemplateSelector (native GTK rendering). Missing: grouping, virtualization |
| ❌ **ListView** | Missing | No handler — use CollectionView as alternative |
| ⚠️ **CarouselView** | Partial | Horizontal/vertical scrolling with `Gtk.ScrolledWindow`. Missing: snap navigation, Loop, PeekAreaInsets |
| ✅ **IndicatorView** | Done | Cairo-drawn dots with IndicatorColor, SelectedIndicatorColor, IndicatorSize |
| ❌ **TableView** | Missing | No handler |
| ⚠️ **SwipeView** | Partial | Passthrough container — swipe-to-reveal not implemented (desktop pattern) |
| ✅ **RefreshView** | Done | Desktop-friendly: spinner + refresh button, IsRefreshing, RefreshColor, Command |

---

## 8. Navigation & Routing

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **NavigationPage stack** | Done | PushAsync, PopAsync, PopToRootAsync |
| ⚠️ **Shell navigation** | Partial | Flyout, tabs, content hierarchy. Missing: GoToAsync route-based navigation |
| N/A **URL/route-based routing** | N/A | Not applicable to desktop Linux apps |
| N/A **Route ↔ Shell sync** | N/A | Browser-specific concept |
| N/A **Non-Shell routing** | N/A | Browser-specific concept |
| ✅ **Back button** | Done | Nav bar back button with stack awareness |
| ✅ **ToolbarItems** | Done | Primary toolbar items rendered as buttons in NavigationPage nav bar |

---

## 9. Alerts & Dialogs

| Dialog | Status | Notes |
|--------|--------|-------|
| ✅ **DisplayAlert** | Done | Custom GTK Window dialog with Title, message, accept/cancel buttons |
| ✅ **DisplayActionSheet** | Done | Dialog with action buttons, destructive button styling, cancel |
| ✅ **DisplayPromptAsync** | Done | Text entry dialog with placeholder, initial value, validation |
| ✅ **Modal overlay** | Done | GTK modal window with transient-for parent |

---

## 10. Gesture Recognizers

| Gesture | Status | Notes |
|---------|--------|-------|
| ✅ **TapGestureRecognizer** | Done | `Gtk.GestureClick` — single/double tap, Command, Tapped event |
| ✅ **PanGestureRecognizer** | Done | `Gtk.GestureDrag` — PanUpdated with StatusType, TotalX/TotalY |
| ✅ **SwipeGestureRecognizer** | Done | `Gtk.GestureSwipe` — velocity-based direction detection (Left/Right/Up/Down) |
| ❌ **PinchGestureRecognizer** | Missing | Pinch zoom not implemented |
| ✅ **PointerGestureRecognizer** | Done | `Gtk.EventControllerMotion` + `Gtk.GestureClick` — Enter/Move/Exit/Press/Release + Commands |

---

## 11. Graphics & Shapes

### Microsoft.Maui.Graphics
| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **GraphicsView** | Done | Cairo-backed IDrawable rendering via `Gtk.DrawingArea` |
| ✅ **Canvas Operations** | Done | DrawLine, DrawRect, DrawEllipse, DrawPath, DrawString, FillRect, FillEllipse, FillPath |
| ✅ **Canvas State** | Done | SaveState, RestoreState, Translate, Rotate, Scale |
| ✅ **Brushes** | Done | SolidColorBrush, LinearGradientBrush (CSS linear-gradient), RadialGradientBrush (CSS radial-gradient) |

### Shapes
| Shape | Status | Notes |
|-------|--------|-------|
| ✅ **Rectangle** | Done | Rounded corners via RadiusX/RadiusY |
| ✅ **Ellipse** | Done | Full ellipse and circle rendering |
| ✅ **Line** | Done | X1, Y1, X2, Y2 |
| ✅ **Path** | Done | PathGeometry with Line, Bezier, QuadraticBezier, PolyLine, PolyBezier segments |
| ✅ **Polygon** | Done | Points collection, closed shape |
| ✅ **Polyline** | Done | Points collection, open shape |
| ✅ **Fill & Stroke** | Done | SolidColorBrush fill/stroke, StrokeThickness, dash pattern, line cap/join |

---

## 12. Common View Properties (Base Handler)

Every handler must support these properties mapped from the base `IView`:

### Visibility & State
- [x] Opacity — `MapOpacity` applies via CSS
- [x] IsVisible — `MapVisibility` toggles `Gtk.Widget.SetVisible()`
- [x] IsEnabled — `MapIsEnabled` toggles `Gtk.Widget.SetSensitive()`
- [x] InputTransparent — `MapInputTransparent` sets `Gtk.Widget.SetCanTarget()`

### Sizing
- [x] WidthRequest / HeightRequest — Used in `GetDesiredSize()` measurement
- [x] MinimumWidthRequest / MinimumHeightRequest — Used in measurement
- [x] MaximumWidthRequest / MaximumHeightRequest — Constrained in `GetDesiredSize()`

### Layout
- [x] HorizontalOptions / VerticalOptions — Handled by MAUI cross-platform layout
- [x] Margin — Handled by MAUI layout engine (cross-platform)
- [x] Padding — Mapped on individual handlers (Entry, Editor, Button, etc.)
- [x] FlowDirection (LTR, RTL) — `MapFlowDirection` sets `Gtk.TextDirection`
- [ ] ZIndex — Not mapped

### Appearance
- [x] BackgroundColor / Background — `MapBackground` applies via CSS `background-color`

### Transforms
- [x] TranslationX / TranslationY — `Gsk.Transform.Translate()` via `SetChildTransform`
- [x] Rotation / RotationX / RotationY — `Gsk.Transform.Rotate()` via `SetChildTransform`
- [x] Scale / ScaleX / ScaleY — `Gsk.Transform.Scale()` via `SetChildTransform`
- [x] AnchorX / AnchorY — Translate-to-anchor pattern in transform chain

### Effects
- [x] Shadow — `MapShadow` applies via CSS `box-shadow`
- [x] Clip — `MapClip` applies CSS `border-radius` + `overflow:hidden` for RoundRectangleGeometry/EllipseGeometry

### Automation
- [x] AutomationId — `MapAutomationId` sets accessible name
- [x] Semantic properties — `MapSemantics` sets accessibility description

### Transitions/Animation
- [ ] GTK CSS transitions — Not implemented

---

## 13. VisualStateManager & Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **VisualStateManager** | Missing | No GoToState implementation; GTK native widget states (hover, focused) work natively |
| ❌ **PropertyTrigger** | Missing | Not implemented |
| ❌ **DataTrigger** | Missing | Not implemented |
| ❌ **MultiTrigger** | Missing | Not implemented |
| ❌ **EventTrigger** | Missing | Not implemented |
| ❌ **Behaviors** | Missing | Not implemented |

> Note: Many VSM scenarios "just work" because GTK4 natively handles hover, focus, active, and disabled states via its CSS engine.

---

## 14. Font Management

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **IFontManager** | Done | `GtkFontManager` resolves font family names, applies via Pango/CSS |
| ✅ **IFontRegistrar** | Done | `GtkFontRegistrar` registers embedded fonts with aliases |
| ✅ **IEmbeddedFontLoader** | Done | `FileSystemEmbeddedFontLoader` extracts fonts from assembly resources |
| ✅ **Font loading** | Done | Registered via `fc-config` / Fontconfig for system-wide availability |
| ✅ **Font properties** | Done | FontSize, FontFamily, FontAttributes applied to Label, Button, Entry, Editor, etc. |

---

## 15. Essentials / Platform Services

| Service | Interface | Status | Notes |
|---------|-----------|--------|-------|
| ✅ **App Info** | `IAppInfo` | Done | App name, version, build, package name, theme |
| ✅ **App Actions** | `IAppActions` | Done | Desktop quick actions |
| ✅ **Battery** | `IBattery` | Done | UPower-based charge level, state, power source |
| ✅ **Browser** | `IBrowser` | Done | `xdg-open` for URLs |
| ✅ **Clipboard** | `IClipboard` | Done | `Gdk.Clipboard` copy/paste |
| ✅ **Connectivity** | `IConnectivity` | Done | NetworkManager-based network status |
| ✅ **Device Display** | `IDeviceDisplay` | Done | Screen dimensions, density via `Gdk.Display` |
| ✅ **Device Info** | `IDeviceInfo` | Done | Platform, model, manufacturer, OS version |
| ✅ **File Picker** | `IFilePicker` | Done | `Gtk.FileDialog` native file picker |
| ✅ **File System** | `IFileSystem` | Done | XDG data/cache directories |
| ⚠️ **Geolocation** | `IGeolocation` | Partial | GeoClue integration via `where-am-i` CLI — basic location only |
| ✅ **Launcher** | `ILauncher` | Done | `xdg-open` URI launching |
| ✅ **Map** | `IMap` | Done | Opens system maps app |
| ✅ **Media Picker** | `IMediaPicker` | Done | `Gtk.FileDialog` for photo/video selection |
| ✅ **Preferences** | `IPreferences` | Done | JSON file-based key-value storage |
| ✅ **Secure Storage** | `ISecureStorage` | Done | Encrypted file storage with DPAPI-like protection |
| ✅ **Screenshot** | `IScreenshot` | Done | `Gtk.WidgetPaintable` screenshot capture |
| ❌ **Semantic Screen Reader** | `ISemanticScreenReader` | Missing | No AT-SPI integration yet |
| ✅ **Share** | `IShare` | Done | XDG portal share |
| ✅ **Text-to-Speech** | `ITextToSpeech` | Done | `espeak-ng` integration |
| ✅ **Version Tracking** | `IVersionTracking` | Done | File-based version history |
| ❌ **Vibration** | `IVibration` | Stub | Not applicable to most Linux desktops |
| ⚠️ **Web Authenticator** | `IWebAuthenticator` | Partial | Basic OAuth flow via system browser |
| ❌ **Phone Dialer** | `IPhoneDialer` | Stub | Not applicable to desktop |
| ❌ **SMS** | `ISms` | Stub | Not applicable to desktop |
| ❌ **Contacts** | `IContacts` | Stub | Not implemented |
| ❌ **Sensors** | Various | Stub | Accelerometer, Barometer, Compass, Gyroscope — not applicable to desktop |

---

## 16. Styling Infrastructure

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **Border style mapping** | Done | Stroke → CSS border, StrokeShape → border-radius via BorderHandler |
| ✅ **View state mapping** | Done | IsVisible → `SetVisible()`, IsEnabled → `SetSensitive()`, Opacity → CSS |
| ✅ **Automation mapping** | Done | AutomationId → accessible name |
| ✅ **GTK CSS theming** | Done | `GtkThemeManager` for dark/light mode, `ApplyCustomCss()` for global CSS |
| ✅ **Per-widget CSS** | Done | `ApplyCss()` helper for inline style application via `CssProvider` |

---

## 17. WebView

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **URL loading** | Done | WebKit-based navigation to URLs |
| ✅ **HTML content** | Done | Display raw HTML via HtmlWebViewSource |
| ✅ **JavaScript execution** | Done | EvaluateJavaScriptAsync via WebKit |
| ✅ **Navigation events** | Done | Navigating, Navigated with success/failure |
| ✅ **GoBack / GoForward** | Done | Navigation history with CanGoBack/CanGoForward state |
| ✅ **UserAgent** | Done | Custom user agent string |

---

## 18. Label — FormattedText Detail

FormattedText requires Pango markup rendering for rich text:

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **Span rendering** | Done | Pango markup via `Gtk.Label.SetMarkup()` |
| ✅ **Span.Text** | Done | Escaped via `GLib.Functions.MarkupEscapeText` |
| ✅ **Span.TextColor** | Done | Pango `foreground` attribute |
| ✅ **Span.BackgroundColor** | Done | Pango `background` attribute |
| ✅ **Span.FontSize** | Done | Pango `size` attribute (in Pango units) |
| ✅ **Span.FontFamily** | Done | Pango `font_family` attribute |
| ✅ **Span.FontAttributes** | Done | Pango `weight="bold"` / `style="italic"` |
| ✅ **Span.TextDecorations** | Done | Pango `underline="single"` / `strikethrough="true"` |
| ✅ **Span.CharacterSpacing** | Done | Pango `letter_spacing` attribute |

> Note: GTK4 `Gtk.Label` supports Pango markup natively, making Span rendering straightforward to implement.

---

## 19. MenuBar (Desktop)

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **MenuBarItem** | Done | Top-level menu items via `Gtk.PopoverMenuBar` |
| ✅ **MenuFlyoutItem** | Done | Menu items with Text, Command via `Gio.SimpleAction` |
| ⚠️ **MenuFlyoutSeparator** | Partial | Basic separator support |
| ✅ **Dropdown behavior** | Done | Native GTK popover menu behavior |

> `MenuBar` rendered via `GtkMenuBarManager` — integrates with `Gtk.Window` action groups. NavigationPage propagates `Page.MenuBarItems` to the window on navigation.

---

## 20. Lifecycle Events

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **OnWindowCreated** | Done | `ConfigureLifecycleEvents().AddGtk()` hook |
| ✅ **OnMauiApplicationCreated** | Done | Access to `Gtk.Application` instance |
| ✅ **App icon** | Done | Hicolor icon theme integration via MauiIcon build targets |
| ✅ **Desktop entry** | Done | `.desktop` file generation via `GtkDesktopIntegration` |

---

## Summary Statistics

| Category | Implemented | Total | Coverage |
|----------|-------------|-------|----------|
| **Core Infrastructure** | 10 | 10 | 100% |
| **Pages** | 5 | 5 | 100% |
| **Layouts** | 9 | 9 | 100% |
| **Basic Controls** | 14 | 14 | 100% |
| **Input Controls** | 4 | 4 | 100% |
| **Collection Controls** | 4 (+ 3 partial) | 7 | 57-100% |
| **Navigation** | 4 | 4 | 100% |
| **Alerts & Dialogs** | 4 | 4 | 100% |
| **Gesture Recognizers** | 2 (+ 1 partial) | 5 | 50% |
| **Graphics** | 4 | 4 | 100% |
| **Shapes** | 7 | 7 | 100% |
| **Font Services** | 5 | 5 | 100% |
| **WebView** | 6 | 6 | 100% |
| **MenuBar** | 4 | 4 | 100% |
| **Essentials** | 18 (+ 2 partial) | 26 | 77% |
| **Base View Properties** | 16 | 22 | 73% |
| **FormattedText/Spans** | 9 | 9 | 100% |
| **VisualStateManager** | 0 | 6 | 0% |

### Key Gaps (Priority Order)
1. **PinchGestureRecognizer** — Pinch zoom not implemented
2. **VisualStateManager** — GTK handles most states natively, but MAUI VSM integration missing
3. **Shell GoToAsync** — Route-based navigation not implemented
4. **ListView / TableView** — Missing handlers (CollectionView serves as alternative)
