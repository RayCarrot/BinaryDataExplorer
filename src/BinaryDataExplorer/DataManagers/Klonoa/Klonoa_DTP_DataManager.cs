﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;

namespace BinaryDataExplorer;

public class Klonoa_DTP_DataManager : Klonoa_DataManager
{
    public override string DisplayName => "Klonoa: Door to Phantomile";

    public override IEnumerable<IDataManager.DefaultFile> GetDefaultFiles(object mode)
    {
        var config = (KlonoaSettings_DTP_PS1)mode;

        return new IDataManager.DefaultFile[]
        {
            new IDataManager.DefaultFile(config.FilePath_BIN),
            new IDataManager.DefaultFile(config.FilePath_IDX, config.Address_IDX),
            new IDataManager.DefaultFile(config.FilePath_EXE, config.Address_EXE, MemoryMappedPriority: 0), // Give lower prio to prioritize IDX
        };
    }

    public override IEnumerable<IDataManager.DataManagerMode> GetModes() => new IDataManager.DataManagerMode[]
    {
        new IDataManager.DataManagerMode("US", new KlonoaSettings_DTP_US(), "us"),
        new IDataManager.DataManagerMode("Prototype (1997/07/17)", new KlonoaSettings_DTP_Prototype_19970717(), "proto_19970717"),
    };

    public override IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files)
    {
        // Get the settings
        KlonoaSettings_DTP_PS1 settings = (KlonoaSettings_DTP_PS1)mode;

        // Add the settings to the context
        context.AddKlonoaSettings(settings);

        // Load the IDX
        IDX idxData = FileFactory.Read<IDX>(context, settings.FilePath_IDX);

        // Create the loader
        var loader = Loader.Create(context, idxData);

        // Add the BIN file
        return new BinaryData_FileViewModel[]
        {
            new BinaryData_FileViewModel(new BinaryData_File($"BIN", null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetBINBlocksAsync(loader, idxData),
            }),
        }.ToAsyncEnumerable();
    }

    public async IAsyncEnumerable<BinaryData_File> GetBINBlocksAsync(Loader loader, IDX idxData)
    {
        await Task.CompletedTask;

        // Enumerate every block
        for (int blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
        {
            var index = blockIndex;

            yield return new BinaryData_File($"{blockIndex}", null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetBINBlockFilesAsync(loader, idxData, index)
            };
        }
    }

    public async IAsyncEnumerable<BinaryData_File> GetBINBlockFilesAsync(Loader loader, IDX idxData, int blockIndex)
    {
        // Switch to the BIN block
        loader.SwitchBlocks(blockIndex);

        // Add every BIN block file
        for (int fileIndex = 0; fileIndex < idxData.Entries[blockIndex].LoadCommands.Length; fileIndex++)
        {
            // Get the load command
            IDXLoadCommand loadCmd = idxData.Entries[blockIndex].LoadCommands[fileIndex];

            // Ignore any command which does not load a file
            if (loadCmd.Type != 2)
                continue;

            // Load the BIN block file
            BaseFile fileData = await Task.Run(() => loader.LoadBINFile(fileIndex));

            // Process code files
            if (loadCmd.FILE_Type == IDXLoadCommand.FileType.Code)
                loader.ProcessBINFile(fileIndex);

            yield return new BinaryData_File($"{fileIndex} ({loadCmd.FILE_Type})", fileData)
            {
                HasFiles = fileData is ArchiveFile archive && archive.OffsetTable.FilesCount > 0,
                GetFilesFunc = () => GetArchiveFilesAsync(fileData),
                AutoRetrieveFileObjectDataItems = fileData is { } and not ArchiveFile,
                GetAdditionalDataItemsFunc = () => loadCmd.GetBinaryDataItems("LoadCommand").Yield(),
            };
        }

        if (loader.BINBlock >= loader.Settings.BLOCK_FirstLevel)
        {
            // Load the level code data
            loader.ProcessLevelData();

            yield return new BinaryData_File($"LevelData_2D", loader.LevelData2D);
            yield return new BinaryData_File($"LevelData_3D", loader.LevelData3D);

            // Load the hard-coded objects data
            if (loader.LevelPack?.Sectors != null)
            {
                for (int sectorIndex = 0; sectorIndex < loader.LevelPack.Sectors.Length; sectorIndex++)
                {
                    loader.LevelSector = sectorIndex;
                    BaseHardCodedObjectsLoader objLoader = loader.Settings.GetHardCodedObjectsLoader(loader);
                    objLoader.LoadObjects();
                }

                loader.LevelSector = -1;
            }
        }
    }
}