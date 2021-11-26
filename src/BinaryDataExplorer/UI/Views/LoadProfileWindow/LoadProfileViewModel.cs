using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BinaryDataExplorer;

public class LoadProfileViewModel : BaseViewModel
{
    #region Constructor

    public LoadProfileViewModel()
    {
        // Set properties
        Profiles = new ObservableCollection<ProfileViewModel>(Services.App.UserData.App_Profiles.Select(x => new ProfileViewModel(this, x)));

        // Create commands
        AddProfileCommand = new RelayCommand(AddProfile);
    }

    #endregion

    #region Commands

    public ICommand AddProfileCommand { get; }

    #endregion

    #region Public Properties

    public ObservableCollection<ProfileViewModel> Profiles { get; }
    public ProfileViewModel SelectedProfile { get; set; }

    #endregion

    #region Public Methods

    public void AddProfile()
    {
        var profile = new UserData_DataProfile();

        var result = EditProfile(profile);

        if (!result)
            return;

        Services.App.UserData.App_Profiles.Add(profile);
        Profiles.Add(new ProfileViewModel(this, profile));
    }

    public bool EditProfile(UserData_DataProfile profile)
    {
        // TODO: Move to UI manager
        var editProfileWindow = new EditProfileWindow();
        var editProfileViewModel = new EditProfileViewModel(Services.App.DataManagers, Services.App.GetDataManager(profile.DataManager), profile.Files)
        {
            SelectedName = profile.Name,
            SelectedPath = profile.DataPath,
            SelectedModeID = profile.Mode,
        };
        editProfileWindow.DataContext = editProfileViewModel;
        editProfileWindow.ShowDialog();

        if (editProfileWindow.DialogResult != true)
            return false;

        if (editProfileViewModel.SelectedManager == null)
        {
            // TODO: Move to UI manager
            MessageBox.Show("No data manager selected", "Error loading data", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!Directory.Exists(editProfileViewModel.SelectedPath))
        {
            // TODO: Move to UI manager
            MessageBox.Show("Data path does not exist", "Error loading data", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        profile.Name = editProfileViewModel.SelectedName;
        profile.DataManager = editProfileViewModel.SelectedManager.GetType().FullName;
        profile.DataPath = editProfileViewModel.SelectedPath;
        profile.Mode = editProfileViewModel.SelectedMode.ID;
        profile.Files = editProfileViewModel.Files.Select(x => new UserData_DataProfileFile
        {
            FilePath = x.FilePath,
            Address = x.Address,
            MemoryMappedPriority = x.MemoryMappedPriority,
            IsReadOnly = x.IsReadOnly,
            FileType = x.FileType
        }).ToArray();

        return true;
    }
        
    #endregion
}