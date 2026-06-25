using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MDEditor.Localization;

namespace MDEditor.Models;

public class AppSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MDEditor", "settings.json");

    public bool IsDarkTheme { get; set; }
    public string Language { get; set; } = "IT";
    public bool IsReversed { get; set; }

    /// <summary>Lista dei percorsi dei file aperti nell'ultima sessione.</summary>
    public List<string> LastOpenFiles { get; set; } = new();

    /// <summary>Percorso del file attivo nell'ultima sessione (vuoto se nessuno).</summary>
    public string? LastActiveFile { get; set; }

    public static AppSettings Load()
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { /* ignore, use defaults */ }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch { /* ignore */ }
    }

    public AppLanguage GetLanguage() =>
        Language == "EN" ? AppLanguage.EN : AppLanguage.IT;
}
