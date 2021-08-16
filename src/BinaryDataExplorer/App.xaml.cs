using System.IO;
using System.Windows;
using System.Windows.Threading;
using ControlzEx.Theming;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Initialize the app
            Services.App.Initialize(e.Args);

            Services.App.UserData.PropertyChanged += UserData_OnPropertyChanged;

            // Update the theme
            UpdateTheme();
        }

        private void UserData_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(AppUserData.Theme_Dark):
                case nameof(AppUserData.Theme_Sync):
                    UpdateTheme();
                    break;

                case nameof(AppUserData.DataGrid_ColorMode):

                    if (!Services.App.BinaryData.IsInitialized)
                        return;

                    // Hacky force refresh of the data grid
                    var file = Services.App.BinaryData.SelectedBinaryDataFile;
                    Services.App.BinaryData.SelectedBinaryDataFile = null;
                    Services.App.BinaryData.SelectedBinaryDataFile = file;
                    break;
            }
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                File.WriteAllText($"crashlog.txt", e.Exception.ToString());
            }
            catch
            {
                // ignored
            }
        }

        public void UpdateTheme()
        {
            var data = Services.App.UserData;
            const string color = "Purple";

            if (data.Theme_Sync)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
            }
            else
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.DoNotSync;
                ThemeManager.Current.ChangeTheme(this, $"{(data.Theme_Dark ? "Dark" : "Light")}.{color}");
            }
        }

        public T OpenWindowDialog<T>(BaseWindow window, T viewModel)
            where T : BaseViewModel
        {
            window.DataContext = viewModel;
            window.ShowDialog();

            if (window.DialogResult != true)
                return null;
            else
                return viewModel;
        }
    }
}