using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.LV;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinaryDataExplorer
{
    public class Klonoa_LV_DataManager : Klonoa_DataManager
    {
        public override string DisplayName => "Klonoa 2: Lunatea's Veil";

        public override IEnumerable<IDataManager.DefaultFile> GetDefaultFiles(object mode)
        {
            var config = (LoaderConfiguration_LV)mode;

            yield return new IDataManager.DefaultFile(config.FilePath_HEAD);

            for (int i = 0; i < 3; i++)
            {
                var bin = (Loader_LV.BINType)i;

                if (bin == Loader_LV.BINType.KL)
                {
                    for (int lang = 0; lang < config.LanguagesCount; lang++)
                        yield return new IDataManager.DefaultFile(config.GetFilePath(bin, languageIndex: lang));
                }
                else
                {
                    yield return new IDataManager.DefaultFile(config.GetFilePath(bin));
                }
            }
        }

        public override IEnumerable<IDataManager.DataManagerMode> GetModes() => new IDataManager.DataManagerMode[]
        {
            new IDataManager.DataManagerMode("US", new LoaderConfiguration_LV_US(), "us"),
            new IDataManager.DataManagerMode("EU", new LoaderConfiguration_LV_EU(), "eu"),
        };

        public override async IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files)
        {
            await Task.CompletedTask;

            // Get the config
            LoaderConfiguration_LV config = (LoaderConfiguration_LV)mode;

            // Load the IDX
            HeadPack_ArchiveFile headPack = FileFactory.Read<HeadPack_ArchiveFile>(config.FilePath_HEAD, context, (_, head) => head.Pre_HasMultipleLanguages = config.HasMultipleLanguages);

            // Create the loader
            var loader = Loader_LV.Create(context, headPack, config);

            for (int lang = 0; lang < config.LanguagesCount; lang++)
            {
                var languageIndex = lang;

                yield return new BinaryData_FileViewModel(new BinaryData_File(config.LanguagesCount > 1 ? $"KL{lang + 1}" : "KL", null)
                {
                    HasFiles = true,
                    GetFilesFunc = () => GetBINFilesAsync(loader, Loader_LV.BINType.KL, languageIndex: languageIndex),
                });
            }

            yield return new BinaryData_FileViewModel(new BinaryData_File($"BGM", null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetBINFiles_BGM_Async(loader),
            });

            yield return new BinaryData_FileViewModel(new BinaryData_File($"PPT", null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetBINFilesAsync(loader, Loader_LV.BINType.PPT),
            });
        }

        public async IAsyncEnumerable<BinaryData_File> GetBINFilesAsync(Loader_LV loader, Loader_LV.BINType bin, int languageIndex = 0)
        {
            // Add every BIN file
            for (int fileIndex = 0; fileIndex < loader.GetBINHeader(bin, languageIndex).FilesCount; fileIndex++)
            {
                // Load the BIN file
                BaseFile fileData;

                if (bin == Loader_LV.BINType.KL)
                    fileData = await Task.Run(() => loader.LoadBINFile<LevelPack_ArchiveFile>(bin, fileIndex));
                else
                    fileData = await Task.Run(() => loader.LoadBINFile<RawData_File>(bin, fileIndex));

                var fileObj = new BinaryData_File($"{fileIndex}", fileData)
                {
                    HasFiles = fileData is ArchiveFile archive && archive.OffsetTable.FilesCount > 0,
                    GetFilesFunc = () => GetArchiveFilesAsync(fileData),
                    AutoRetrieveFileObjectDataItems = fileData is { } and not ArchiveFile,
                };

                yield return fileObj;
            }
        }

        public async IAsyncEnumerable<BinaryData_File> GetBINFiles_BGM_Async(Loader_LV loader)
        {
            await Task.CompletedTask;

            var header = loader.GetBINHeader_BGM();

            // Add every BIN file
            for (int fileIndex = 0; fileIndex < header.FilesCount; fileIndex++)
            {
                var bgmFile = header.FileDescriptors[fileIndex];
                var index = fileIndex;

                var fileObj = new BinaryData_File($"{fileIndex}", null)
                {
                    HasFiles = bgmFile.FilesCount > 0,
                    GetFilesFunc = () => GetBGMFiles(loader, index),
                };

                yield return fileObj;
            }
        }

        public async IAsyncEnumerable<BinaryData_File> GetBGMFiles(Loader_LV loader, int fileIndex)
        {
            var bgmFile = loader.GetBINHeader_BGM().FileDescriptors[fileIndex];

            for (int bgmIndex = 0; bgmIndex < bgmFile.FilesCount; bgmIndex++)
            {
                // Load the BGM file
                BaseFile fileData = await Task.Run(() => loader.LoadBINFile<RawData_File>(Loader_LV.BINType.BGM, fileIndex, bgmIndex: bgmIndex));

                var fileObj = new BinaryData_File($"{bgmIndex}", fileData)
                {
                    HasFiles = fileData is ArchiveFile archive && archive.OffsetTable.FilesCount > 0,
                    GetFilesFunc = () => GetArchiveFilesAsync(fileData),
                    AutoRetrieveFileObjectDataItems = fileData is { } and not ArchiveFile,
                };

                yield return fileObj;
            }
        }
    }
}