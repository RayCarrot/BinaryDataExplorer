using System;
using System.Runtime.InteropServices;
using System.Text;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_BitValueItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_BitValueItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, int offset, int count, Type valueType, string name, int value) 
            : base(parent: parent, address: address, type: $"Int{count}", typeInfo: GetBitMask(offset, count, valueType), name: name, value: $"{value}") 
        { }

        protected static string GetBitMask(int offset, int count, Type valueType)
        {
            var str = new StringBuilder();

            var size = Marshal.SizeOf(valueType) * 8;

            for (int i = 0; i < size; i++)
            {
                if (i != 0 && i % 8 == 0)
                    str.Append(' ');

                var index = size - i - 1;

                str.Append(index >= offset && index < offset + count ? "1" : "0");
            }

            return str.ToString();
        }
    }
}