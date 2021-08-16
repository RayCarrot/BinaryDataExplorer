using BinarySerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RayCarrot.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BinaryDataExplorer
{
    public class AppViewModel : BaseViewModel
    {
        #region Constructor

        public AppViewModel()
        {
            BinaryData = new BinaryDataViewModel();
            DataManagers = new ObservableCollection<IDataManager>();

            Path_AppDataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            Path_AppUserDataFile = Path.Combine(Path_AppDataDir, $"Settings.json");
            Path_SerializerLogFile = Path.Combine(Path_AppDataDir, $"SerializerLog.txt");

            LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
            ExitCommand = new RelayCommand(Exit);
            GoToCommand = new RelayCommand(GoTo);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenURLCommand = new RelayCommand(x => OpenURL(x?.ToString()));
        }

        #endregion

        #region Paths

        public string Path_AppDataDir { get; }
        public string Path_AppUserDataFile { get; }
        public string Path_SerializerLogFile { get; }

        #endregion

        #region Commands

        public ICommand LoadProfileCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand GoToCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenURLCommand { get; }

        #endregion

        #region Public Properties

        public string Title => $"Binary Data Explorer - {CurrentAppVersion}";
        public Version CurrentAppVersion => new Version(0, 1, 0, 0);
        public AppUserData UserData { get; set; }

        public BinaryDataViewModel BinaryData { get; }
        public ObservableCollection<IDataManager> DataManagers { get; }
        public bool IsUnloaded { get; private set; }

        #endregion

        #region Private Methods

        private IEnumerable<IDataManager> GetAvailableDataManagers()
        {
            // TODO: Instead of hard-coding this have data managers get loaded from plugins (.dll files in a plugins dir which all reference an API)?
            return new IDataManager[]
            {
                new Ray1_PC_DataManager(),
                new Klonoa_DTP_DataManager(),
                new Klonoa_LV_DataManager(),
                new PS1_DataManager(),
                new GBA_DataManager(),
                new Image_DataManager(),
            };
        }

        #endregion

        #region Public Methods

        public void Initialize(string[] args)
        {
            // Create the data directory
            Directory.CreateDirectory(Path_AppDataDir);

            // Load data
            LoadAppUserData();

            if (UserData.App_Version < CurrentAppVersion)
                PostUpdate(UserData.App_Version);

            // Update the version
            UserData.App_Version = CurrentAppVersion;

            // Get the data managers
            RefreshDataManagers();
        }

        public void RefreshDataManagers()
        {
            DataManagers.Clear();

            foreach (IDataManager dataManager in GetAvailableDataManagers())
                DataManagers.Add(dataManager);
        }

        public void LoadAppUserData()
        {
            if (File.Exists(Path_AppUserDataFile))
            {
                try
                {
                    var json = File.ReadAllText(Path_AppUserDataFile);
                    UserData = JsonConvert.DeserializeObject<AppUserData>(json, new StringEnumConverter());

                    if (UserData == null)
                        throw new Exception($"User data was empty");

                    UserData.Verify();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred when loading the app user data. Error message: {ex.Message}");

                    ResetAppUserData();
                }
            }
            else
            {
                ResetAppUserData();
            }
        }

        public void ResetAppUserData()
        {
            UserData = new AppUserData();
        }

        public void SaveAppUserData()
        {
            // Serialize to JSON and save to file
            var json = JsonConvert.SerializeObject(UserData, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(Path_AppUserDataFile, json);
        }

        public void PostUpdate(Version prevVersion) { }

        public async Task LoadProfileAsync()
        {
            var loadProfileViewModel = App.Current.OpenWindowDialog(new LoadProfileWindow(), new LoadProfileViewModel());

            SaveAppUserData();

            if (loadProfileViewModel == null)
                return;

            var profile = loadProfileViewModel.SelectedProfile.Profile;

            // Make sure each file exists
            foreach (var file in profile.Files)
            {
                if (!File.Exists(Path.Combine(loadProfileViewModel.SelectedProfile.Profile.DataPath, file.FilePath)))
                {
                    // TODO: Move to UI manager
                    MessageBox.Show($"The file '{file.FilePath}' doesn't exist", "Error loading profile", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Unload currently loaded data
            await BinaryData.UnloadAsync();

            IDataManager dataManager = GetDataManager(profile.DataManager);

            if (dataManager == null)
            {
                // TODO: Move to UI manager
                MessageBox.Show("Data manager not found", "Error loading profile", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            object mode = dataManager.GetModes()?.FirstOrDefault(x => x.ID == profile.Mode)?.Mode;

            // Load the new data
            await BinaryData.LoadAsync(dataManager, profile.DataPath, mode, profile.Files);
        }

        public void GoTo()
        {
            if (!BinaryData.IsInitialized)
                return;

            var goToViewModel = App.Current.OpenWindowDialog(new GoToWindow(), new GoToViewModel(BinaryData.Context.MemoryMap.Files.Select(x => x.FilePath)));
            
            if (goToViewModel == null)
                return;

            BinaryData.NavigateTo(new Pointer(goToViewModel.Address, BinaryData.Context.GetFile(goToViewModel.File)));
        }

        public void OpenSettings()
        {
            App.Current.OpenWindowDialog(new SettingsWindow(), new SettingsViewModel());

            SaveAppUserData();
        }

        public void OpenURL(string url) => Services.File.OpenURL(url);

        public IDataManager GetDataManager(string name) => DataManagers.FirstOrDefault(x => x.GetType().FullName == name);

        public async Task UnloadAsync()
        {
            // Unload the data
            await BinaryData.UnloadAsync();

            // Save the app user data
            SaveAppUserData();

            IsUnloaded = true;
        }

        public void Exit() => Application.Current.MainWindow?.Close();

        #endregion
    }
}