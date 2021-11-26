using BinarySerializer;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDataExplorer;

public class BinaryDataViewModel : BaseViewModel
{
    public BinaryDataViewModel()
    {
        LoadAsyncLock = new AsyncLock();
        BinaryDataFiles = new ObservableCollection<BinaryData_FileViewModel>();
        DataLookupTable = new Dictionary<Pointer, FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>>();
    }

    protected bool IsUnloading { get; set; }

    public AsyncLock LoadAsyncLock { get; }
    public bool IsLoading { get; set; }
    public bool IsInitialized { get; set; }

    public IDataManager DataManager { get; protected set; }
    public Context Context { get; protected set; }
    public bool IsInitializing { get; protected set; }

    public ObservableCollection<BinaryData_FileViewModel> BinaryDataFiles { get; }
    public BinaryData_FileViewModel SelectedBinaryDataFile { get; set; }

    public Dictionary<Pointer, FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>> DataLookupTable { get; }

    public async Task LoadAsync(IDataManager dataManager, string basePath, object mode, UserData_DataProfileFile[] files)
    {
        try
        {
            IsInitializing = true;

            // Set context and data manager
            Context = new BinaryExplorerContext(basePath);
            DataManager = dataManager;

            // Load files
            foreach (var file in files)
            {
                // TODO: Allow big endian
                if (file.Address == 0)
                    Context.AddFile(new LinearFile(Context, file.FilePath));
                else
                    Context.AddFile(new MemoryMappedFile(Context, file.FilePath, file.Address, memoryMappedPriority: file.MemoryMappedPriority));
            }

            // Mark as initialized
            IsInitialized = true;

            // Load the data files using the context
            await UseContextAsync(async () =>
            {
                var types = dataManager.GetFileTypes().ToArray();
                var profileFiles = files.Where(x => x.FileType != null).Select(x =>
                {
                    var type = types.FirstOrDefault(t => t.ID == x.FileType);

                    return new IDataManager.ProfileFile(x.FilePath, type?.Type, type?.ID);
                }).ToArray();

                await foreach(BinaryData_FileViewModel file in DataManager.LoadAsync(Context, mode, profileFiles))
                    BinaryDataFiles.Add(file);
            }, returnIfLoading: false);
        }
        finally
        {
            IsInitializing = false;
        }
    }
    public async Task UnloadAsync()
    {
        if (IsUnloading)
            return;

        IsUnloading = true;

        try
        {
            using (await LoadAsyncLock.LockAsync())
            {
                Context?.Dispose();
                Context = null;
                BinaryDataFiles.Clear();
                SelectedBinaryDataFile = null;
                DataLookupTable.Clear();
                IsInitialized = false;
            }
        }
        finally
        {
            IsUnloading = false;
        }
    }

    public void NavigateTo(Pointer pointer)
    {
        if (pointer == null)
            return;

        if (!DataLookupTable.ContainsKey(pointer))
            return;

        var vm = DataLookupTable[pointer];
            
        var dataItems = new List<FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>>();

        FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel> vmParent = vm;

        var prevParent = vmParent;

        while (vmParent != null)
        {
            prevParent = vmParent;
            vmParent = vmParent.Parent;
                
            if (vmParent != null)
                dataItems.Add(vmParent);
        }

        foreach (var item in ((IEnumerable<FlattenedHierarchicalDataItemViewModel<BinaryData_BaseItemViewModel>>)dataItems).Reverse())
            item.IsExpanded = true;

        vm.Data.IsSelected = true;

        var file = ((BinaryData_FlattenedHierarchialDataItemViewModel)prevParent).File;

        // TODO: Recursively expand parents
        file.IsSelected = true;
    }

    public async Task UseContextAsync(Func<Task> func, bool returnIfLoading)
    {
        // Ignore running if it's loading and set to return if loading
        if (IsLoading && returnIfLoading)
            return;

        // Return if unloading
        if (IsUnloading)
            return;

        // Lock, thus waiting until the context is not being used anymore
        using (await LoadAsyncLock.LockAsync())
        {
            // Return if unloading
            if (IsUnloading)
                return;

            IsLoading = true;

            try
            {
                await func();
            }
            finally
            {
                IsLoading = false;
                Context?.Dispose();
            }
        }
    }
    public Task UseContextAsync(Action action, bool returnIfLoading) => UseContextAsync(() =>
    {
        action();
        return Task.CompletedTask;
    }, returnIfLoading);
}