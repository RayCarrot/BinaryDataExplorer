using System.Linq;

namespace BinaryDataExplorer
{
    public static class DesignTimeVM
    {
        public static AppViewModel App
        {
            get
            {
                var app = new AppViewModel();

                app.RefreshDataManagers();

                app.BinaryData.BinaryDataFiles.Add(new BinaryData_FileViewModel(new BinaryData_File($"BIN", null)
                {
                    HasFiles = true,
                    GetFilesFunc = () =>
                    {
                        return new BinaryData_File[]
                        {
                            new BinaryData_File("Test", null)
                        }.ToAsyncEnumerable();
                    },
                    GetAdditionalDataItemsFunc = () => new BinaryData_BaseItemViewModel[]
                    {
                        new BinaryData_DefaultItemViewModel(null, null, typeof(int), "Test", "35"),
                    },
                }));

                app.BinaryData.SelectedBinaryDataFile = app.BinaryData.BinaryDataFiles.First();

                app.BinaryData.SelectedBinaryDataFile.IsExpanded = true;
                app.BinaryData.SelectedBinaryDataFile.IsSelected = true;

                app.BinaryData.IsInitialized = true;

                return app;
            }
        }

        public static EditProfileViewModel EditProfile
        {
            get
            {
                var managers = App.DataManagers;

                var vm = new EditProfileViewModel(managers, managers.First(), new UserData_DataProfileFile[]
                {
                    new UserData_DataProfileFile
                    {
                        FilePath = "NormalFile.bin",
                        Address = 0,
                        IsReadOnly = false,
                        FileType = null
                    },
                });

                return vm;
            }
        }
    }
}