using BinarySerializer;
using System.Collections.Generic;
using System.Linq;

namespace BinaryDataExplorer
{
    public abstract class DataManager : IDataManager
    {
        public abstract string DisplayName { get; }
        public virtual IEnumerable<IDataManager.DefaultFile> GetDefaultFiles(object mode) => Enumerable.Empty<IDataManager.DefaultFile>();
        public virtual IEnumerable<IDataManager.FileType> GetFileTypes() => Enumerable.Empty<IDataManager.FileType>();
        public abstract IEnumerable<IDataManager.DataManagerMode> GetModes();
        public abstract IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files);
    }
}