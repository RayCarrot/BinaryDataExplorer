using System.Windows.Input;
using RayCarrot.UI;

namespace BinaryDataExplorer;

public class ProfileViewModel : BaseViewModel
{
    public ProfileViewModel(LoadProfileViewModel loadProfileViewModel, UserData_DataProfile profile)
    {
        // Set properties
        LoadProfileViewModel = loadProfileViewModel;
        Profile = profile;
        Header = Profile.Name;

        // Create commands
        EditCommand = new RelayCommand(Edit);
        OpenFolderCommand = new RelayCommand(OpenFolder);
        DeleteCommand = new RelayCommand(Delete);
    }

    public ICommand EditCommand { get; }
    public ICommand OpenFolderCommand { get; }
    public ICommand DeleteCommand { get; }

    public LoadProfileViewModel LoadProfileViewModel { get; }
    public UserData_DataProfile Profile { get; }
    public bool IsSelected { get; set; }

    public string Header { get; set; }

    public void Edit()
    {
        LoadProfileViewModel.EditProfile(Profile);
        Header = Profile.Name;
    }

    public void OpenFolder()
    {
        Services.File.OpenExplorerPath(Profile.DataPath);
    }

    public void Delete()
    {
        // Remove from the view model
        LoadProfileViewModel.Profiles.Remove(this);

        // Clear the selection
        LoadProfileViewModel.SelectedProfile = null;

        // Remove from user data
        Services.App.UserData.App_Profiles.Remove(Profile);
    }
}