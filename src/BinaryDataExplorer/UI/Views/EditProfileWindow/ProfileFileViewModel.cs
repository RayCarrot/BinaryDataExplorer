using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    public class ProfileFileViewModel : BaseViewModel
    {
        public ProfileFileViewModel(EditProfileViewModel editProfileViewModel, IDataManager manager, bool isReadOnly)
        {
            EditProfileViewModel = editProfileViewModel;
            IsReadOnly = isReadOnly;

            var fileTypes = new IDataManager.FileType("None", null, null).Yield().Concat(manager.GetFileTypes());
            FileTypes = new ObservableCollection<IDataManager.FileType>(fileTypes);

            SelectedFileType = FileTypes.First();

            DeleteCommand = new RelayCommand(Delete);
        }

        public ICommand DeleteCommand { get; }

        public ObservableCollection<IDataManager.FileType> FileTypes { get; }

        public EditProfileViewModel EditProfileViewModel { get; }

        public bool IsReadOnly { get; }
        public string FilePath { get; set; }
        public long Address { get; set; }
        public IDataManager.FileType SelectedFileType { get; set; }
        public string FileType
        {
            get => SelectedFileType?.ID;
            set => SelectedFileType = FileTypes.FirstOrDefault(x => x.ID == value);
        }

        public void Delete()
        {
            EditProfileViewModel.Files.Remove(this);
        }
    }
}