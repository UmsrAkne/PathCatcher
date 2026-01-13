using PathCatcher.Utils;
using Prism.Mvvm;

namespace PathCatcher.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();

    public string Title => appVersionInfo.Title;
}