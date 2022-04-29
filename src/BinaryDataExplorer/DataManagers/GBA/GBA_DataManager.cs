using System.Collections.Generic;

namespace BinaryDataExplorer;

public class GBA_DataManager : GenericDataManager
{
    public override string DisplayName => "GameBoy";

    public override IEnumerable<IDataManager.FileType> GetFileTypes() => new IDataManager.FileType[]
    {
        new IDataManager.FileType("GB/GBC ROM", typeof(BinarySerializer.Nintendo.GB.ROMBase), "gb_rom"),
        new IDataManager.FileType("GBA ROM", typeof(BinarySerializer.Nintendo.GBA.ROMBase), "gba_rom"),
    };
}