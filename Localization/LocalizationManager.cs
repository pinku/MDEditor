using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MDEditor.Localization;

public enum AppLanguage { IT, EN }

[DefaultMember("Item")]
public class LocalizationManager : INotifyPropertyChanged
{
    public static LocalizationManager Instance { get; } = new();

    private AppLanguage _currentLang = AppLanguage.IT;

    public AppLanguage CurrentLang
    {
        get => _currentLang;
        set { _currentLang = value; OnPropertyChanged(); OnPropertyChanged("Item[]"); OnPropertyChanged("Item"); }
    }

    public string CurrentLangName =>
        _currentLang switch { AppLanguage.IT => "Italiano", AppLanguage.EN => "English", _ => "Italiano" };

    public string this[string key] => GetString(key);

    public string GetString(string key) =>
        _currentLang switch
        {
            AppLanguage.EN => EnStrings.GetValueOrDefault(key, key),
            _ => ItStrings.GetValueOrDefault(key, key)
        };

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    // ========== ITALIANO ==========
    private static readonly Dictionary<string, string> ItStrings = new()
    {
        ["App.Title"] = "MDEditor",
        ["Menu.File"] = "File",
        ["Menu.New"] = "Nuovo",
        ["Menu.Open"] = "Apri",
        ["Menu.Save"] = "Salva",
        ["Menu.Exit"] = "Esci",
        ["Menu.View"] = "Visualizza",
        ["Menu.Theme"] = "Tema",
        ["Menu.ThemeLight"] = "Chiaro",
        ["Menu.ThemeDark"] = "Scuro",
        ["Menu.Language"] = "Lingua",
        ["Menu.LangIT"] = "Italiano",
        ["Menu.LangEN"] = "English",
        ["Menu.Swap"] = "Inverti pannelli",
        ["Menu.Help"] = "Aiuto",
        ["Menu.About"] = "Informazioni su MDEditor",

        ["Toolbar.New"] = "Nuovo documento (Ctrl+N)",
        ["Toolbar.Open"] = "Apri file (Ctrl+O)",
        ["Toolbar.Save"] = "Salva file (Ctrl+S)",
        ["Toolbar.Theme"] = "Cambia tema (Ctrl+T)",
        ["Toolbar.View"] = "Vista:",
        ["Toolbar.Split"] = "Split",
        ["Toolbar.Editor"] = "Editor",
        ["Toolbar.Preview"] = "Preview",
        ["Toolbar.Swap"] = "Inverti editor/preview",

        ["Format.Bold"] = "Grassetto (Ctrl+B)",
        ["Format.Italic"] = "Corsivo (Ctrl+I)",
        ["Format.Strike"] = "Barrato (Ctrl+D)",
        ["Format.H1"] = "Titolo 1 (Ctrl+1)",
        ["Format.H2"] = "Titolo 2 (Ctrl+2)",
        ["Format.H3"] = "Titolo 3 (Ctrl+3)",
        ["Format.UL"] = "Elenco puntato (Ctrl+U)",
        ["Format.OL"] = "Elenco numerato",
        ["Format.Check"] = "Checklist",
        ["Format.Link"] = "Link (Ctrl+K)",
        ["Format.Image"] = "Immagine (Ctrl+Shift+I)",
        ["Format.Code"] = "Codice inline",
        ["Format.CodeBlock"] = "Blocco codice (Ctrl+Shift+C)",
        ["Format.Quote"] = "Citazione",
        ["Format.Table"] = "Inserisci tabella...",
        ["Format.AddRow"] = "Aggiungi riga alla tabella",
        ["Format.AddCol"] = "Aggiungi colonna alla tabella",
        ["Format.HR"] = "Linea orizzontale",
        ["Format.Icon"] = "Icona / Emoji (Ctrl+Shift+E)",

        ["Editor.Header"] = "Markdown",
        ["Preview.Header"] = "Preview",

        ["Status.Ready"] = "Pronto",
        ["Status.NewFile"] = "Nuovo file",
        ["Status.Opened"] = "Aperto: {0}",
        ["Status.Saved"] = "Salvato: {0}",

        ["Dialog.SaveQuestion"] = "Vuoi salvare le modifiche?",
        ["Dialog.ErrorOpen"] = "Errore apertura file",
        ["Dialog.ErrorSave"] = "Errore salvataggio",
        ["Dialog.NoTable"] = "Posiziona il cursore all'interno di una tabella Markdown.",
        ["Dialog.NoTableTitle"] = "Nessuna tabella trovata",
        ["Dialog.WebViewError"] = "WebView2 non disponibile. Installa WebView2 Runtime.",
        ["Dialog.WebViewTitle"] = "Avviso",

        ["About.Title"] = "Informazioni su MDEditor",
        ["About.Version"] = "Versione 1.1.0",
        ["About.Description"] = "Editor e viewer Markdown con preview in tempo reale, temi chiaro/scuro, supporto tabelle, emoji e icone Font Awesome.",
        ["About.Technologies"] = "Tecnologie",
        ["About.Credits"] = "Realizzato da",

        ["File.Untitled"] = "senza titolo",
        ["File.Filter"] = "File Markdown (*.md)|*.md|Tutti i file (*.*)|*.*",
    };

    // ========== ENGLISH ==========
    private static readonly Dictionary<string, string> EnStrings = new()
    {
        ["App.Title"] = "MDEditor",
        ["Menu.File"] = "File",
        ["Menu.New"] = "New",
        ["Menu.Open"] = "Open",
        ["Menu.Save"] = "Save",
        ["Menu.Exit"] = "Exit",
        ["Menu.View"] = "View",
        ["Menu.Theme"] = "Theme",
        ["Menu.ThemeLight"] = "Light",
        ["Menu.ThemeDark"] = "Dark",
        ["Menu.Language"] = "Language",
        ["Menu.LangIT"] = "Italiano",
        ["Menu.LangEN"] = "English",
        ["Menu.Swap"] = "Swap panels",
        ["Menu.Help"] = "Help",
        ["Menu.About"] = "About MDEditor",

        ["Toolbar.New"] = "New document (Ctrl+N)",
        ["Toolbar.Open"] = "Open file (Ctrl+O)",
        ["Toolbar.Save"] = "Save file (Ctrl+S)",
        ["Toolbar.Theme"] = "Toggle theme (Ctrl+T)",
        ["Toolbar.View"] = "View:",
        ["Toolbar.Split"] = "Split",
        ["Toolbar.Editor"] = "Editor",
        ["Toolbar.Preview"] = "Preview",
        ["Toolbar.Swap"] = "Swap editor/preview",

        ["Format.Bold"] = "Bold (Ctrl+B)",
        ["Format.Italic"] = "Italic (Ctrl+I)",
        ["Format.Strike"] = "Strikethrough (Ctrl+D)",
        ["Format.H1"] = "Heading 1 (Ctrl+1)",
        ["Format.H2"] = "Heading 2 (Ctrl+2)",
        ["Format.H3"] = "Heading 3 (Ctrl+3)",
        ["Format.UL"] = "Bulleted list (Ctrl+U)",
        ["Format.OL"] = "Numbered list",
        ["Format.Check"] = "Checklist",
        ["Format.Link"] = "Link (Ctrl+K)",
        ["Format.Image"] = "Image (Ctrl+Shift+I)",
        ["Format.Code"] = "Inline code",
        ["Format.CodeBlock"] = "Code block (Ctrl+Shift+C)",
        ["Format.Quote"] = "Blockquote",
        ["Format.Table"] = "Insert table...",
        ["Format.AddRow"] = "Add row to table",
        ["Format.AddCol"] = "Add column to table",
        ["Format.HR"] = "Horizontal rule",
        ["Format.Icon"] = "Icon / Emoji (Ctrl+Shift+E)",

        ["Editor.Header"] = "Markdown",
        ["Preview.Header"] = "Preview",

        ["Status.Ready"] = "Ready",
        ["Status.NewFile"] = "New file",
        ["Status.Opened"] = "Opened: {0}",
        ["Status.Saved"] = "Saved: {0}",

        ["Dialog.SaveQuestion"] = "Do you want to save changes?",
        ["Dialog.ErrorOpen"] = "Error opening file",
        ["Dialog.ErrorSave"] = "Error saving file",
        ["Dialog.NoTable"] = "Place the cursor inside a Markdown table.",
        ["Dialog.NoTableTitle"] = "No table found",
        ["Dialog.WebViewError"] = "WebView2 not available. Please install WebView2 Runtime.",
        ["Dialog.WebViewTitle"] = "Warning",

        ["About.Title"] = "About MDEditor",
        ["About.Version"] = "Version 1.1.0",
        ["About.Description"] = "Markdown editor and viewer with live preview, light/dark themes, table support, emoji and Font Awesome icons.",
        ["About.Technologies"] = "Technologies",
        ["About.Credits"] = "Created by",

        ["File.Untitled"] = "untitled",
        ["File.Filter"] = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
    };
}
