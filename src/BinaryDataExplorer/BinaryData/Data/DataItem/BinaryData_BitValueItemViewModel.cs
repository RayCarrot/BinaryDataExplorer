using System;
using System.Text;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_BitValueItemViewModel : BinaryData_BaseItemViewModel
    {
        public BinaryData_BitValueItemViewModel(
            BinaryData_BaseItemViewModel parent, 
            Pointer address, 
            int position, 
            int length, 
            int valueSize, 
            string name, 
            object value,
            Type valueType) 
            : base(
                parent: parent, 
                address: address, 
                type: $"{position}_{length} {valueType.GetFriendlyName()}", 
                typeInfo: GetBitMask(position, length, valueSize), 
                name: name, 
                value: $"{value}") 
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