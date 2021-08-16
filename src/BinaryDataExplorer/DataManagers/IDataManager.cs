using BinarySerializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryDataExplorer
{
    public interface IDataManager
    {
        public string DisplayName { get; }

        public IEnumerable<DefaultFile> GetDefaultFiles(object mode) => Enumerable.Empty<DefaultFile>();

        public IEnumerable<FileType> GetFileTypes();

        public IEnumerable<DataManagerMode> GetModes() => new DataManagerMode("Default", null, "default").Yield();

        public IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, ProfileFile[] files);

        public record DataManagerMode(string DisplayName, object Mode, string ID);
        public record DefaultFile(string FilePath, long Address = 0);
        public record FileType(string DisplayName, Type Type, string ID);
        public record ProfileFile(string FilePath, Type Type, string ID);
    }
}