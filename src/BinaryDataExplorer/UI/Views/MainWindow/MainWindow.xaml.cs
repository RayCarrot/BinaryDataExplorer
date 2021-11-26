using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace BinaryDataExplorer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BaseWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Services.App;
        Services.App.UserData.UI_WindowState?.ApplyToWindow(this);
    }

    private async void MainWindow_OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(1);
        await Services.App.LoadProfileAsync();
    }

    private async void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        if (Services.App.IsUnloaded)
            return;

        // Update the saved window state
        Services.App.UserData.UI_WindowState = UserData_WindowSessionState.GetWindowState(this);

        e.Cancel = true;
        await Services.App.UnloadAsync();
        await Task.Delay(1);
        Close();
    }
}