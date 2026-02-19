# TextEditor

A text editor with 2D transformations built in F# using Windows Forms and a custom lightweight control framework, based on the tutorial by Cisternino (Unipi).

Tutorial playlist: https://www.youtube.com/watch?v=iaKZSFkDZuI&list=PLH0ZF0pFNhGg5fa1g1V6yuoHgkWPCj80D

## Features

- **Interactive text editing** with keyboard input (alphanumeric, space, Enter, Delete)
- **Per-character transformations** - each character can be individually positioned, rotated, and scaled
- **Pan, zoom, and rotate** the entire canvas via on-screen controls or function keys
- **Font switching** between multiple fonts per character
- **Multi-selection** with Ctrl modifier for bulk transformations
- **Cursor animation** with blinking effect
- **Custom lightweight control framework** (LWControl/LWContainer) with hit testing and event routing

## Controls

| Input | Action |
|---|---|
| Alphanumeric keys | Type characters |
| Enter | New line |
| Delete | Remove selected character |
| F1-F8 | Movement and transformation shortcuts |
| Ctrl + click | Multi-select characters |
| ▲▼◄► buttons | Navigate / translate |
| +/- buttons | Zoom in/out |
| L/R buttons | Rotate |
| F button | Switch font |

## Architecture

The project is composed of three layers:

- **worldview.fsx** - World-to-View matrix transformations (translate, rotate, scale)
- **lvc.fsx** - Lightweight control framework with abstract `LWControl` and `LWContainer`
- **Tutorial.fsx** - Editor logic: `Carattere` (character objects), `Editor` (editing surface), `Controlli` (navigation panel), `ControlliEditor` (transformation controls)

## Tech stack

- F# on .NET Framework 4.6.1
- Windows Forms / GDI+ / System.Drawing.Drawing2D

## License

Apache License 2.0 - see [LICENSE](LICENSE) for details.
