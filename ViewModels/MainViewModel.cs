using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Markdig;
using MDEditor.Localization;
using MDEditor.Models;
using Microsoft.Win32;

namespace MDEditor.ViewModels;

public enum ViewMode { Split, EditOnly, PreviewOnly }

public partial class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _markdownText = "";
    private string _filePath = "";
    private string _statusText = "";
    private bool _isDarkTheme;
    private ViewMode _viewMode = ViewMode.Split;
    private string _htmlContent = "";

    private readonly MarkdownPipeline _pipeline;

    public MainViewModel()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        _statusText = Loc("Status.Ready");

        NewCommand = new RelayCommand(_ => NewFile());
        OpenCommand = new RelayCommand(_ => OpenFile());
        SaveCommand = new RelayCommand(_ => SaveFile());
        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
        SetViewCommand = new RelayCommand(p => SetView(p?.ToString() ?? "split"));

        // Default markdown
        MarkdownText = "# Benvenuto in MDEditor\n\n" +
                       "Un semplice editor **Markdown** con preview in tempo reale.\n\n" +
                       "## Caratteristiche\n\n" +
                       "- ✏️ Editor e preview affiancati\n" +
                       "- 🌙 Tema chiaro e scuro (`Ctrl+T`)\n" +
                       "- 📂 Apri e salva file `.md`\n" +
                       "- 🧮 Supporto per **Latex** ($E=mc^2$)\n" +
                       "- 📊 Tabelle, codice e diagrammi\n\n" +
                       "### Codice\n\n" +
                       "```csharp\n" +
                       "Console.WriteLine(\"Hello MDEditor!\");\n" +
                       "```\n\n" +
                       "### Tabella\n\n" +
                       "| Feature | Status |\n" +
                       "|---------|--------|\n" +
                       "| Markdown | ✅ |\n" +
                       "| Temi | ✅ |\n" +
                       "| Emoji | ✅ |\n";

        UpdatePreview();
    }

    public string MarkdownText
    {
        get => _markdownText;
        set
        {
            if (_markdownText != value)
            {
                _markdownText = value;
                OnPropertyChanged();
                UpdatePreview();
                IsModified = true;
            }
        }
    }

    public string FilePath
    {
        get => _filePath;
        set { _filePath = value; OnPropertyChanged(); }
    }

    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set { _isDarkTheme = value; OnPropertyChanged(); OnPropertyChanged(nameof(ThemeButtonText)); OnPropertyChanged(nameof(IsLightTheme)); }
    }

    public bool IsLightTheme
    {
        get => !_isDarkTheme;
        set { if (value) IsDarkTheme = false; }
    }

    public string ThemeButtonText => IsDarkTheme ? "☀️ Light" : "🌙 Dark";

    public string HtmlContent
    {
        get => _htmlContent;
        set { _htmlContent = value; OnPropertyChanged(); }
    }

    private bool _isModified;
    public bool IsModified
    {
        get => _isModified;
        set { _isModified = value; OnPropertyChanged(); UpdateTitle(); }
    }

    // View mode properties
    public bool IsSplitView
    {
        get => _viewMode == ViewMode.Split;
        set { if (value) SetView("split"); }
    }

    public bool IsEditOnly
    {
        get => _viewMode == ViewMode.EditOnly;
        set { if (value) SetView("edit"); }
    }

    public bool IsPreviewOnly
    {
        get => _viewMode == ViewMode.PreviewOnly;
        set { if (value) SetView("preview"); }
    }

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ToggleThemeCommand { get; }
    public ICommand SetViewCommand { get; }

    private void NewFile()
    {
        if (IsModified)
        {
            var result = MessageBox.Show(Loc("Dialog.SaveQuestion"), "MDEditor",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) SaveFile();
            else if (result == MessageBoxResult.Cancel) return;
        }

        MarkdownText = "";
        FilePath = "";
        IsModified = false;
        StatusText = Loc("Status.NewFile");
    }

    private void OpenFile()
    {
        if (IsModified)
        {
            var result = MessageBox.Show(Loc("Dialog.SaveQuestion"), "MDEditor",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) SaveFile();
            else if (result == MessageBoxResult.Cancel) return;
        }

        var dlg = new OpenFileDialog
        {
            Filter = Loc("File.Filter"),
            DefaultExt = ".md"
        };

        if (dlg.ShowDialog() == true)
        {
            try
            {
                MarkdownText = File.ReadAllText(dlg.FileName);
                FilePath = dlg.FileName;
                IsModified = false;
                StatusText = string.Format(Loc("Status.Opened"), Path.GetFileName(dlg.FileName));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Loc("Dialog.ErrorOpen")}: {ex.Message}", "MDEditor",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveFile()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            var dlg = new SaveFileDialog
            {
                Filter = Loc("File.Filter"),
                DefaultExt = ".md",
                FileName = "documento.md"
            };

            if (dlg.ShowDialog() != true) return;
            FilePath = dlg.FileName;
        }

        try
        {
            File.WriteAllText(FilePath, MarkdownText);
            IsModified = false;
            StatusText = string.Format(Loc("Status.Saved"), Path.GetFileName(FilePath));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Loc("Dialog.ErrorSave")}: {ex.Message}", "MDEditor",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static string Loc(string key) => LocalizationManager.Instance[key];

    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        var theme = IsDarkTheme ? "DarkTheme" : "LightTheme";
        var dict = new ResourceDictionary
        {
            Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative)
        };

        var appDict = Application.Current.Resources.MergedDictionaries;
        for (int i = appDict.Count - 1; i >= 0; i--)
        {
            var src = appDict[i].Source;
            if (src != null && src.ToString().Contains("Themes/"))
                appDict.RemoveAt(i);
        }
        appDict.Add(dict);

        App.Settings.IsDarkTheme = IsDarkTheme;
        App.Settings.Save();
        UpdatePreview();
    }

    private void SetView(string mode)
    {
        _viewMode = mode switch
        {
            "edit" => ViewMode.EditOnly,
            "preview" => ViewMode.PreviewOnly,
            _ => ViewMode.Split
        };

        OnPropertyChanged(nameof(IsSplitView));
        OnPropertyChanged(nameof(IsEditOnly));
        OnPropertyChanged(nameof(IsPreviewOnly));

        // Notify MainWindow to adjust column visibility
        ViewModeChanged?.Invoke(this, _viewMode);
    }

    public event EventHandler<ViewMode>? ViewModeChanged;

    private void UpdatePreview()
    {
        try
        {
            // Replace custom :fa[name] syntax with Font Awesome icons before markdown parsing
            // Must be done on source so markdown sees it as raw HTML
            var processed = FaRegex().Replace(MarkdownText, "<i class='fa-solid fa-$1'></i>");
            var htmlBody = Markdown.ToHtml(processed, _pipeline);
            var css = IsDarkTheme ? DarkCss : LightCss;
            var faLink = @"<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css'>";
            HtmlContent = $@"<!DOCTYPE html>
<html><head><meta charset='utf-8'>
{faLink}
<style>{css}</style></head>
<body>{htmlBody}</body></html>";
        }
        catch
        {
            HtmlContent = "<html><body><p style='color:red'>Errore nel rendering</p></body></html>";
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@":fa\[([a-zA-Z0-9\-_]+)\]")]
    private static partial System.Text.RegularExpressions.Regex FaRegex();

    private void UpdateTitle()
    {
        var title = "MDEditor";
        if (!string.IsNullOrEmpty(FilePath))
            title += $" - {Path.GetFileName(FilePath)}";
        if (IsModified) title += " *";
        ViewTitleChanged?.Invoke(this, title);
    }

    public event EventHandler<string>? ViewTitleChanged;

    public void InitializeTheme(bool dark)
    {
        IsDarkTheme = dark;
        var theme = dark ? "DarkTheme" : "LightTheme";
        var dict = new ResourceDictionary
        {
            Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative)
        };
        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
        UpdatePreview();
    }

    private const string LightCss = @"
* { box-sizing: border-box; margin: 0; padding: 0; }
body {
    font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
    font-size: 15px; line-height: 1.7; color: #1e1e1e;
    background-color: #ffffff; padding: 28px 32px; margin: 0;
    max-width: 100%; word-wrap: break-word;
}
h1 { font-size: 2em; font-weight: 600; border-bottom: 2px solid #e8e8e8; padding-bottom: 10px; margin: 0 0 16px 0; color: #1a1a1a; letter-spacing: -0.5px; }
h2 { font-size: 1.55em; font-weight: 600; border-bottom: 1px solid #eee; padding-bottom: 8px; margin: 24px 0 12px 0; color: #222; }
h3 { font-size: 1.25em; font-weight: 600; margin: 20px 0 10px 0; color: #333; }
h4 { font-size: 1.1em; font-weight: 600; margin: 16px 0 8px 0; color: #444; }
p { margin: 0 0 12px 0; }
a { color: #0078D4; text-decoration: none; }
a:hover { text-decoration: underline; }
code { background-color: #f0f0f0; padding: 2px 7px; border-radius: 4px; font-family: 'Cascadia Code', 'Consolas', monospace; font-size: 13px; color: #d63384; }
pre { background-color: #f6f8fa; padding: 18px; border-radius: 8px; overflow-x: auto; border: 1px solid #e4e4e4; margin: 12px 0; }
pre code { background: none; padding: 0; color: #1a1a1a; font-size: 13px; }
blockquote { border-left: 4px solid #0078D4; margin: 12px 0; padding: 8px 18px; color: #555; background: #f4f7fb; border-radius: 0 6px 6px 0; }
blockquote p { margin: 0; }
table { border-collapse: collapse; width: 100%; margin: 12px 0; font-size: 14px; }
th, td { border: 1px solid #ddd; padding: 10px 14px; text-align: left; }
th { background-color: #f5f5f5; font-weight: 600; color: #333; }
tr:nth-child(even) { background-color: #fafafa; }
img { max-width: 100%; border-radius: 6px; }
hr { border: none; border-top: 1px solid #e4e4e4; margin: 24px 0; }
ul, ol { padding-left: 26px; margin: 8px 0 12px 0; }
li { margin: 3px 0; }
input[type=checkbox] { margin-right: 6px; transform: scale(1.1); accent-color: #0078D4; }
";

    private const string DarkCss = @"
* { box-sizing: border-box; margin: 0; padding: 0; }
body {
    font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
    font-size: 15px; line-height: 1.7; color: #d4d4d4;
    background-color: #1e1e1e; padding: 28px 32px; margin: 0;
    max-width: 100%; word-wrap: break-word;
}
h1 { font-size: 2em; font-weight: 600; border-bottom: 2px solid #3a3a3a; padding-bottom: 10px; margin: 0 0 16px 0; color: #e8e8e8; letter-spacing: -0.5px; }
h2 { font-size: 1.55em; font-weight: 600; border-bottom: 1px solid #3a3a3a; padding-bottom: 8px; margin: 24px 0 12px 0; color: #ddd; }
h3 { font-size: 1.25em; font-weight: 600; margin: 20px 0 10px 0; color: #ccc; }
h4 { font-size: 1.1em; font-weight: 600; margin: 16px 0 8px 0; color: #bbb; }
p { margin: 0 0 12px 0; }
a { color: #60CDFF; text-decoration: none; }
a:hover { text-decoration: underline; }
code { background-color: #2d2d2d; padding: 2px 7px; border-radius: 4px; font-family: 'Cascadia Code', 'Consolas', monospace; font-size: 13px; color: #f48771; }
pre { background-color: #2a2a2a; padding: 18px; border-radius: 8px; overflow-x: auto; border: 1px solid #3e3e3e; margin: 12px 0; }
pre code { background: none; padding: 0; color: #d4d4d4; font-size: 13px; }
blockquote { border-left: 4px solid #60CDFF; margin: 12px 0; padding: 8px 18px; color: #aaa; background: #282828; border-radius: 0 6px 6px 0; }
blockquote p { margin: 0; }
table { border-collapse: collapse; width: 100%; margin: 12px 0; font-size: 14px; }
th, td { border: 1px solid #3e3e3e; padding: 10px 14px; text-align: left; }
th { background-color: #333; font-weight: 600; color: #ddd; }
tr:nth-child(even) { background-color: #242424; }
img { max-width: 100%; border-radius: 6px; }
hr { border: none; border-top: 1px solid #3e3e3e; margin: 24px 0; }
ul, ol { padding-left: 26px; margin: 8px 0 12px 0; }
li { margin: 3px 0; }
input[type=checkbox] { margin-right: 6px; transform: scale(1.1); accent-color: #60CDFF; }
";
}
