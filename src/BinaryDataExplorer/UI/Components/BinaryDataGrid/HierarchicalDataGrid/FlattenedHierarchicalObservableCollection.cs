using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BinaryDataExplorer;

public class FlattenedHierarchicalObservableCollection<T> : ObservableCollection<FlattenedHierarchicalDataItemViewModel<T>>
{
    public FlattenedHierarchicalObservableCollection()
    {
        HierarchicalData = new List<FlattenedHierarchicalDataItemViewModel<T>>();
    }

    protected List<FlattenedHierarchicalDataItemViewModel<T>> HierarchicalData { get; }
        
    public void Initialize()
    {
        Clear();
            
        foreach (FlattenedHierarchicalDataItemViewModel<T> m in HierarchicalData.Where(c => c.IsVisible).SelectMany(x => new[] { x }.Concat(x.GetVisibleDescendants)))
        {
            Add(m);
        }
    }

    public void AddData(FlattenedHierarchicalDataItemViewModel<T> dataItem) => HierarchicalData.Add(dataItem);

    public void AddVisibleChildren(FlattenedHierarchicalDataItemViewModel<T> d)
    {
        if (!Contains(d))
            return;
            
        int parentIndex = IndexOf(d);
            
        foreach (FlattenedHierarchicalDataItemViewModel<T> c in d.Children)
        {
            parentIndex += 1;
            Insert(parentIndex, c);
        }
    }

    public void RemoveVisibleChildren(FlattenedHierarchicalDataItemViewModel<T> d)
    {
        foreach (var c in d.Children.Where(c => Contains(c)))
            Remove(c);
    }
}