using BinarySerializer.GBA;
using System.Collections.Generic;

namespace BinaryDataExplorer;

public class GBA_DataManager : GenericDataManager
{
    public override string DisplayName => "GameBoy";

    public override IEnumerable<IDataManager.FileType> GetFileTypes() => new IDataManager.FileType[]
    {
        new IDataManager.FileType("GBC ROM", typeof(GBC_ROMBase), "gbc_rom"),
        new IDataManager.FileType("GBA ROM", typeof(GBA_ROMBase), "gba_rom"),
    };
}