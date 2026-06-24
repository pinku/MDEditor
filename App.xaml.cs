using System.Windows;
using MDEditor.Localization;
using MDEditor.Models;

namespace MDEditor;

public partial class App : Application
{
    public static AppSettings Settings { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
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
