# P1 — Text Input Handler Parity

Track mapper-key parity for `EntryHandler`, `EditorHandler`, and `SearchBarHandler`
against MAUI reference handlers.

## EntryHandler — 10 missing mapper keys

| Property | GTK Approach | Status |
|---|---|---|
| `CharacterSpacing` | CSS `letter-spacing` | ✅ Done |
| `ClearButtonVisibility` | `Gtk.Entry` secondary icon | ✅ Done |
| `CursorPosition` | `Gtk.Editable.SetPosition` | ✅ Done |
| `SelectionLength` | `Gtk.Editable.SelectRegion` | ✅ Done |
| `PlaceholderColor` | — (Entry inherits placeholder CSS from theme) | ⏭ Skipped |
| `ReturnType` | No-op (mobile IME concept) | ✅ Done |
| `VerticalTextAlignment` | Widget `valign` | ✅ Done |
| `IsSpellCheckEnabled` | No GTK Entry spell-check; no-op | ✅ Done |
| `IsTextPredictionEnabled` | No GTK Entry prediction; no-op | ✅ Done |
| `Keyboard` | `SetInputPurpose` | ✅ Done |

## EditorHandler — 10 missing mapper keys

| Property | GTK Approach | Status |
|---|---|---|
| `CharacterSpacing` | CSS `letter-spacing` | ✅ Done |
| `CursorPosition` | `TextBuffer.PlaceCursor` via iter | ✅ Done |
| `SelectionLength` | `TextBuffer.SelectRange` | ✅ Done |
| `HorizontalTextAlignment` | `SetJustification` | ✅ Done |
| `MaxLength` | Buffer change signal + clamp | ✅ Done |
| `PlaceholderColor` | Overlay label not yet implemented | ⏭ Skipped |
| `VerticalTextAlignment` | Widget `valign` | ✅ Done |
| `IsSpellCheckEnabled` | No built-in spell-check; no-op | ✅ Done |
| `IsTextPredictionEnabled` | No prediction; no-op | ✅ Done |
| `Keyboard` | `SetInputPurpose` | ✅ Done |

## SearchBarHandler — 13 missing mapper keys

| Property | GTK Approach | Status |
|---|---|---|
| `CharacterSpacing` | CSS `letter-spacing` | ✅ Done |
| `HorizontalTextAlignment` | CSS `text-align` | ✅ Done |
| `IsReadOnly` | `SetEditable(false)` | ✅ Done |
| `MaxLength` | `SetMaxWidthChars` (hint; no hard clamp) | ✅ Done |
| `PlaceholderColor` | CSS `placeholder` selector | ✅ Done |
| `CancelButtonColor` | CSS on last image child | ✅ Done |
| `SearchIconColor` | — (not exposed separately) | ⏭ Skipped |
| `ReturnType` | — (not in ISearchBar interface) | ⏭ Skipped |
| `VerticalTextAlignment` | Widget `valign` | ✅ Done |
| `IsSpellCheckEnabled` | No-op | ✅ Done |
| `IsTextPredictionEnabled` | No-op | ✅ Done |
| `Keyboard` | — (not in ISearchBar interface) | ⏭ Skipped |
| `Font` | Already mapped (audit false-positive) | ✅ Done |

## Notes
- GTK4 does not support spell-check or text-prediction on `Entry`/`SearchEntry`
  natively. Those mappers are intentional no-ops that prevent MAUI warnings.
- `ReturnType` is a mobile IME concept; mapped as no-op on desktop Linux.
- `Keyboard` maps to `Gtk.InputPurpose` for IME hints.
