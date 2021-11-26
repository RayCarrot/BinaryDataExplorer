namespace BinaryDataExplorer;

public class UserData_DataProfileFile
{
    public string FilePath { get; set; }
    public long Address { get; set; }
    public long MemoryMappedPriority { get; set; } = -1;
    public bool IsReadOnly { get; set; }
    public string FileType { get; set; }
}