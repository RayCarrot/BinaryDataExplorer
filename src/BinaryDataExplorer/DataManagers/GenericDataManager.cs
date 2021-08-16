using BinarySerializer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinaryDataExplorer
{
    public abstract class GenericDataManager : DataManager
    {
        public override IEnumerable<IDataManager.DataManagerMode> GetModes() => new IDataManager.DataManagerMode("Default", null, "default").Yield();

        public override async IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files)
        {
            await Task.CompletedTask;

            foreach (IDataManager.ProfileFile file in files)
            {
                var obj = (BinarySerializable)Activator.CreateInstance(file.Type);

                var s = context.Deserializer;
                s.Goto(context.GetFile(file.FilePath).StartPointer);
                obj.Init(s.CurrentPointer);
                obj.Serialize(s);

                yield return new BinaryData_FileViewModel(new BinaryData_File(file.FilePath, obj)
                {
                    HasFiles = false,
                    AutoRetrieveFileObjectDataItems = true,
                });
            }
        }
    }
}