namespace BinaryDataExplorer;

public class BinaryData_FlattenedHierarchialDataItemViewModel : FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>
{
    public BinaryData_FlattenedHierarchialDataItemViewModel(FlattenedHierarchicalObservableCollection<BinaryData_BaseItemViewModel> flattenedCollection, BinaryData_BaseItemViewModel data, BinaryData_FileViewModel file) : base(flattenedCollection, null, data)
    {
        File = file;
    }

    public BinaryData_FileViewModel File { get; }
}