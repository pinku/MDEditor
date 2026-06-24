# MDEditor

A **Markdown** editor and viewer with live preview, light/dark themes, table support, emoji and Font Awesome icons.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-%235835CC.svg?logo=windows)
![license](https://img.shields.io/badge/license-MIT-green)
![release](https://img.shields.io/badge/release-v1.1.0-blue)
![windows](https://img.shields.io/badge/platform-Windows%2010%2B-0078D6?logo=windows)

## ✨ Features

- 📝 **Split editor + preview** with resizable splitter
- 🌗 **Light and dark theme** (`Ctrl+T`) with auto-save
- 🌐 **Multi-language**: English and Italian
- 📊 **Advanced tables**: insert dialog, add row/column
- 😀 **Emoji & Font Awesome 6** picker with search (`Ctrl+Shift+E`)
- ⌨️ **Keyboard shortcuts** for all formatting
- 💾 **Settings persistence**: theme, language and layout saved automatically
- ⇄ **Swap panels** editor/preview
- 🧮 **Extended Markdown** via Markdig: tables, code, diagrams, math, footnotes

## Shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+N/O/S` | New / Open / Save |
| `Ctrl+T` | Toggle light/dark theme |
| `Ctrl+B` | **Bold** |
| `Ctrl+I` | *Italic* |
| `Ctrl+D` | ~~Strikethrough~~ |
| `Ctrl+1/2/3` | Heading H1 / H2 / H3 |
| `Ctrl+U` | Bulleted list |
| `Ctrl+K` | Link |
| `Ctrl+Shift+I` | Image |
| `Ctrl+Shift+C` | Code block |
| `Ctrl+Shift+E` | Emoji / Icon picker |

## 🚀 Installation

### Direct download

Get the latest release from [GitHub Releases](https://github.com/pinku/MDEditor/releases/latest):

📥 [**MDEditor-v1.1.0-win-x64.zip**](https://github.com/pinku/MDEditor/releases/download/v1.1.0/MDEditor-v1.1.0-win-x64.zip) (~61 MB)

Extract the ZIP and run `MDEditor.exe`. Requires [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) (already included in Windows 11).

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) (included in Windows 11, installable on Windows 10)

### Build & Run

```bash
# Clone the repository
git clone https://github.com/pinku/MDEditor.git
cd MDEditor

# Run
dotnet run

# Build Release
dotnet build -c Release

# Publish as standalone app (Windows x64)
dotnet publish -c Release -r win-x64 -o ./publish --self-contained true
```

### npm

```bash
npm run build          # Build Release
npm run publish:win    # Publish self-contained Windows x64
npm run run            # Run in debug mode
```

## 🛠 Tech Stack

| Technology | Purpose |
|---|---|
| [.NET 10](https://dotnet.microsoft.com/) | Runtime & SDK |
| [WPF](https://github.com/dotnet/wpf) | Desktop UI framework |
| [Markdig](https://github.com/xoofx/markdig) | Markdown → HTML parsing |
| [WebView2](https://learn.microsoft.com/microsoft-edge/webview2) | HTML preview rendering |
| [Font Awesome 6](https://fontawesome.com/) | Icons in preview |

## 📁 Project Structure

```
MDEditor/
├── App.xaml / .cs              # App startup, resources, settings
├── MainWindow.xaml / .cs       # Main window
├── Dialogs/
│   ├── AboutDialog.xaml / .cs  # About window
│   ├── IconDialog.xaml / .cs   # Emoji/icon picker
│   └── TableDialog.xaml / .cs  # Table insert dialog
├── Localization/
│   └── LocalizationManager.cs  # EN/IT dictionaries
├── Models/
│   └── AppSettings.cs          # JSON settings persistence
├── Themes/
│   ├── LightTheme.xaml         # Modern light theme
│   └── DarkTheme.xaml          # Modern dark theme
└── ViewModels/
    ├── MainViewModel.cs        # Main ViewModel
    └── RelayCommand.cs         # ICommand implementation
```

## 👤 Author

**Diego Pianarosa** — [GitHub](https://github.com/pinku)

## 📄 License

MIT
