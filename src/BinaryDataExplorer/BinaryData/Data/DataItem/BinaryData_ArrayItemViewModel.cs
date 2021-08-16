using System;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_ArrayItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_ArrayItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, Type type, long count, string name, string value) 
            : base(parent: parent, address: address, type: $"{type.Name}[{count}]", typeInfo: $"{type.FullName}[{count}]", name: name, value: value)
        { }
    }
}