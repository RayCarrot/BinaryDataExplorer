using System.Windows;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_ByteArrayItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_ByteArrayItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, long count, string name, byte[] value) 
            : base(parent: parent, address: address, type: $"{typeof(byte).GetFriendlyName()}[{count}]", typeInfo: $"{typeof(byte).FullName}[{count}]", name: name, value: value.ToHexString(align: 8, maxLines: 1))
        {
            ByteArray = value;
        }

        public byte[] ByteArray { get; }

        public override void CopyValue()
        {
            Clipboard.SetText(ByteArray.ToHexString(align: 16));
        }
    }
}