using System.Windows;
using System.Windows.Controls;

namespace MDEditor.Dialogs;

public partial class TableDialog : Window
{
    public string? TableMarkdown { get; private set; }

    public TableDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;

        // Live preview
        ColsBox.TextChanged += (s, e) => UpdatePreview();
        RowsBox.TextChanged += (s, e) => UpdatePreview();
        AlignLeft.Checked += (s, e) => UpdatePreview();
        AlignCenter.Checked += (s, e) => UpdatePreview();
        AlignRight.Checked += (s, e) => UpdatePreview();
        HasHeader.Checked += (s, e) => UpdatePreview();
        HasHeader.Unchecked += (s, e) => UpdatePreview();

        UpdatePreview();
        ColsBox.Focus();
        ColsBox.SelectAll();
    }

    private void UpdatePreview()
    {
        if (!int.TryParse(ColsBox.Text, out int cols) || cols < 1) cols = 3;
        if (!int.TryParse(RowsBox.Text, out int rows) || rows < 1) rows = 3;
        if (cols > 20) cols = 20;
        if (rows > 50) rows = 50;

        var result = GenerateTable(cols, rows);
        PreviewText.Text = result;
    }

    private string GenerateTable(int cols, int rows)
    {
        string align;
        if (AlignCenter.IsChecked == true)
            align = ":---:";
        else if (AlignRight.IsChecked == true)
            align = "---:";
        else
            align = "---";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine();

        // Header row
        if (HasHeader.IsChecked == true)
        {
            for (int c = 0; c < cols; c++)
            {
                sb.Append("| Colonna ");
                sb.Append(c + 1);
                sb.Append(' ');
            }
            sb.AppendLine("|");

            // Separator
            for (int c = 0; c < cols; c++)
            {
                sb.Append('|');
                sb.Append(align);
            }
            sb.AppendLine("|");
        }

        // Data rows
        int startRow = HasHeader.IsChecked == true ? 1 : 0;
        if (startRow == 0)
        {
            // separator first for no-header tables
            for (int c = 0; c < cols; c++)
            {
                sb.Append('|');
                sb.Append(align);
            }
            sb.AppendLine("|");
        }

        for (int r = startRow; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                sb.Append("| ");
                
                if (HasHeader.IsChecked == false && r == 0)
                    sb.Append($"Colonna {c + 1} ");
                else
                    sb.Append("  ");
            }
            sb.AppendLine("|");
        }

        sb.AppendLine();
        return sb.ToString();
    }

    private void Insert_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(ColsBox.Text, out int cols) || cols < 1) cols = 3;
        if (!int.TryParse(RowsBox.Text, out int rows) || rows < 1) rows = 3;
        if (cols > 20) cols = 20;
        if (rows > 50) rows = 50;

        TableMarkdown = GenerateTable(cols, rows);
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
