using System.IO;
using System.Windows;
using System.Windows.Input;
using RayCarrot.UI;

namespace BinaryDataExplorer;

public class SettingsViewModel : BaseViewModel
{
    #region Constructor

    public SettingsViewModel()
    {
        Data = Services.App.UserData;
        OpenSerializerLogCommand = new RelayCommand(OpenSerializerLog);
    }

    #endregion

    #region Commands

    public ICommand OpenSerializerLogCommand { get; }

    #endregion

    #region Public Properties

    public AppUserData Data { get; }

    #endregion

    #region Public Methods

    public void OpenSerializerLog()
    {
        var file = Services.App.Path_SerializerLogFile;

        if (File.Exists(file))
            Services.File.LaunchFile(file);
        else
            // TODO: Move to UI manager
            MessageBox.Show("No serializer log file has been created", "File does not exist");
    }

    #endregion
}