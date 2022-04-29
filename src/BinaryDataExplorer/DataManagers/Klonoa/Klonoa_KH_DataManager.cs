using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.KH;
using BinarySerializer.Nintendo.GBA;

namespace BinaryDataExplorer;

public class Klonoa_KH_DataManager : Klonoa_DataManager
{
    public override string DisplayName => "Klonoa Heroes";

    public virtual string ROMFileName => $"ROM.gba";

    public override IEnumerable<IDataManager.DefaultFile> GetDefaultFiles(object mode)
    {
        return new IDataManager.DefaultFile[]
        {
            new IDataManager.DefaultFile(ROMFileName, Constants.Address_ROM),
        };
    }

    public override IEnumerable<IDataManager.DataManagerMode> GetModes() => new IDataManager.DataManagerMode[]
    {
        new IDataManager.DataManagerMode("JP", new KlonoaSettings_KH(), "jp"),
    };

    public override IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files)
    {
        // Get the settings
        KlonoaSettings_KH settings = (KlonoaSettings_KH)mode;

        // Don't serialize any maps by default as they're very slow (due to the tiles)
        settings.SerializeMap = new KlonoaSettings_KH.MapID(-1, -1, -1);

        // Add the settings to the context
        context.AddKlonoaSettings(settings);

        // Add the pointers
        context.AddPreDefinedPointers(DefinedPointers.GBA_JP);

        // Load the ROM
        KlonoaHeroesROM rom = FileFactory.Read<KlonoaHeroesROM>(context, ROMFileName);

        // Add the ROM
        return new BinaryData_FileViewModel[]
        {
            new BinaryData_FileViewModel(new BinaryData_File($"ROM", rom.Header)
            {
                HasFiles = true,
                GetFilesFunc = () => GetROMArchivesAsync(rom),
            }),
        }.ToAsyncEnumerable();
    }

    public async IAsyncEnumerable<BinaryData_File> GetROMArchivesAsync(KlonoaHeroesROM rom)
    {
        await Task.CompletedTask;

        // Menu pack
        yield return new BinaryData_File(nameof(rom.MenuPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.MenuPack)
        };

        // Enemy animations pack
        yield return new BinaryData_File(nameof(rom.EnemyAnimationsPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.EnemyAnimationsPack)
        };

        // Gameplay pack
        yield return new BinaryData_File(nameof(rom.GameplayPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.GameplayPack)
        };

        // Items pack
        yield return new BinaryData_File(nameof(rom.ItemsPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.ItemsPack)
        };

        // UI pack
        yield return new BinaryData_File(nameof(rom.UIPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.UIPack)
        };

        // Story pack
        yield return new BinaryData_File(nameof(rom.StoryPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.StoryPack)
        };

        // Maps pack
        yield return new BinaryData_File(nameof(rom.MapsPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetMapFilesAsync(rom.MapsPack)
        };

        // World map pack
        yield return new BinaryData_File(nameof(rom.WorldMapPack), null)
        {
            HasFiles = true,
            GetFilesFunc = () => GetArchiveFilesAsync(rom.WorldMapPack)
        };

        // Enemy object definitions
        yield return BinaryData_File.FromObjectArray(nameof(rom.EnemyObjectDefinitions), rom.EnemyObjectDefinitions);
    }

    public async IAsyncEnumerable<BinaryData_File> GetMapFilesAsync(MapsPack_ArchiveFile maps)
    {
        await Task.CompletedTask;

        for (int i = 0; i < maps.Maps.Length; i++)
        {
            int index = i;
            OffsetTable.KH_KW_Entry entry = maps.OffsetTable.KH_KW_Entries[index];

            yield return new BinaryData_File($"{i} ({typeof(Map_File).GetFriendlyName()}) - Map {entry.MapID1}-{entry.MapID2}-{entry.MapID3}", null)
            {
                GetAdditionalDataItemsFunc = () => maps.GetMap(index).GetBinaryDataItems("Map").Yield()
            };
        }
    }
}