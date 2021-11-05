using System;
using System.Runtime.InteropServices;
using System.Text;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_BitValueItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_BitValueItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, int offset, int count, int valueSize, string name, long value) 
            : base(parent: parent, address: address, type: $"Int{count}", typeInfo: GetBitMask(offset, count, valueSize), name: name, value: $"{value}") 
        { }

        public BinaryData_BitValueItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, int offset, int count, Type valueType, string name, long value) 
            : this(parent, address, offset, count, Marshal.SizeOf(valueType) * 8, name, value) 
        { }

        protected static string GetBitMask(int offset, int count, int valueSize)
        {
            var str = new StringBuilder();

            for (int i = 0; i < valueSize; i++)
            {
                if (i != 0 && i % 8 == 0)
                    str.Append(' ');

                var index = valueSize - i - 1;

                str.Append(index >= offset && index < offset + count ? "1" : "0");
            }

            return str.ToString();
        }
    }
}