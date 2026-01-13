using System.Collections.ObjectModel;
using System.IO;
using PathCatcher.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace PathCatcher.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();
    private string pendingPath = string.Empty;

    public string Title => appVersionInfo.Title;

    public ObservableCollection<string> DirectoryPaths { get; set; } = new ();

    public string PendingPath { get => pendingPath; set => SetProperty(ref pendingPath, value); }

    public DelegateCommand AddDPathCommand => new DelegateCommand(() =>
    {
        if (string.IsNullOrWhiteSpace(PendingPath) || !Directory.Exists(PendingPath))
        {
            return;
        }

        DirectoryPaths.Add(PendingPath);
    });
}