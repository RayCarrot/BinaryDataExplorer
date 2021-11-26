using System.Windows.Input;
using BinarySerializer;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    public class BinaryData_PointerItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_PointerItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, string name, Pointer value) 
            : base(parent: parent, address: address, type: typeof(Pointer).GetFriendlyName(), typeInfo: typeof(Pointer).FullName, name: name, value: value?.ToString())
        {
            PointerValue = value;

            NavigateToPointerCommand = new RelayCommand(NavigateToPointer, PointerValue != null);
        }

        public Pointer PointerValue { get; }

        public ICommand NavigateToPointerCommand { get; }

        public void NavigateToPointer()
        {
            Services.BinaryData.NavigateTo(PointerValue);
        }
    }
}