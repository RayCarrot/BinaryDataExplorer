using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RayCarrot.UI;

namespace BinaryDataExplorer
{
    public class GoToViewModel : BaseViewModel
    {
        public GoToViewModel(IEnumerable<string> files)
        {
            Files = new ObservableCollection<string>(files);
            File = Files.FirstOrDefault();
        }

        public ObservableCollection<string> Files { get; }
        public long Address { get; set; }
        public string File { get; set; }
    }
}