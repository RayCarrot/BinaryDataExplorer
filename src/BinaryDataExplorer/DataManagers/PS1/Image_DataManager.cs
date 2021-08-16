using System.Collections.Generic;
using BinarySerializer.Image;

namespace BinaryDataExplorer
{
    public class Image_DataManager : GenericDataManager
    {
        public override string DisplayName => "Image";

        public override IEnumerable<IDataManager.FileType> GetFileTypes() => new IDataManager.FileType[]
        {
            new IDataManager.FileType("DDS", typeof(DDS), "dds"),
            new IDataManager.FileType("FLIC", typeof(FLIC), "flic"),
            new IDataManager.FileType("PCX", typeof(PCX), "pcx"),
            new IDataManager.FileType("TGA", typeof(TGA), "tga"),
        };
    }
}