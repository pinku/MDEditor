using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MDEditor.Dialogs;
using MDEditor.Localization;
using MDEditor.Models;
using MDEditor.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace MDEditor;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private bool _isReversed;

    public MainWindow()
    {
        InitializeComponent();

        _vm = new MainViewModel();
        DataContext = _vm;

        _vm.ViewModeChanged += OnViewModeChanged;
        _vm.ViewTitleChanged += (_, t) => Title = t;

        InitializeWebView();
        InitializeFromSettings();
        SetupFormatShortcuts();

        // Set window icon from exe
        SetWindowIcon();

        // If launched via double-click on an .md file, open it
        var fileToOpen = App.FileToOpen;
        if (!string.IsNullOrEmpty(fileToOpen) && File.Exists(fileToOpen))
            _vm.LoadFile(fileToOpen);
    }

    private void InitializeFromSettings()
    {
        var settings = App.Settings;

        // Theme
        _vm.InitializeTheme(settings.IsDarkTheme);
        UpdateThemeButtonContent();

        // Language
        LocalizationManager.Instance.CurrentLang = settings.GetLanguage();
        LocalizeUI();
        UpdateLangButton();
        UpdateLangMenuChecks();
        LocalizationManager.Instance.PropertyChanged += (_, _) =>
        {
            LocalizeUI();
            UpdateLangButton();
            UpdateLangMenuChecks();
        };

        // Panel layout
        _isReversed = settings.IsReversed;
        if (_isReversed)
            ApplySwap();

        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ThemeButtonText))
                UpdateThemeButtonContent();
            if (e.PropertyName == nameof(MainViewModel.HtmlContent))
                UpdateWebViewContent();
        };
    }

    // ===========================
    //  MENU HANDLERS
    // ===========================

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void ThemeLight_Click(object sender, RoutedEventArgs e)
    {
        if (!_vm.IsLightTheme) _vm.ToggleThemeCommand.Execute(null);
    }

    private void ThemeDark_Click(object sender, RoutedEventArgs e)
    {
        if (!_vm.IsDarkTheme) _vm.ToggleThemeCommand.Execute(null);
    }

    private void LangIT_Click(object sender, RoutedEventArgs e)
    {
        SetLanguage(AppLanguage.IT);
    }

    private void LangEN_Click(object sender, RoutedEventArgs e)
    {
        SetLanguage(AppLanguage.EN);
    }

    private void LangToggle_Click(object sender, RoutedEventArgs e)
    {
        var newLang = LocalizationManager.Instance.CurrentLang == AppLanguage.IT
            ? AppLanguage.EN : AppLanguage.IT;
        SetLanguage(newLang);
    }

    private void SetLanguage(AppLanguage lang)
    {
        LocalizationManager.Instance.CurrentLang = lang;
        App.Settings.Language = lang.ToString();
        App.Settings.Save();
        LocalizeUI();

        // Refresh status and format tooltips
        _vm.StatusText = LocalizationManager.Instance["Status.Ready"];
    }

    private void UpdateLangButton()
    {
        LangButton.Text = LocalizationManager.Instance.CurrentLang == AppLanguage.IT
            ? "🇮🇹 IT" : "🇬🇧 EN";
    }

    private void UpdateLangMenuChecks()
    {
        MenuLangIT.IsChecked = LocalizationManager.Instance.CurrentLang == AppLanguage.IT;
        MenuLangEN.IsChecked = LocalizationManager.Instance.CurrentLang == AppLanguage.EN;
    }

    private void LocalizeUI()
    {
        var l = LocalizationManager.Instance;

        // Menu
        MenuFile.Header = l["Menu.File"];
        MenuNew.Header = l["Menu.New"];
        MenuOpen.Header = l["Menu.Open"];
        MenuSave.Header = l["Menu.Save"];
        MenuExit.Header = l["Menu.Exit"];
        MenuView.Header = l["Menu.View"];
        MenuTheme.Header = l["Menu.Theme"];
        MenuThemeLight.Header = l["Menu.ThemeLight"];
        MenuThemeDark.Header = l["Menu.ThemeDark"];
        MenuLanguage.Header = l["Menu.Language"];
        MenuSwap.Header = l["Menu.Swap"];
        MenuHelp.Header = l["Menu.Help"];
        MenuAbout.Header = l["Menu.About"];

        // Toolbar buttons
        BtnNew.ToolTip = l["Toolbar.New"];
        BtnOpen.ToolTip = l["Toolbar.Open"];
        BtnSave.ToolTip = l["Toolbar.Save"];
        BtnTheme.ToolTip = l["Toolbar.Theme"];
        BtnSwap.ToolTip = l["Toolbar.Swap"];
        BtnLang.ToolTip = l["Menu.Language"];

        // Toolbar labels
        LblNew.Text = l["Menu.New"];
        LblOpen.Text = l["Menu.Open"];
        LblSave.Text = l["Menu.Save"];
        LblView.Text = l["Toolbar.View"];
        LblSplit.Text = l["Toolbar.Split"];
        LblEditor.Text = l["Toolbar.Editor"];
        LblPreview.Text = l["Toolbar.Preview"];

        // Editor/Preview headers
        EditorHeaderText.Text = l["Editor.Header"];
        PreviewHeaderText.Text = l["Preview.Header"];

        // Format toolbar tooltips (in order of appearance)
        var formatKeys = new[]
        {
            "Format.Bold", "Format.Italic", "Format.Strike",
            "Format.H1", "Format.H2", "Format.H3",
            "Format.UL", "Format.OL", "Format.Check",
            "Format.Link", "Format.Image",
            "Format.Code", "Format.CodeBlock",
            "Format.Icon",
            "Format.Quote", "Format.Table", "Format.AddRow", "Format.AddCol", "Format.HR"
        };
        int ki = 0;
        foreach (var child in FormatToolbar.Children)
        {
            if (child is Button btn && ki < formatKeys.Length)
            {
                btn.ToolTip = l[formatKeys[ki++]];
            }
        }
    }

    private void SwapPanels_Click(object sender, RoutedEventArgs e)
    {
        _isReversed = !_isReversed;
        App.Settings.IsReversed = _isReversed;
        App.Settings.Save();
        ApplySwap();
    }

    private void ApplySwap()
    {
        var mainContentGrid = FindMainContentGrid();
        if (mainContentGrid == null) return;

        if (_isReversed)
        {
            // Editor column 0 → move to 2, Preview column 2 → move to 0
            Grid.SetColumn(EditorBorder, 2);
            Grid.SetColumn(PreviewBorder, 0);
            MainSplitter.Visibility = Visibility.Visible;
        }
        else
        {
            Grid.SetColumn(EditorBorder, 0);
            Grid.SetColumn(PreviewBorder, 2);
            MainSplitter.Visibility = Visibility.Visible;
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new AboutDialog(developerName: "Diego Pianarosa",
                                  website: "https://github.com/pinku/MDEditor");
        dlg.ShowDialog();
    }

    // ===========================
    //  FORMATTING SHORTCUTS
    // ===========================

    private void SetupFormatShortcuts()
    {
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatBold()), Key.B, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatItalic()), Key.I, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatStrike()), Key.D, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatUL()), Key.U, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatLink()), Key.K, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatCodeBlock()), Key.C, ModifierKeys.Control | ModifierKeys.Shift));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatImage()), Key.I, ModifierKeys.Control | ModifierKeys.Shift));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatH1()), Key.D1, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatH2()), Key.D2, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatH3()), Key.D3, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new SimpleCommand(() => FormatIconDialog()), Key.E, ModifierKeys.Control | ModifierKeys.Shift));
    }

    // ===========================
    //  FORMATTING CLICK HANDLERS
    // ===========================

    private void FormatBold_Click(object sender, RoutedEventArgs e) => FormatBold();
    private void FormatItalic_Click(object sender, RoutedEventArgs e) => FormatItalic();
    private void FormatStrike_Click(object sender, RoutedEventArgs e) => FormatStrike();
    private void FormatH1_Click(object sender, RoutedEventArgs e) => FormatH1();
    private void FormatH2_Click(object sender, RoutedEventArgs e) => FormatH2();
    private void FormatH3_Click(object sender, RoutedEventArgs e) => FormatH3();
    private void FormatUL_Click(object sender, RoutedEventArgs e) => FormatUL();
    private void FormatOL_Click(object sender, RoutedEventArgs e) => FormatOL();
    private void FormatCheck_Click(object sender, RoutedEventArgs e) => FormatCheck();
    private void FormatLink_Click(object sender, RoutedEventArgs e) => FormatLink();
    private void FormatImage_Click(object sender, RoutedEventArgs e) => FormatImage();
    private void FormatCode_Click(object sender, RoutedEventArgs e) => FormatCode();
    private void FormatCodeBlock_Click(object sender, RoutedEventArgs e) => FormatCodeBlock();
    private void FormatQuote_Click(object sender, RoutedEventArgs e) => FormatQuote();
    private void FormatTableDialog_Click(object sender, RoutedEventArgs e) => FormatTableDialog();
    private void FormatAddRow_Click(object sender, RoutedEventArgs e) => FormatAddRow();
    private void FormatAddCol_Click(object sender, RoutedEventArgs e) => FormatAddCol();
    private void FormatHR_Click(object sender, RoutedEventArgs e) => FormatHR();
    private void FormatIcon_Click(object sender, RoutedEventArgs e) => FormatIconDialog();

    // ===========================
    //  FORMATTING METHODS
    // ===========================

    private void FormatBold() => WrapSelection("**", "**", "testo");
    private void FormatItalic() => WrapSelection("*", "*", "testo");
    private void FormatStrike() => WrapSelection("~~", "~~", "testo");
    private void FormatH1() => PrefixLine("# ", "Titolo 1");
    private void FormatH2() => PrefixLine("## ", "Titolo 2");
    private void FormatH3() => PrefixLine("### ", "Titolo 3");
    private void FormatUL() => PrefixLine("- ", "Elemento");
    private void FormatOL() => PrefixLine("1. ", "Elemento");
    private void FormatCheck() => PrefixLine("- [ ] ", "Task");
    private void FormatCode() => WrapSelection("`", "`", "codice");

    private void FormatLink()
    {
        var selected = EditorTextBox.SelectedText;
        if (string.IsNullOrEmpty(selected))
        {
            var caret = EditorTextBox.CaretIndex;
            InsertAtCursor("[testo](url)");
            EditorTextBox.Select(caret + 1, 5);
        }
        else
        {
            var start = EditorTextBox.SelectionStart;
            ReplaceSelection($"[{selected}](url)");
            EditorTextBox.Select(start + selected.Length + 4, 3);
        }
    }

    private void FormatImage() => InsertAtCursor("![alt](url)");

    private void FormatCodeBlock()
    {
        var selected = EditorTextBox.SelectedText;
        if (string.IsNullOrEmpty(selected))
        {
            var caret = EditorTextBox.CaretIndex;
            InsertAtCursor("\n```\n\n```\n");
            EditorTextBox.CaretIndex = caret + 5;
        }
        else
        {
            ReplaceSelection($"\n```\n{selected}\n```\n");
        }
    }

    private void FormatQuote() => PrefixLine("> ", "Citazione");

    private void FormatHR() => InsertAtCursor("\n---\n");

    // ===========================
    //  TABLE METHODS
    // ===========================

    private void FormatTableDialog()
    {
        var dlg = new TableDialog { Owner = this };
        if (dlg.ShowDialog() == true && !string.IsNullOrEmpty(dlg.TableMarkdown))
            InsertAtCursor(dlg.TableMarkdown);
    }

    private void FormatAddRow()
    {
        var tableInfo = FindTableAtCursor();
        if (tableInfo == null)
        {
            MessageBox.Show(Loc("Dialog.NoTable"), Loc("Dialog.NoTableTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var (start, end, colCount, _) = tableInfo.Value;
        var sb = new StringBuilder();
        sb.Append('|');
        for (int c = 0; c < colCount; c++)
            sb.Append("  |");
        sb.Append('\n');

        var text = _vm.MarkdownText;
        var insertPos = end;
        if (insertPos < text.Length && text[insertPos] == '\n')
            insertPos++;

        _vm.MarkdownText = text[..insertPos] + sb.ToString() + (insertPos < text.Length ? text[insertPos..] : "");
        EditorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        EditorTextBox.CaretIndex = Math.Min(insertPos + sb.Length, EditorTextBox.Text.Length);
        EditorTextBox.Focus();
    }

    private void FormatAddCol()
    {
        var tableInfo = FindTableAtCursor();
        if (tableInfo == null)
        {
            MessageBox.Show(Loc("Dialog.NoTable"), Loc("Dialog.NoTableTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var (start, end, colCount, separatorIdx) = tableInfo.Value;
        var text = _vm.MarkdownText;
        var tableText = text[start..end];
        var lines = tableText.Split('\n');

        var sb = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.EndsWith('|'))
            {
                sb.Append(line);
                sb.Append(i == separatorIdx ? "---|" : "  |");
            }
            else
            {
                sb.Append(line);
                sb.Append(i == separatorIdx ? "| ---" : "| ");
            }
            sb.AppendLine();
        }

        _vm.MarkdownText = text[..start] + sb.ToString().TrimEnd() + (end < text.Length ? text[end..] : "");
        EditorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        EditorTextBox.CaretIndex = Math.Min(start + sb.Length, EditorTextBox.Text.Length);
        EditorTextBox.Focus();
    }

    private (int start, int end, int colCount, int separatorIdx)? FindTableAtCursor()
    {
        var text = EditorTextBox.Text;
        var pos = Math.Clamp(EditorTextBox.CaretIndex, 0, text.Length);

        var allLines = text.Split('\n');
        int charCount = 0;
        int tableStartLine = -1;
        int tableEndLine = -1;
        int separatorIdx = -1;
        int colCount = 0;

        for (int i = 0; i < allLines.Length; i++)
        {
            var lineStart = charCount;
            charCount += allLines[i].Length + 1;

            var trimmed = allLines[i].Trim();
            bool isTableLine = trimmed.StartsWith('|') && trimmed.EndsWith('|');

            if (isTableLine)
            {
                if (tableStartLine < 0) tableStartLine = i;
                tableEndLine = i;

                if (Regex.IsMatch(trimmed, @"^\|[\s\-:]+\|$"))
                    separatorIdx = i;

                if (colCount == 0 && !Regex.IsMatch(trimmed, @"^\|[\s\-:]+\|$"))
                    colCount = trimmed.Count(c => c == '|') - 1;
            }
            else if (tableStartLine >= 0)
            {
                if (pos >= GetLineStart(allLines, tableStartLine) && pos <= charCount)
                    break;

                tableStartLine = -1;
                tableEndLine = -1;
                separatorIdx = -1;
                colCount = 0;
            }
        }

        if (tableStartLine >= 0 && pos < GetLineStart(allLines, tableStartLine))
            return null;

        if (tableStartLine < 0 || colCount < 1)
            return null;

        var start = GetLineStart(allLines, tableStartLine);
        var end = GetLineStart(allLines, tableEndLine) + allLines[tableEndLine].Length;

        return (start, end, colCount, separatorIdx >= 0 ? separatorIdx - tableStartLine : -1);
    }

    private static int GetLineStart(string[] lines, int lineIdx)
    {
        int pos = 0;
        for (int i = 0; i < lineIdx && i < lines.Length; i++)
            pos += lines[i].Length + 1;
        return pos;
    }

    // ===========================
    //  ICON DIALOG
    // ===========================

    private void FormatIconDialog()
    {
        var dlg = new IconDialog { Owner = this };
        if (dlg.ShowDialog() == true && !string.IsNullOrEmpty(dlg.SelectedCode))
            InsertAtCursor(dlg.SelectedCode);
    }

    // ===========================
    //  HELPERS
    // ===========================

    private void WrapSelection(string prefix, string suffix, string placeholder)
    {
        EditorTextBox.Focus();
        var selected = EditorTextBox.SelectedText;
        if (string.IsNullOrEmpty(selected))
        {
            var caret = EditorTextBox.CaretIndex;
            InsertAtCursor(prefix + placeholder + suffix);
            EditorTextBox.Select(caret + prefix.Length, placeholder.Length);
        }
        else
        {
            var start = EditorTextBox.SelectionStart;
            ReplaceSelection(prefix + selected + suffix);
            EditorTextBox.Select(start + prefix.Length, selected.Length);
        }
    }

    private void PrefixLine(string prefix, string placeholder)
    {
        EditorTextBox.Focus();
        var text = _vm.MarkdownText;
        var caret = EditorTextBox.CaretIndex;

        var lineStart = text.LastIndexOf('\n', caret > 0 ? caret - 1 : 0) + 1;
        var lineEnd = text.IndexOf('\n', caret);
        if (lineEnd < 0) lineEnd = text.Length;

        var currentLine = text[lineStart..lineEnd];

        if (!string.IsNullOrWhiteSpace(currentLine))
        {
            _vm.MarkdownText = text[..lineStart] + prefix + currentLine + text[lineEnd..];
            EditorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EditorTextBox.CaretIndex = Math.Min(lineStart + prefix.Length + currentLine.Length, EditorTextBox.Text.Length);
        }
        else
        {
            InsertAtCursor(prefix + placeholder);
        }
    }

    private void InsertAtCursor(string text)
    {
        var caret = EditorTextBox.CaretIndex;
        var current = _vm.MarkdownText;
        _vm.MarkdownText = current[..caret] + text + current[caret..];
        EditorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        EditorTextBox.CaretIndex = Math.Min(caret + text.Length, EditorTextBox.Text.Length);
        EditorTextBox.Focus();
    }

    private void ReplaceSelection(string text)
    {
        var start = EditorTextBox.SelectionStart;
        var len = EditorTextBox.SelectionLength;
        var current = _vm.MarkdownText;
        _vm.MarkdownText = current[..start] + text + current[(start + len)..];
        EditorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        EditorTextBox.CaretIndex = Math.Min(start + text.Length, EditorTextBox.Text.Length);
        EditorTextBox.Focus();
    }

    // ===========================
    //  THEME & WEBVIEW
    // ===========================

    private void UpdateThemeButtonContent()
    {
        var sp = BtnTheme.Content as StackPanel;
        if (sp != null && sp.Children.Count >= 2)
        {
            var emoji = sp.Children[0] as TextBlock;
            var label = sp.Children[1] as TextBlock;
            if (emoji != null && label != null)
            {
                emoji.Text = _vm.IsDarkTheme ? "☀️" : "🌙";
                label.Text = _vm.IsDarkTheme ? "Light" : "Dark";
            }
        }
    }

    private async void InitializeWebView()
    {
        try
        {
            await PreviewWebView.EnsureCoreWebView2Async(null);
            PreviewWebView.CoreWebView2.Settings.IsScriptEnabled = false;
            UpdateWebViewContent();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{Loc("Dialog.WebViewError")}\n\n{ex.Message}\n\nhttps://go.microsoft.com/fwlink/p/?LinkId=2124703",
                Loc("Dialog.WebViewTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void UpdateWebViewContent()
    {
        if (PreviewWebView.CoreWebView2 != null)
            PreviewWebView.CoreWebView2.NavigateToString(_vm.HtmlContent);
    }

    private static string Loc(string key) => LocalizationManager.Instance[key];

    // ===========================
    //  VIEW MODE
    // ===========================

    private void OnViewModeChanged(object? sender, ViewMode mode)
    {
        var mainContentGrid = FindMainContentGrid();
        if (mainContentGrid == null) return;

        var editorCol = mainContentGrid.ColumnDefinitions[_isReversed ? 2 : 0];
        var splitterCol = mainContentGrid.ColumnDefinitions[1];
        var previewCol = mainContentGrid.ColumnDefinitions[_isReversed ? 0 : 2];

        switch (mode)
        {
            case ViewMode.Split:
                editorCol.Width = new GridLength(1, GridUnitType.Star);
                previewCol.Width = new GridLength(1, GridUnitType.Star);
                splitterCol.Width = new GridLength(4);
                EditorBorder.Visibility = Visibility.Visible;
                PreviewBorder.Visibility = Visibility.Visible;
                MainSplitter.Visibility = Visibility.Visible;
                break;

            case ViewMode.EditOnly:
                editorCol.Width = new GridLength(1, GridUnitType.Star);
                previewCol.Width = new GridLength(0);
                splitterCol.Width = new GridLength(0);
                EditorBorder.Visibility = Visibility.Visible;
                PreviewBorder.Visibility = Visibility.Collapsed;
                MainSplitter.Visibility = Visibility.Collapsed;
                break;

            case ViewMode.PreviewOnly:
                editorCol.Width = new GridLength(0);
                previewCol.Width = new GridLength(1, GridUnitType.Star);
                splitterCol.Width = new GridLength(0);
                EditorBorder.Visibility = Visibility.Collapsed;
                PreviewBorder.Visibility = Visibility.Visible;
                MainSplitter.Visibility = Visibility.Collapsed;
                break;
        }
    }

    private Grid? FindMainContentGrid()
    {
        if (Content is Grid rootGrid && rootGrid.Children.Count > 2)
        {
            if (rootGrid.Children[2] is Grid contentGrid)
                return contentGrid;
        }
        return null;
    }

    private void SetWindowIcon()
    {
        try
        {
            var exeDir = AppContext.BaseDirectory;
            var iconPath = System.IO.Path.Combine(exeDir, "Resources", "app.ico");
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(
                    new Uri(iconPath, UriKind.Absolute));
            }
        }
        catch
        {
            // Non-critical: app works without icon
        }
    }
}

internal class SimpleCommand : ICommand
{
    private readonly Action _execute;
    public SimpleCommand(Action execute) => _execute = execute;
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? p) => true;
    public void Execute(object? p) => _execute();
}
