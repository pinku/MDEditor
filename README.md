# MDEditor

Editor e viewer **Markdown** con preview in tempo reale, temi chiaro/scuro, supporto tabelle, emoji e icone Font Awesome.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-%235835CC.svg?logo=windows)
![license](https://img.shields.io/badge/license-MIT-green)

## ✨ Caratteristiche

- 📝 **Editor + Preview** affiancati con splitter ridimensionabile
- 🌗 **Tema chiaro e scuro** (`Ctrl+T`) con salvataggio automatico
- 🌐 **Multi-lingua**: Italiano e English
- 📊 **Tabelle** avanzate: dialog di inserimento, aggiungi riga/colonna
- 😀 **Emoji e icone Font Awesome 6** con selettore integrato (`Ctrl+Shift+E`)
- ⌨️ **Shortcut da tastiera** per tutte le formattazioni
- 💾 **Persistenza impostazioni**: tema, lingua e layout salvati automaticamente
- ⇄ **Inverti pannelli** editor/preview
- 🧮 **Markdown avanzato** via Markdig: tabelle, codice, diagrammi, math, footnote

## Shortcut

| Shortcut | Azione |
|---|---|
| `Ctrl+N/O/S` | Nuovo / Apri / Salva |
| `Ctrl+T` | Toggle tema chiaro/scuro |
| `Ctrl+B` | **Grassetto** |
| `Ctrl+I` | *Corsivo* |
| `Ctrl+D` | ~~Barrato~~ |
| `Ctrl+1/2/3` | Titolo H1 / H2 / H3 |
| `Ctrl+U` | Elenco puntato |
| `Ctrl+K` | Link |
| `Ctrl+Shift+I` | Immagine |
| `Ctrl+Shift+C` | Blocco codice |
| `Ctrl+Shift+E` | Selettore Emoji / Icone |

## 🚀 Installazione

### Download diretto

Scarica l'ultima release da [GitHub Releases](https://github.com/pinku/MDEditor/releases/latest):

📥 [**MDEditor-v1.1.0-win-x64.zip**](https://github.com/pinku/MDEditor/releases/download/v1.1.0/MDEditor-v1.1.0-win-x64.zip) (~61 MB)

Estrai lo ZIP ed esegui `MDEditor.exe`. Richiede [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) (già incluso in Windows 11).

### Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) (incluso in Windows 11, installabile su Windows 10)

### Build & Run

```bash
# Clona il repository
git clone https://github.com/pinku/MDEditor.git
cd MDEditor

# Esegui
dotnet run

# Oppure compila in Release
dotnet build -c Release

# Pubblica come app standalone (Windows x64)
dotnet publish -c Release -r win-x64 -o ./publish --self-contained true
```

### npm

```bash
npm run build          # Compila Release
npm run publish:win    # Pubblica self-contained Windows x64
npm run run            # Avvia in debug
```

## 🛠 Tecnologie

| Tecnologia | Uso |
|---|---|
| [.NET 10](https://dotnet.microsoft.com/) | Runtime e SDK |
| [WPF](https://github.com/dotnet/wpf) | Interfaccia desktop |
| [Markdig](https://github.com/xoofx/markdig) | Parsing Markdown → HTML |
| [WebView2](https://learn.microsoft.com/microsoft-edge/webview2) | Preview HTML |
| [Font Awesome 6](https://fontawesome.com/) | Icone nella preview |

## 📁 Struttura progetto

```
MDEditor/
├── App.xaml / .cs              # Avvio app, risorse, settings
├── MainWindow.xaml / .cs       # Finestra principale
├── Dialogs/
│   ├── AboutDialog.xaml / .cs  # Finestra About
│   ├── IconDialog.xaml / .cs   # Selettore emoji/icone
│   └── TableDialog.xaml / .cs  # Dialog inserimento tabella
├── Localization/
│   └── LocalizationManager.cs  # Dizionari IT/EN
├── Models/
│   └── AppSettings.cs          # Persistenza impostazioni JSON
├── Themes/
│   ├── LightTheme.xaml         # Tema chiaro moderno
│   └── DarkTheme.xaml          # Tema scuro moderno
└── ViewModels/
    ├── MainViewModel.cs        # ViewModel principale
    └── RelayCommand.cs         # Implementazione ICommand
```

## 👤 Autore

**Diego Pianarosa** — [GitHub](https://github.com/pinku)

## 📄 Licenza

MIT
