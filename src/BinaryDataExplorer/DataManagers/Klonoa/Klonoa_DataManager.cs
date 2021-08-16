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

                yield return new BinaryData_File($"{i} ({archiveFile.Item1?.GetType().Name}) - {archiveFile.Item2}", archiveFile.Item1)
                {
                    HasFiles = archiveFile.Item1 is ArchiveFile fileArchive && fileArchive.OffsetTable.FilesCount > 0,
                    GetFilesFunc = () => GetArchiveFilesAsync(archiveFile.Item1),
                    AutoRetrieveFileObjectDataItems = archiveFile.Item1 is { } and not ArchiveFile,
                };
            }
        }
    }
}