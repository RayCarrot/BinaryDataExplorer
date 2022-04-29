using BinarySerializer;

namespace BinaryDataExplorer
{
    public class BinaryData_BitSerializer : BitSerializerObject
    {
        public BinaryData_BitSerializer(BinaryData_Serializer serializerObject, Pointer valueOffset, int valueSize) : base(serializerObject, valueOffset, default, default)
        {
            ValueSize = valueSize;
        }

        public new BinaryData_Serializer SerializerObject => (BinaryData_Serializer)base.SerializerObject;
        public int ValueSize { get; }

        public override T SerializeBits<T>(T value, int length, SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned, string name = null)
        {
            SerializerObject.AddDataItem(new BinaryData_BitValueItemViewModel(SerializerObject.CurrentDataItem, ValueOffset, Position, length, ValueSize, name, value, typeof(T)));

            Position += length;

            return value;
        }
    }
}