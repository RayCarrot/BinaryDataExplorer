namespace BinaryDataExplorer;

public class UserData_DataProfile
{
    public string Name { get; set; }
    public string DataManager { get; set; }
    public string DataPath { get; set; }
    public string Mode { get; set; }
    public UserData_DataProfileFile[] Files { get; set; }
}