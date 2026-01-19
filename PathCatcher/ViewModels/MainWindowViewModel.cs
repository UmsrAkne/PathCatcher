using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using PathCatcher.Core;
using PathCatcher.Models;
using PathCatcher.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace PathCatcher.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly static string ConfigFilePath = Path.Combine(AppContext.BaseDirectory, "directory_paths.txt");

    private readonly AppVersionInfo appVersionInfo = new ();
    private readonly DirectoryWatchService watchService = new();
    private string pendingPath = string.Empty;
    private CopyHistory selectedCopyHistory;

    public MainWindowViewModel()
    {
        watchService.FileCreated += OnFileCreated;
        LoadPathsFromFile();
    }

    public string Title => appVersionInfo.Title;

    public ObservableCollection<string> DirectoryPaths { get; set; } = new ();

    public ObservableCollection<CopyHistory> Histories { get; set; } = new ();

    public CopyHistory SelectedCopyHistory
    {
        get => selectedCopyHistory;
        set => SetProperty(ref selectedCopyHistory, value);
    }

    public string PendingPath { get => pendingPath; set => SetProperty(ref pendingPath, value); }

    public DelegateCommand AddDPathCommand => new (() =>
    {
        AddPath(PendingPath);
        PendingPath = string.Empty;
    });

    public DelegateCommand<CopyHistory> CopyFromHistoryCommand => new ((history) =>
    {
        if (history == null)
        {
            return;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            var files = new StringCollection { history.FilePath, };
            Clipboard.SetFileDropList(files);
        });
    });

    public void SavePathsToFile()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath) !);
            File.WriteAllLines(ConfigFilePath, DirectoryPaths);
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Failed to save config: {ex}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Failed to save config (unauthorized): {ex}");
        }
    }

    private void AddPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return;
        }

        // 重複回避（大文字小文字を無視）
        if (DirectoryPaths.Any(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        DirectoryPaths.Add(path);
        watchService.StartWatch(path);
    }

    private void LoadPathsFromFile()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                // 設定ファイルがなければ作成
                File.WriteAllText(ConfigFilePath, string.Empty);
                return;
            }

            var lines = File.ReadAllLines(ConfigFilePath);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                AddPath(line);
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Failed to load config: {ex}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Failed to load config (unauthorized): {ex}");
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine($"File added: {e.FullPath}");
        var files = new StringCollection { e.FullPath, };
        Application.Current.Dispatcher.Invoke(() =>
        {
            Clipboard.SetFileDropList(files);
            var ch = new CopyHistory
            {
                FilePath = e.FullPath,
                ContentType = ContentTypeDetector.DetectContentType(e.FullPath),
            };

            Histories.Insert(0, ch);
        });
    }
}