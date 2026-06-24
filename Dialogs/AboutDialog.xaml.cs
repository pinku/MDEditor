using System.Diagnostics;
using System.Windows;
using MDEditor.Localization;

namespace MDEditor.Dialogs;

public partial class AboutDialog : Window
{
    private string? _website;

    public AboutDialog(string developerName = "", string? website = null, string? email = null)
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
        _website = website;

        var loc = LocalizationManager.Instance;

        // Localize all text
        Title = loc["About.Title"];
        VersionText.Text = loc["About.Version"];
        DescriptionText.Text = loc["About.Description"];
        TechHeader.Text = loc["About.Technologies"];
        CreditsHeader.Text = loc["About.Credits"];
        CloseButton.Content = "OK";

        // Developer info
        DeveloperName.Text = string.IsNullOrEmpty(developerName) ? "---" : developerName;

        if (!string.IsNullOrEmpty(email))
            CreditsEmail.Text = email;
        else
            CreditsEmail.Visibility = Visibility.Collapsed;

        if (!string.IsNullOrEmpty(website))
        {
            CreditsLinkText.Text = website;
            CreditsLink.NavigateUri = new Uri(website);
        }
        else
        {
            CreditsLink.Visibility = Visibility.Collapsed;
        }
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_website))
        {
            Process.Start(new ProcessStartInfo(_website) { UseShellExecute = true });
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
