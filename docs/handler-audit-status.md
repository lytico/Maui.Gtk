# Handler Property Wiring Audit Tracker

Last updated: 2026-02-13 (CollectionView compile fix applied)

## Purpose
Track MAUI handler property/command wiring parity work for `src/Maui.Linux/Handlers`, including visual controls and container/navigation handlers.

## Audit Snapshot
- Handlers reviewed: **34**
- Handlers with mapper/command parity gaps vs MAUI reference handlers: **18**
- Missing mapper keys identified: **113**
- Missing command keys identified: **0**

## Status Legend
- `Not started`: no implementation work started
- `In progress`: partial implementation or active PR
- `Done`: handler parity item completed and validated

## Priority Workstreams
### P0 - Core functionality gaps
- [ ] **CollectionViewHandler** — missing mapper keys: `26`, missing command keys: `0` _(Status: In progress)_
- [x] **WebViewHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Done)_
- [x] **ImageHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Done)_
- [x] **ImageButtonHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Done)_

### P1 - Input and text parity
- [ ] **EntryHandler** — missing mapper keys: `10`, missing command keys: `0` _(Status: Not started)_
- [ ] **EditorHandler** — missing mapper keys: `10`, missing command keys: `0` _(Status: Not started)_
- [ ] **SearchBarHandler** — missing mapper keys: `13`, missing command keys: `0` _(Status: Not started)_
- [ ] **PickerHandler** — missing mapper keys: `7`, missing command keys: `0` _(Status: Not started)_
- [ ] **DatePickerHandler** — missing mapper keys: `6`, missing command keys: `0` _(Status: In progress)_
- [ ] **TimePickerHandler** — missing mapper keys: `4`, missing command keys: `0` _(Status: In progress)_

### P2 - Styling and polish
- [ ] **ButtonHandler** — missing mapper keys: `6`, missing command keys: `0` _(Status: Not started)_
- [ ] **RadioButtonHandler** — missing mapper keys: `6`, missing command keys: `0` _(Status: Not started)_
- [ ] **SliderHandler** — missing mapper keys: `4`, missing command keys: `0` _(Status: Not started)_
- [ ] **SwitchHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Not started)_
- [ ] **ProgressBarHandler** — missing mapper keys: `1`, missing command keys: `0` _(Status: Not started)_
- [ ] **LabelHandler** — missing mapper keys: `3`, missing command keys: `0` _(Status: Not started)_
- [ ] **ActivityIndicatorHandler** — missing mapper keys: `1`, missing command keys: `0` _(Status: Not started)_
- [ ] **CheckBoxHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Not started)_
- [ ] **ScrollViewHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Not started)_
- [ ] **LayoutHandler** — missing mapper keys: `0`, missing command keys: `0` _(Status: Not started)_

### P3 - Advanced shape/border/window parity
- [ ] **BorderHandler** — missing mapper keys: `5`, missing command keys: `0` _(Status: Not started)_
- [ ] **ShapeViewHandler** — missing mapper keys: `5`, missing command keys: `0` _(Status: Not started)_
- [ ] **WindowHandler** — missing mapper keys: `4`, missing command keys: `0` _(Status: Not started)_

## Mapped-But-Empty Implementations (partial wiring)
- [ ] `CheckBoxHandler.MapForeground` _(Status: Not started)_
- [ ] `LayoutHandler.MapBackground` _(Status: Not started)_
- [ ] `ScrollViewHandler.MapHorizontalScrollBarVisibility` _(Status: Not started)_
- [ ] `ScrollViewHandler.MapVerticalScrollBarVisibility` _(Status: Not started)_
- [ ] `SwitchHandler.MapTrackColor` _(Status: Not started)_
- [ ] `SwitchHandler.MapThumbColor` _(Status: Not started)_

## Detailed Gaps by Handler (vs MAUI reference handlers)
### ActivityIndicatorHandler
- Reference handler: `Microsoft.Maui.Handlers.ActivityIndicatorHandler`
- Missing mapper keys (1):
  - `Color`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### BorderHandler
- Reference handler: `Microsoft.Maui.Handlers.BorderHandler`
- Missing mapper keys (5):
  - `StrokeDashOffset`, `StrokeDashPattern`, `StrokeLineCap`, `StrokeLineJoin`, `StrokeMiterLimit`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### ButtonHandler
- Reference handler: `Microsoft.Maui.Handlers.ButtonHandler`
- Missing mapper keys (6):
  - `CharacterSpacing`, `CornerRadius`, `Font`, `Source`, `StrokeColor`, `StrokeThickness`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### CollectionViewHandler
- Reference handler: `Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler`
- Missing mapper keys (26):
  - `BackgroundColor`, `BackgroundImageSource`, `CanReorderItems`, `Description`, `EmptyView`, `EmptyViewTemplate`, `ExcludedWithChildren`, `Footer`, `FooterTemplate`, `Header`, `HeaderTemplate`, `HeadingLevel`, `Hint`, `HorizontalScrollBarVisibility`, `IsGrouped`, `IsInAccessibleTree`, `IsVisible`, `ItemSizingStrategy`, `ItemsLayout`, `ItemsSource`, `ItemsUpdatingScrollMode`, `ItemTemplate`, `SelectedItem`, `SelectedItems`, `SelectionMode`, `VerticalScrollBarVisibility`
- Missing command keys (0):
  - _(none)_
- Status: `In progress` _(compile blocker fixed; build green, parity work continues)_

### DatePickerHandler
- Reference handler: `Microsoft.Maui.Handlers.DatePickerHandler`
- Missing mapper keys (6):
  - `CharacterSpacing`, `Font`, `IsOpen`, `MaximumDate`, `MinimumDate`, `TextColor`
- Missing command keys (0):
  - _(none)_
- Status: `In progress` _(click-to-edit popup now wired; remaining mapper parity work pending)_

### EditorHandler
- Reference handler: `Microsoft.Maui.Handlers.EditorHandler`
- Missing mapper keys (10):
  - `CharacterSpacing`, `CursorPosition`, `HorizontalTextAlignment`, `IsSpellCheckEnabled`, `IsTextPredictionEnabled`, `Keyboard`, `MaxLength`, `PlaceholderColor`, `SelectionLength`, `VerticalTextAlignment`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### EntryHandler
- Reference handler: `Microsoft.Maui.Handlers.EntryHandler`
- Missing mapper keys (10):
  - `CharacterSpacing`, `ClearButtonVisibility`, `CursorPosition`, `IsSpellCheckEnabled`, `IsTextPredictionEnabled`, `Keyboard`, `PlaceholderColor`, `ReturnType`, `SelectionLength`, `VerticalTextAlignment`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### GraphicsViewHandler
- Reference handler: `Microsoft.Maui.Handlers.GraphicsViewHandler`
- Missing mapper keys (1):
  - `Drawable`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### ImageButtonHandler
- Reference handler: `Microsoft.Maui.Handlers.ImageButtonHandler`
- Missing mapper keys (0):
  - _(none)_
- Missing command keys (0):
  - _(none)_
- Status: `Done`

### ImageHandler
- Reference handler: `Microsoft.Maui.Handlers.ImageHandler`
- Missing mapper keys (0):
  - _(none)_
- Missing command keys (0):
  - _(none)_
- Status: `Done`

### LabelHandler
- Reference handler: `Microsoft.Maui.Handlers.LabelHandler`
- Missing mapper keys (3):
  - `CharacterSpacing`, `LineHeight`, `VerticalTextAlignment`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### PageHandler
- Reference handler: `Microsoft.Maui.Handlers.PageHandler`
- Missing mapper keys (1):
  - `Title`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### PickerHandler
- Reference handler: `Microsoft.Maui.Handlers.PickerHandler`
- Missing mapper keys (7):
  - `CharacterSpacing`, `Font`, `HorizontalTextAlignment`, `IsOpen`, `TextColor`, `TitleColor`, `VerticalTextAlignment`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### ProgressBarHandler
- Reference handler: `Microsoft.Maui.Handlers.ProgressBarHandler`
- Missing mapper keys (1):
  - `ProgressColor`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### RadioButtonHandler
- Reference handler: `Microsoft.Maui.Handlers.RadioButtonHandler`
- Missing mapper keys (6):
  - `CharacterSpacing`, `CornerRadius`, `Font`, `StrokeColor`, `StrokeThickness`, `TextColor`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### SearchBarHandler
- Reference handler: `Microsoft.Maui.Handlers.SearchBarHandler`
- Missing mapper keys (13):
  - `CancelButtonColor`, `CharacterSpacing`, `Font`, `HorizontalTextAlignment`, `IsReadOnly`, `IsSpellCheckEnabled`, `IsTextPredictionEnabled`, `Keyboard`, `MaxLength`, `PlaceholderColor`, `ReturnType`, `SearchIconColor`, `VerticalTextAlignment`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### ShapeViewHandler
- Reference handler: `Microsoft.Maui.Handlers.ShapeViewHandler`
- Missing mapper keys (5):
  - `StrokeDashOffset`, `StrokeDashPattern`, `StrokeLineCap`, `StrokeLineJoin`, `StrokeMiterLimit`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### SliderHandler
- Reference handler: `Microsoft.Maui.Handlers.SliderHandler`
- Missing mapper keys (4):
  - `MaximumTrackColor`, `MinimumTrackColor`, `ThumbColor`, `ThumbImageSource`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

### TimePickerHandler
- Reference handler: `Microsoft.Maui.Handlers.TimePickerHandler`
- Missing mapper keys (4):
  - `CharacterSpacing`, `Font`, `IsOpen`, `TextColor`
- Missing command keys (0):
  - _(none)_
- Status: `In progress` _(click-to-edit popup now wired; remaining mapper parity work pending)_

### WebViewHandler
- Reference handler: `Microsoft.Maui.Handlers.WebViewHandler`
- Missing mapper keys (0):
  - _(none)_
- Missing command keys (0):
  - _(none)_
- Status: `Done`

### WindowHandler
- Reference handler: `Microsoft.Maui.Handlers.WindowHandler`
- Missing mapper keys (4):
  - `Height`, `Width`, `X`, `Y`
- Missing command keys (0):
  - _(none)_
- Status: `Not started`

## Handlers with No Mapper/Command Gaps (current parity baseline)
- `ApplicationHandler`
- `BoxViewHandler`
- `CheckBoxHandler`
- `ContentViewHandler`
- `FlyoutPageHandler`
- `FrameHandler`
- `LayoutHandler`
- `NavigationPageHandler`
- `ScrollViewHandler`
- `ShapeHandler`
- `StepperHandler`
- `SwitchHandler`
- `TabbedPageHandler`

## Notes
- Some MAUI properties are intentionally unsupported by GTK or current architecture; mark those as `Done` with rationale when triaged.
- Re-run audit script after significant handler updates and refresh this file.
