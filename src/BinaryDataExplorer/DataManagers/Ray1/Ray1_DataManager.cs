using BinarySerializer.Ray1;
using System.Collections.Generic;
using BinarySerializer;

namespace BinaryDataExplorer
{
    public class Ray1_PC_DataManager : GenericDataManager
    {
        public override string DisplayName => "Rayman 1 (PC)";

        public override IEnumerable<IDataManager.DataManagerMode> GetModes() => new IDataManager.DataManagerMode[]
        {
            new IDataManager.DataManagerMode("RAYMAN (PC)", new Ray1Settings(Ray1EngineVersion.PC, World.Jungle, 1), "ray_pc"),
            new IDataManager.DataManagerMode("KIT (PC)", new Ray1Settings(Ray1EngineVersion.PC_Kit, World.Jungle, 1), "raykit_pc"),
        };

        public override IEnumerable<IDataManager.FileType> GetFileTypes() => new IDataManager.FileType[]
        {
            // TODO: Add save file once encoding is supported
            new IDataManager.FileType("Level", typeof(PC_LevFile), "lev"),
            new IDataManager.FileType("Allfix", typeof(PC_AllfixFile), "fix"),
            new IDataManager.FileType("World", typeof(PC_WorldFile), "wld"),
            new IDataManager.FileType("BigRay", typeof(PC_BigRayFile), "bigray"),
        };

        public override IAsyncEnumerable<BinaryData_FileViewModel> LoadAsync(Context context, object mode, IDataManager.ProfileFile[] files)
        {
            context.AddSettings((Ray1Settings)mode);

            return base.LoadAsync(context, mode, files);
        }
    }
}