using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BinaryDataExplorer
{
    /// <summary>
    /// Interaction logic for BinaryDataView.xaml
    /// </summary>
    public partial class BinaryDataView : UserControl
    {
        public BinaryDataView()
        {
            InitializeComponent();
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Services.BinaryData.SelectedBinaryDataFile = (BinaryData_FileViewModel)e.NewValue;
        }

        private void BinaryData_DataGridNameCell_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell = (DataGridCell)sender;
            var dataItem = (FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>)cell.DataContext;
            dataItem.IsExpanded = !dataItem.IsExpanded;
        }
    }
}