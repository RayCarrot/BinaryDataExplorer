using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace BinaryDataExplorer;

public class BinaryData_File
{
    public BinaryData_File(string header, BinarySerializable fileObject)
    {
        Header = header;
        FileObject = fileObject;
    }

    public string Header { get; }
    public BinarySerializable FileObject { get; }

    // Files
    public virtual bool HasFiles { get; init; }
    public Func<IAsyncEnumerable<BinaryData_File>> GetFilesFunc { get; init; }
    public virtual IAsyncEnumerable<BinaryData_File> GetFilesAsync() => GetFilesFunc?.Invoke() ?? AsyncEnumerable.Empty<BinaryData_File>();

    // Data items
    public bool AutoRetrieveFileObjectDataItems { get; init; } = true;
    public Func<IEnumerable<BinaryData_BaseItemViewModel>> GetAdditionalDataItemsFunc { get; init; }
    public virtual IEnumerable<BinaryData_BaseItemViewModel> GetDataItems()
    {
        if (GetAdditionalDataItemsFunc != null)
        {
            foreach (var items in GetAdditionalDataItemsFunc())
                yield return items;
        }

        if (FileObject != null && AutoRetrieveFileObjectDataItems)
            yield return FileObject.GetBinaryDataItems("FileData");
    }
}