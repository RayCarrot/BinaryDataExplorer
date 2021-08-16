using BinarySerializer;

namespace BinaryDataExplorer
{
    public static class BinarySerializableExtensions
    {
        public static BinaryData_BaseItemViewModel GetBinaryDataItems(
            this BinarySerializable obj, 
            string name)
        {
            // Create the serializer
            var s = new BinaryData_Serializer(obj.Context);

            // Go to the object
            s.Goto(obj.Offset);

            // Serialize and get the data
            return s.SerializeDataObject(obj, name);
        }
    }
}