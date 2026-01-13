using System.ComponentModel;
using PathCatcher.ViewModels;

namespace PathCatcher.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (DataContext is MainWindowViewModel vm)
        {
            vm.SavePathsToFile();
        }
    }
}