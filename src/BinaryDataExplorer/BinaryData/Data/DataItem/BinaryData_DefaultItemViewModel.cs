using System;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_DefaultItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_DefaultItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, Type type, string name, string value) 
            : base(parent: parent, address: address, type: type.Name, typeInfo: type.FullName, name: name, value: value) 
        { }
    }
}