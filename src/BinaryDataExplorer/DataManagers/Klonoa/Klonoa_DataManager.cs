using BinarySerializer;
using BinarySerializer.Klonoa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinaryDataExplorer
{
    public abstract class Klonoa_DataManager : DataManager
    {
        public async IAsyncEnumerable<BinaryData_File> GetArchiveFilesAsync(BinarySerializable fileData)
        {
            await Task.CompletedTask;

            if (fileData is not ArchiveFile archive)
                yield break;

            // Add every archive file
            for (var i = 0; i < archive.ParsedFiles.Length; i++)
            {
                var archiveFile = archive.ParsedFiles[i];

                yield return new BinaryData_File($"{i} ({archiveFile?.Obj.GetType().Name}) - {archiveFile?.Name}", archiveFile?.Obj)
                {
                    HasFiles = archiveFile?.Obj is ArchiveFile fileArchive && fileArchive.OffsetTable.FilesCount > 0,
                    GetFilesFunc = () => GetArchiveFilesAsync(archiveFile?.Obj),
                    AutoRetrieveFileObjectDataItems = archiveFile?.Obj is { } and not ArchiveFile,
                };
            }
        }
    }
}