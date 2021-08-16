using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BinarySerializer;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    public abstract class BinaryData_BaseItemViewModel : BaseViewModel
    {
        protected BinaryData_BaseItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, string type, string typeInfo, string name, string value)
        {
            Parent = parent;
            Address = address;
            Type = type;
            TypeInfo = typeInfo;
            Name = name;
            Value = value;
            DataItems = new ObservableCollection<BinaryData_BaseItemViewModel>();

            CopyValueCommand = new RelayCommand(CopyValue);
        }

        public ICommand CopyValueCommand { get; set; }

        public Pointer Address { get; }
        public string Type { get; }
        public string TypeInfo { get; }
        public string Name { get; }
        public string Value { get; }

        public bool IsSelected { get; set; }

        public BinaryData_BaseItemViewModel Parent { get; }
        public ObservableCollection<BinaryData_BaseItemViewModel> DataItems { get; }

        public void AddDataItem(BinaryData_BaseItemViewModel dataItem) => DataItems.Add(dataItem);

        public virtual void CopyValue()
        {
            Clipboard.SetText(Value);
        }
    }
}