using BinarySerializer;

namespace BinaryDataExplorer;

public class BinaryData_LogItemViewModel : BinaryData_BaseItemViewModel
{
    public BinaryData_LogItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, string log) 
        : base(parent: parent, address: address, type: "LOG", typeInfo: "Serializer log", name: null, value: log)
    { }
}