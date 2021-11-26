using System.Collections.Generic;
using BinarySerializer.PS1;

namespace BinaryDataExplorer;

public class PS1_DataManager : GenericDataManager
{
    public override string DisplayName => "PS1";

    public override IEnumerable<IDataManager.FileType> GetFileTypes() => new IDataManager.FileType[]
    {
        new IDataManager.FileType("TIM", typeof(PS1_TIM), "tim"),
        new IDataManager.FileType("TMD", typeof(PS1_TMD), "tmd"),
        new IDataManager.FileType("BGD", typeof(PS1_BGD), "bgd"),
        new IDataManager.FileType("CEL", typeof(PS1_CEL), "cel"),
    };
}