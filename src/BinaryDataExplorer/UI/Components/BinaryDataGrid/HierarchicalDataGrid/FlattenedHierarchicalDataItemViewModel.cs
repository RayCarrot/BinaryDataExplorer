using System.Collections.Generic;
using System.Linq;
using RayCarrot.UI;

namespace BinaryDataExplorer;

public class FlattenedHierarchicalDataItemViewModel<T> : BaseViewModel
{
    public FlattenedHierarchicalDataItemViewModel(FlattenedHierarchicalObservableCollection<T> flattenedCollection, FlattenedHierarchicalDataItemViewModel<T> parent, T data)
    {
        FlattenedCollection = flattenedCollection;
        Parent = parent;
        Children = new List<FlattenedHierarchicalDataItemViewModel<T>>();
        Data = data;

        if (Parent == null)
            IsVisible = true;
    }

    public FlattenedHierarchicalObservableCollection<T> FlattenedCollection { get; }
    public FlattenedHierarchicalDataItemViewModel<T> Parent { get; }
    public List<FlattenedHierarchicalDataItemViewModel<T>> Children { get; }
    public T Data { get; }

    public FlattenedHierarchicalDataItemViewModel<T> AddChild(T data)
    {
        var item = new FlattenedHierarchicalDataItemViewModel<T>(FlattenedCollection, this, data);
        Children.Add(item);
        return item;
    }
    public bool HasChildren => Children.Any();

    private int? _level;
    public int Level => _level ??= Parent?.Level + 1 ?? 0;

    public bool IsExpanded
    {
        get => _expanded;
        set
        {
            if (_expanded == value) 
                return;
                
            _expanded = value;
                
            if (_expanded)
                Expand();
            else
                Collapse();
        }
    }
    public bool IsVisible
    {
        get => _visible;
        set
        {
            if (_visible == value) 
                return;
                
            _visible = value;
                
            if (_visible)
                ShowChildren();
            else
                HideChildren();
        }
    }

    public IEnumerable<FlattenedHierarchicalDataItemViewModel<T>> GetVisibleDescendants =>
        Children.Where(x => x.IsVisible).SelectMany(x => new[] { x }.Concat(x.GetVisibleDescendants));

    private bool _expanded;
    private bool _visible;

    private void Collapse()
    {
        FlattenedCollection.RemoveVisibleChildren(this);

        foreach (FlattenedHierarchicalDataItemViewModel<T> d in Children)
            d.IsVisible = false;
    }
    private void Expand()
    {
        FlattenedCollection.AddVisibleChildren(this);

        foreach (FlattenedHierarchicalDataItemViewModel<T> d in Children)
            d.IsVisible = true;
    }

    private void HideChildren()
    {
        if (!IsExpanded) 
            return;
            
        FlattenedCollection.RemoveVisibleChildren(this);

        foreach (FlattenedHierarchicalDataItemViewModel<T> d in Children)
            d.IsVisible = false;
    }
    private void ShowChildren()
    {
        if (!IsExpanded) 
            return;
            
        FlattenedCollection.AddVisibleChildren(this);
            
        foreach (FlattenedHierarchicalDataItemViewModel<T> d in Children)
            d.IsVisible = true;
    }
}