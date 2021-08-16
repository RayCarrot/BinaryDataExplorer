using System.Windows;

namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for LoadDataWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : BaseWindow
    {
        public EditProfileWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}