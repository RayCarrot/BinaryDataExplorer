using System.Windows;
using System.Windows.Input;

namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for LoadProfileWindow.xaml
    /// </summary>
    public partial class LoadProfileWindow : BaseWindow
    {
        public LoadProfileWindow()
        {
            InitializeComponent();
            Loaded += (_, _) => GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(ProfilesListBox, new ProfilesListDropTarget());
        }

        private void LoadProfile_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ProfileItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}