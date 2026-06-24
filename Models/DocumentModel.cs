using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MDEditor.Models;

public class DocumentModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _filePath = "";
    private string _content = "";
    private bool _isModified;

    public string FilePath
    {
        get => _filePath;
        set
        {
            if (_filePath != value)
            {
                _filePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
                IsModified = true;
            }
        }
    }

    public bool IsModified
    {
        get => _isModified;
        set { _isModified = value; OnPropertyChanged(); OnPropertyChanged(nameof(Title)); }
    }

    public string DisplayName =>
        string.IsNullOrEmpty(FilePath) ? "Nuovo" : Path.GetFileName(FilePath);

    public string Title =>
        DisplayName + (IsModified ? " *" : "");

    public DocumentModel(string filePath = "", string content = "")
    {
        _filePath = filePath;
        _content = content;
        _isModified = false;
    }
}
