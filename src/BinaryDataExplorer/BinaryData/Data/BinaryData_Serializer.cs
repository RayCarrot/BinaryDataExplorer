using BinarySerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryDataExplorer;

public class BinaryData_Serializer : SerializerObject, IDisposable
{
    #region Constructor

    public BinaryData_Serializer(Context context) : base(context)
    {
        FilePositions = new Dictionary<BinaryFile, long>();
    }

    #endregion

    #region Protected Properties

    protected Dictionary<BinaryFile, long> FilePositions { get; }
    public long? CurrentFilePosition
    {
        get => FilePositions.ContainsKey(CurrentFile) ? FilePositions[CurrentFile] : null;
        set => FilePositions[CurrentFile] = value ?? 0;
    }

    protected BinaryFile CurrentFile { get; set; }

    #endregion

    #region Public Properties

    /// <summary>
    /// The current length of the data being serialized
    /// </summary>
    public override long CurrentLength => 0;

    /// <summary>
    /// The current binary file being used by the serializer
    /// </summary>
    public override BinaryFile CurrentBinaryFile => CurrentFile;

    /// <summary>
    /// The current file offset
    /// </summary>
    public override long CurrentFileOffset => CurrentFilePosition ?? 0;

    #endregion

    #region Data

    public BinaryData_BaseItemViewModel CurrentDataItem { get; set; }

    public BinaryData_BaseItemViewModel AddDataItem(BinaryData_BaseItemViewModel dataItem)
    {
        CurrentDataItem.AddDataItem(dataItem);
        return dataItem;
    }

    protected void AscendDataItemsHierarchy(BinaryData_BaseItemViewModel item)
    {
        CurrentDataItem = item;
    }

    protected void DescendDataItemsHierarchy()
    {
        CurrentDataItem = CurrentDataItem.Parent;
    }

    public BinaryData_BaseItemViewModel SerializeDataObject(BinarySerializable obj, string name)
    {
        var dataItem = new BinaryData_DefaultItemViewModel(null, CurrentPointer, obj.GetType(), name, null);
        AscendDataItemsHierarchy(dataItem);
        obj.Serialize(this);
        DescendDataItemsHierarchy();
        return dataItem;
    }

    #endregion

    #region Logging

    public override void Log(string logString)
    {
        AddDataItem(new BinaryData_LogItemViewModel(CurrentDataItem, CurrentPointer, logString));
    }

    #endregion

    #region Encoding

    public override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, bool allowLocalPointers = false, string filename = null)
    {
        using (MemoryStream memStream = new MemoryStream(0))
        {
            // Stream key
            string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

            // Add the stream
            StreamFile sf = new StreamFile(
                context: Context,
                name: key,
                stream: memStream,
                endianness: endianness ?? CurrentFile.Endianness,
                allowLocalPointers: allowLocalPointers);

            try
            {
                Context.AddFile(sf);

                DoAt(sf.StartPointer, () =>
                {
                    action();
                    memStream.Position = 0;
                });
            }
            finally
            {
                Context.RemoveFile(sf);
            }
        }

        // TODO: Increment position
        //if (encoded != null)
        //    CurrentFilePosition += encoded.Length;
    }

    public override Pointer BeginEncoded(IStreamEncoder encoder, Endian? endianness = null, bool allowLocalPointers = false, string filename = null)
    {
        // Stream key
        string key = filename ?? $"{CurrentPointer}_{encoder.Name}";

        // Add the stream
        MemoryStream memStream = new MemoryStream();

        StreamFile sf = new StreamFile(
            context: Context,
            name: key,
            stream: memStream,
            endianness: endianness ?? CurrentFile.Endianness,
            allowLocalPointers: allowLocalPointers);

        Context.AddFile(sf);

        EncodedFiles.Add(new EncodedState()
        {
            File = sf,
            Stream = memStream,
            Encoder = encoder
        });

        return sf.StartPointer;
    }

    public override void EndEncoded(Pointer endPointer)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region XOR

    public override void BeginXOR(IXORCalculator xorCalculator) { }
    public override void EndXOR() { }
    public override IXORCalculator GetXOR() => null;

    #endregion

    #region Positioning

    public override void Goto(Pointer offset)
    {
        if (offset == null)
            return;

        if (offset.File != CurrentFile)
            SwitchToFile(offset.File);

        CurrentFilePosition = offset.FileOffset;
    }

    public override void DoAt(Pointer offset, Action action)
    {
        if (offset == null)
            return;

        Pointer off_current = CurrentPointer;
        Goto(offset);
        action();
        Goto(off_current);
    }

    public override T DoAt<T>(Pointer offset, Func<T> action)
    {
        if (offset == null)
            return default;

        Pointer off_current = CurrentPointer;
        Goto(offset);
        var result = action();
        Goto(off_current);
        return result;
    }

    #endregion

    #region Checksum

    public override T SerializeChecksum<T>(T calculatedChecksum, string name = null)
    {
        ReadType(calculatedChecksum);
        return calculatedChecksum;
    }

    /// <summary>
    /// Begins calculating byte checksum for all decrypted bytes read from the stream
    /// </summary>
    /// <param name="checksumCalculator">The checksum calculator to use</param>
    public override void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) { }

    /// <summary>
    /// Ends calculating the checksum and return the value
    /// </summary>
    /// <typeparam name="T">The type of checksum value</typeparam>
    /// <returns>The checksum value</returns>
    public override T EndCalculateChecksum<T>() => default;

    #endregion

    #region Serialization

    public override T Serialize<T>(T obj, string name = null)
    {
        AddDataItem(new BinaryData_DefaultItemViewModel(CurrentDataItem, CurrentPointer, typeof(T), name, $"{obj}"));
        ReadType(obj);
        return obj;
    }

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
    {
        BinaryData_BaseItemViewModel dataItem;
            
        if (obj is BaseColor c)
            dataItem = AddDataItem(new BinaryData_ColorItemViewModel(CurrentDataItem, CurrentPointer, name, c));
        else
            dataItem = AddDataItem(new BinaryData_DefaultItemViewModel(CurrentDataItem, CurrentPointer, typeof(T), name, obj.ShortLog));

        AscendDataItemsHierarchy(dataItem);

        Depth++;

        if (obj.Offset == null)
            obj.Init(CurrentPointer);

        onPreSerialize?.Invoke(obj);
        obj.Serialize(this);
        Depth--;

        DescendDataItemsHierarchy();
        return obj;
    }

    public override Pointer SerializePointer(Pointer obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, string name = null)
    {
        AddDataItem(new BinaryData_PointerItemViewModel(CurrentDataItem, CurrentPointer, name, obj));

        CurrentFilePosition += size switch
        {
            PointerSize.Pointer16 => 2,
            PointerSize.Pointer32 => 4,
            PointerSize.Pointer64 => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        return obj;
    }

    public override Pointer<T> SerializePointer<T>(Pointer<T> obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null)
    {
        AddDataItem(new BinaryData_PointerItemViewModel(CurrentDataItem, CurrentPointer, name, obj.PointerValue));

        Depth++;

        CurrentFilePosition += size switch
        {
            PointerSize.Pointer16 => 2,
            PointerSize.Pointer32 => 4,
            PointerSize.Pointer64 => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        if (obj != null && obj.PointerValue != null && resolve && obj.Value != null)
            DoAt(obj.PointerValue, () => SerializeObject<T>(obj.Value, onPreSerialize: onPreSerialize));

        Depth--;
        return obj;
    }

    public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null)
    {
        AddDataItem(new BinaryData_DefaultItemViewModel(CurrentDataItem, CurrentPointer, typeof(string), name, obj));

        if (length.HasValue)
            CurrentFilePosition += length;
        else
            CurrentFilePosition += (encoding ?? Context.DefaultEncoding).GetBytes(obj + '\0').Length;

        return obj;
    }

    #endregion

    #region Array Serialization

    public override T[] SerializeArraySize<T, U>(T[] obj, string name = null)
    {
        U Size = (U)Convert.ChangeType((obj?.Length) ?? 0, typeof(U));
        Serialize<U>(Size, name: $"{name}.Length");
        return obj;
    }

    public override T[] SerializeArray<T>(T[] obj, long count, string name = null)
    {
        if (obj is byte [] bytes)
        {
            AddDataItem(new BinaryData_ByteArrayItemViewModel(CurrentDataItem, CurrentPointer, count, name, bytes));
            CurrentFilePosition += bytes.Length;
            return obj;
        }

        var dataItem = AddDataItem(new BinaryData_ArrayItemViewModel(CurrentDataItem, CurrentPointer, typeof(T), count, name, null));
        AscendDataItemsHierarchy(dataItem);

        T[] buffer = GetArray(obj, count);

        for (int i = 0; i < count; i++)
            // Read the value
            Serialize<T>(buffer[i], name: $"{name}[{i}]");

        DescendDataItemsHierarchy();
        return buffer;
    }

    public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null)
    {
        var dataItem = AddDataItem(new BinaryData_ArrayItemViewModel(CurrentDataItem, CurrentPointer, typeof(T), count, name, null));
        AscendDataItemsHierarchy(dataItem);

        T[] buffer = GetArray(obj, count);

        for (int i = 0; i < count; i++)
            // Read the value
            SerializeObject<T>(buffer[i], onPreSerialize: onPreSerialize, name: $"{name}[{i}]");

        DescendDataItemsHierarchy();
        return buffer;
    }

    public override Pointer[] SerializePointerArray(Pointer[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, string name = null)
    {
        var dataItem = AddDataItem(new BinaryData_ArrayItemViewModel(CurrentDataItem, CurrentPointer, typeof(Pointer), count, name, null));
        AscendDataItemsHierarchy(dataItem);

        Pointer[] buffer = GetArray(obj, count);

        for (int i = 0; i < count; i++)
            // Read the value
            SerializePointer(buffer[i], anchor: anchor, allowInvalid: allowInvalid, name: $"{name}[{i}]");

        DescendDataItemsHierarchy();
        return buffer;
    }

    public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null)
    {
        var dataItem = AddDataItem(new BinaryData_ArrayItemViewModel(CurrentDataItem, CurrentPointer, typeof(Pointer<T>), count, name, null));
        AscendDataItemsHierarchy(dataItem);

        Pointer<T>[] buffer = GetArray(obj, count);

        for (int i = 0; i < count; i++)
            // Read the value
            SerializePointer<T>(buffer[i], anchor: anchor, resolve: resolve, onPreSerialize: onPreSerialize, allowInvalid: allowInvalid, name: $"{name}[{i}]");

        DescendDataItemsHierarchy();
        return buffer;
    }

    public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
    {
        var dataItem = AddDataItem(new BinaryData_ArrayItemViewModel(CurrentDataItem, CurrentPointer, typeof(string), count, name, null));
        AscendDataItemsHierarchy(dataItem);

        for (int i = 0; i < count; i++)
            // Read the value
            SerializeString(obj[i], length, encoding, name: $"{name}[{i}]");

        DescendDataItemsHierarchy();
        return obj;
    }

    #endregion

    #region Other Serialization

    public override void DoEndian(Endian endianness, Action action) => action();

    public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
    {
        int offset = 0;

        var items = new List<(long value, int length, string name, int offset)>();

        serializeFunc((v, length, name) =>
        {
            items.Add((v, length, name, offset));
            offset += length;
            return v;
        });

        var bytesCount = (int)Math.Ceiling(offset / 8f);

        foreach (var item in items)
            AddDataItem(new BinaryData_BitValueItemViewModel(CurrentDataItem, CurrentPointer, item.offset, item.length, bytesCount * 8, item.name, item.value, typeof(long)));

        CurrentFilePosition += bytesCount;
    }

    public override void DoBits<T>(Action<BitSerializerObject> serializeFunc)
    {
        serializeFunc(new BinaryData_BitSerializer(this, CurrentPointer, Marshal.SizeOf(typeof(T)) * 8));

        ReadType<T>(default);
    }

    #endregion

    #region Protected Helpers

    protected T[] GetArray<T>(T[] obj, long count)
    {
        // Create or resize array if necessary
        return obj ?? new T[(int)count];
    }

    protected void SwitchToFile(BinaryFile newFile)
    {
        if (newFile == null)
            return;

        if (!FilePositions.ContainsKey(newFile))
            FilePositions.Add(newFile, 0);

        CurrentFile = newFile;
    }

    protected void ReadType<T>(T value)
    {
        if (value is byte[] ba)
            CurrentFilePosition += ba.Length;
        else if (value is Array a)
            foreach (var item in a)
                ReadType(item);
        else if (value?.GetType().IsEnum == true)
            ReadType(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
        else if (value is bool)
            CurrentFilePosition += 1;
        else if (value is sbyte)
            CurrentFilePosition += 1;
        else if (value is byte)
            CurrentFilePosition += 1;
        else if (value is short)
            CurrentFilePosition += 2;
        else if (value is ushort)
            CurrentFilePosition += 2;
        else if (value is int)
            CurrentFilePosition += 4;
        else if (value is uint)
            CurrentFilePosition += 4;
        else if (value is long)
            CurrentFilePosition += 8;
        else if (value is ulong)
            CurrentFilePosition += 8;
        else if (value is float)
            CurrentFilePosition += 4;
        else if (value is double)
            CurrentFilePosition += 8;
        else if (value is string s)
            CurrentFilePosition += Context.DefaultEncoding.GetBytes(s + '\0').Length;
        else if (value is UInt24)
            CurrentFilePosition += 3;
        else if (Nullable.GetUnderlyingType(typeof(T)) != null)
        {
            // It's nullable
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType == typeof(byte))
                CurrentFilePosition += 1;
            else
                throw new NotSupportedException($"The specified type {typeof(T)} is not supported.");
        }
        else if (value is null)
            throw new ArgumentNullException(nameof(value));
        else
            throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported.");
    }

    #endregion

    #region Disposing

    public void Dispose()
    {
        FilePositions.Clear();
    }

    public void DisposeFile(BinaryFile file)
    {
        FilePositions.Remove(file);
    }

    #endregion
}