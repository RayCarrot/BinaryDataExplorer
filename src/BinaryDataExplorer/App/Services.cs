namespace BinaryDataExplorer;

public static class Services
{
    static Services()
    {
        // IDEA: Move to dependency injection?
        App = new AppViewModel();
        File = new FileManager();
    }

    public static AppViewModel App { get; }
    public static BinaryDataViewModel BinaryData => App.BinaryData;
    public static FileManager File { get; }
}