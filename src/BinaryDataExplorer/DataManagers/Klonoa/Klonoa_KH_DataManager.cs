using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.GBA;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.KH;

namespace BinaryDataExplorer
{
    public class Klonoa_KH_DataManager : Klonoa_DataManager
    {
        public override string DisplayName => "Klonoa Heroes";

        public virtual string ROMFileName => $"ROM.gba";

        public override IEnumerable<IDataManager.DefaultFile> GetDefaultFiles(object mode)
        {
            return new IDataManager.DefaultFile[]
            {
                new IDataManager.DefaultFile(ROMFileName, GBAConstants.Address_ROM),
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

            // Add the settings to the context
            context.AddKlonoaSettings(settings);

            // Add the pointers
            context.AddPreDefinedPointers(DefinedPointers.GBA_JP);

            // Load the ROM
            KlonoaHeroesROM rom = FileFactory.Read<KlonoaHeroesROM>(ROMFileName, context);

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

            yield return new BinaryData_File(nameof(rom.MenuPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.MenuPack)
            };

            yield return new BinaryData_File(nameof(rom.EnemyAnimationsPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.EnemyAnimationsPack)
            };

            yield return new BinaryData_File(nameof(rom.GameplayPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.GameplayPack)
            };

            yield return new BinaryData_File(nameof(rom.ItemsPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.ItemsPack)
            };

            yield return new BinaryData_File(nameof(rom.UIPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.UIPack)
            };

            yield return new BinaryData_File(nameof(rom.StoryPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.StoryPack)
            };

            yield return new BinaryData_File(nameof(rom.MapsPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.MapsPack)
            };

            yield return new BinaryData_File(nameof(rom.WorldMapPack), null)
            {
                HasFiles = true,
                GetFilesFunc = () => GetArchiveFilesAsync(rom.WorldMapPack)
            };
        }
    }
}