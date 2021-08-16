using System.Windows;
using System.Windows.Controls;

namespace BinaryDataExplorer
{
    public class BinaryData_DataGridValueCellTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is not FrameworkElement element || item is not FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel> dataItem) 
                return null;

            return dataItem.Data switch
            {
                BinaryData_ColorItemViewModel => element.FindResource("BinaryData_DataGridCell.Value.BaseColor") as DataTemplate,
                BinaryData_PointerItemViewModel => element.FindResource("BinaryData_DataGridCell.Value.Pointer") as DataTemplate,
                _ => element.FindResource("BinaryData_DataGridCell.Value.Default") as DataTemplate,
            };
        }
    }
}