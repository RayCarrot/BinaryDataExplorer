using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    public class EditProfileViewModel : BaseViewModel
    {
        #region Constructor

        public EditProfileViewModel(IEnumerable<IDataManager> managers, IDataManager manager = null, IEnumerable<UserData_DataProfileFile> files = null)
        {
            DataManagers = new ObservableCollection<IDataManager>(managers);
            Modes = new ObservableCollection<IDataManager.DataManagerMode>();

            if (!DataManagers.Any())
                throw new Exception("Can't load data when no data managers are available");

            BrowseDataPathCommand = new RelayCommand(BrowseDataPath);
            AddFileCommand = new RelayCommand(AddFile);

            SelectedManager = manager ?? DataManagers.First();

            files ??= Enumerable.Empty<UserData_DataProfileFile>();
            Files = new ObservableCollection<ProfileFileViewModel>(files.Select(x => new ProfileFileViewModel(this, manager, x.IsReadOnly)
            {
                FilePath = x.FilePath,
                Address = x.Address,
                MemoryMappedPriority = x.MemoryMappedPriority,
                FileType = x.FileType,
            }));
        }

        #endregion

        #region Commands

        public ICommand BrowseDataPathCommand { get; }
        public ICommand AddFileCommand { get; }

        #endregion

        #region Private Fields

        private IDataManager _selectedManager;
        private IDataManager.DataManagerMode _selectedMode;

        #endregion

        #region Public Properties

        public ObservableCollection<IDataManager> DataManagers { get; }
        public ObservableCollection<IDataManager.DataManagerMode> Modes { get; }

        public string SelectedName { get; set; }

        public IDataManager SelectedManager
        {
            get => _selectedManager;
            set
            {
                if (value == _selectedManager)
                    return;

                _selectedManager = value;

                Modes.Clear();
                RefreshFiles();

                if (SelectedManager != null)
                {
                    Modes.AddRange(SelectedManager.GetModes());
                    SelectedMode = Modes.FirstOrDefault();
                }
            }
        }

        public string SelectedPath { get; set; }

        public IDataManager.DataManagerMode SelectedMode
        {
            get => _selectedMode;
            set
            {
                if (value == _selectedMode)
                    return;

                _selectedMode = value;

                RefreshReadOnlyFiles();
            }
        }

        public string SelectedModeID
        {
            get => SelectedMode?.ID;
            set => SelectedMode = Modes.FirstOrDefault(x => x.ID == value);
        }

        public ObservableCollection<ProfileFileViewModel> Files { get; }

        #endregion

        #region Protected Methods

        protected void RefreshFiles()
        {
            Files?.Clear();
        }

        protected void RefreshReadOnlyFiles()
        {
            if (Files == null)
                return;

            foreach (var file in Files.Where(x => x.IsReadOnly).ToArray())
                Files.Remove(file);

            if (SelectedManager == null || _selectedMode == null) 
                return;

            foreach (var file in SelectedManager.GetDefaultFiles(_selectedMode.Mode))
            {
                Files.Add(new ProfileFileViewModel(this, SelectedManager, true)
                {
                    FilePath = file.FilePath,
                    Address = file.Address,
                    MemoryMappedPriority = file.MemoryMappedPriority,
                });
            }
        }

        #endregion

        #region Public Methods

        public void BrowseDataPath()
        {
            using var dialog = new CommonOpenFileDialog
            {
                Title = "Select the data path",
                InitialDirectory = SelectedPath,
                AllowNonFileSystemItems = false,
                IsFolderPicker = true,
                EnsureFileExists = true,
                EnsurePathExists = true
            };

            // Show the dialog
            var dialogResult = dialog.ShowDialog(Application.Current.Windows.Cast<Window>().FirstOrDefault(x => x.IsActive));

            if (dialogResult == CommonFileDialogResult.Ok)
                SelectedPath = dialog.FileName;
        }

        public void AddFile()
        {
            Files.Add(new ProfileFileViewModel(this, SelectedManager, false));
        }

        #endregion
    }
}