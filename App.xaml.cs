using System.Windows;
using MDEditor.Localization;
using MDEditor.Models;

namespace MDEditor;

public partial class App : Application
{
    public static AppSettings Settings { get; private set; } = null!;

    /// <summary>File path from command line (e.g. double-clicked .md file). Read by MainWindow.</summary>
    public static string? FileToOpen { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Capture command-line args BEFORE base.OnStartup creates the window via StartupUri
        FileToOpen = e.Args.Length > 0 ? e.Args[0] : null;

        base.OnStartup(e);

        Settings = AppSettings.Load();

        // Apply language
        LocalizationManager.Instance.CurrentLang = Settings.GetLanguage();

        // Theme will be applied by MainWindow based on settings
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Settings.Save();
        base.OnExit(e);
    }
}
