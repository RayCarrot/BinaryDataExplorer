namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : BaseWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // Save settings when closing
            Closed += (_, _) => Services.App.SaveAppUserData();
        }
    }
}