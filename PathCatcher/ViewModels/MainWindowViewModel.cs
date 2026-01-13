using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using PathCatcher.Core;
using PathCatcher.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace PathCatcher.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();
    private readonly DirectoryWatchService watchService = new();
    private string pendingPath = string.Empty;

    public MainWindowViewModel()
    {
        watchService.FileCreated += OnFileCreated;
    }

    public string Title => appVersionInfo.Title;

    public ObservableCollection<string> DirectoryPaths { get; set; } = new ();

    public string PendingPath { get => pendingPath; set => SetProperty(ref pendingPath, value); }

    public DelegateCommand AddDPathCommand => new (() =>
    {
        if (string.IsNullOrWhiteSpace(PendingPath) || !Directory.Exists(PendingPath))
        {
            return;
        }

        DirectoryPaths.Add(PendingPath);
        watchService.StartWatch(PendingPath);
    });

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine($"File added: {e.FullPath}");
        var files = new StringCollection { e.FullPath, };
        Application.Current.Dispatcher.Invoke(() =>
        {
            Clipboard.SetFileDropList(files);
        });
    }
}