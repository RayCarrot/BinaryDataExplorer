using RayCarrot.UI;
using System;
using System.Collections.Generic;

namespace BinaryDataExplorer;

public class AppUserData : BaseViewModel
{
    /// <summary>
    /// Default constructor. This will always reset the app data to ensure any values missed when reading will still be set correctly.
    /// </summary>
    public AppUserData() => Reset();

    /// <summary>
    /// Resets all properties to their default values
    /// </summary>
    public void Reset()
    {
        App_Version = Services.App.CurrentAppVersion;
        App_Profiles = new List<UserData_DataProfile>();
        UI_WindowState = null;
        Serializer_EnableLog = false;
        Theme_Dark = true;
        Theme_Sync = false;
        DataGrid_ColorMode = UserData_DataGrid_ColorMode.Show;
    }

    /// <summary>
    /// Verifies and corrects any invalid values
    /// </summary>
    public void Verify()
    {
        App_Version ??= Services.App.CurrentAppVersion;
        App_Profiles ??= new List<UserData_DataProfile>();

        foreach (var profile in App_Profiles)
            profile.Files ??= new UserData_DataProfileFile[0];
    }

    public Version App_Version { get; set; }
    public List<UserData_DataProfile> App_Profiles { get; set; }

    public UserData_WindowSessionState UI_WindowState { get; set; }
        
    public bool Serializer_EnableLog { get; set; }

    public bool Theme_Dark { get; set; }
    public bool Theme_Sync { get; set; }

    public UserData_DataGrid_ColorMode DataGrid_ColorMode { get; set; }
}