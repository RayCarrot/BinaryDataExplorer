using System.Windows;

namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for GoToWindow.xaml
    /// </summary>
    public partial class GoToWindow : BaseWindow
    {
        public GoToWindow()
        {
            InitializeComponent();
        }

        private void NavigateButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}