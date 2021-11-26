using System.Diagnostics;
using System.IO;

namespace BinaryDataExplorer;

public class FileManager
{
    // TODO: Error handling

    public void OpenURL(string url)
    {
        url = url.Replace("&", "^&");
        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
        {
            CreateNoWindow = true
        });
    }

    public Process LaunchFile(string file, bool asAdmin = false, string arguments = null, string wd = null)
    {
        // Create the process start info
        ProcessStartInfo info = new ProcessStartInfo
        {
            // Set the file path
            FileName = file,

            // Set to working directory to the parent directory if not otherwise specified
            WorkingDirectory = wd ?? Path.GetDirectoryName(file),

            UseShellExecute = true
        };

        // Set arguments if specified
        if (arguments != null)
            info.Arguments = arguments;

        // Set to run as admin if specified
        if (asAdmin)
            info.Verb = "runas";

        // Start the process and get the process
        var p = Process.Start(info);

        // Return the process
        return p;
    }

    public void OpenExplorerPath(string path)
    {
        if (File.Exists(path))
            Process.Start("explorer.exe", "/select, \"" + path + "\"")?.Dispose();
        else if (Directory.Exists(path))
            Process.Start("explorer.exe", path)?.Dispose();
    }
}