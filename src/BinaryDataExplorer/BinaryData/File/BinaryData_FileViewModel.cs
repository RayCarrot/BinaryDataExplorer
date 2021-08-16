using System.Collections.Generic;
using RayCarrot.UI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDataExplorer
{
    public class BinaryData_FileViewModel : BaseViewModel
    {
        public BinaryData_FileViewModel(BinaryData_File fileData)
        {
            FileData = fileData;
            DataItems = new FlattenedHierarchicalObservableCollection<BinaryData_BaseItemViewModel>();
            Files = new ObservableCollection<BinaryData_FileViewModel>();

            // Add dummy entry if it can be expanded so it can be expanded in the UI
            if (FileData.HasFiles)
                Files.Add(null);
        }

        public string Header => FileData.Header;
        public BinaryData_File FileData { get; }
        public FlattenedHierarchicalObservableCollection<BinaryData_BaseItemViewModel> DataItems { get; }
        public ObservableCollection<BinaryData_FileViewModel> Files { get; }

        private bool? _isSelected;
        public bool IsSelected
        {
            get => _isSelected ?? false;
            set
            {
                if (_isSelected == null)
                    OnFirstSelected();    

                _isSelected = value;
            }
        }

        private bool? _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded ?? false;
            set
            {
                if (_isExpanded == null)
                {
                    _isExpanded = value;
                    OnFirstExpanded();
                    return;
                }

                _isExpanded = value;
            }
        }

        protected async void OnFirstSelected() => await InitializeDataItemsAsync();
        protected async void OnFirstExpanded() => await InitializeFilesAsync();

        public async Task InitializeDataItemsAsync()
        {
            // Add data
            await Services.BinaryData.UseContextAsync(() =>
            {
                DataItems.Clear();
                var dataTable = Services.BinaryData.DataLookupTable;

                foreach (var dataItem in FileData.GetDataItems())
                {
                    var dataItemVM = new BinaryData_FlattenedHierarchialDataItemViewModel(DataItems, dataItem, this);
                    DataItems.AddData(dataItemVM);
                    setChildren(dataItemVM, dataItem.DataItems);

                    if (dataItem.Address != null)
                        dataTable[dataItem.Address] = dataItemVM;

                    void setChildren(FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel> vm, IEnumerable<BinaryData_BaseItemViewModel> dataItems)
                    {
                        foreach (var item in dataItems)
                        {
                            var childVM = vm.AddChild(item);
                            setChildren(childVM, item.DataItems);
                            if (item.Address != null)
                                dataTable[item.Address] = childVM;
                        }
                    }
                }
            }, returnIfLoading: false);

            // Initialize the data
            DataItems.Initialize();

            foreach (var item in DataItems.ToArray())
                item.IsExpanded = true;
        }
        public async Task InitializeFilesAsync()
        {
            await Services.BinaryData.UseContextAsync(async () =>
            {
                Files.Clear();

                await foreach (var childFile in FileData.GetFilesAsync())
                    Files.Add(new BinaryData_FileViewModel(childFile));
            }, returnIfLoading: false);
        }
    }
}