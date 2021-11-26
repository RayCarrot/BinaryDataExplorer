using GongSolutions.Wpf.DragDrop;

namespace BinaryDataExplorer;

public class ProfilesListDropTarget : DefaultDropHandler
{
    public override void Drop(IDropInfo dropInfo)
    {
        // Handle the drop
        base.Drop(dropInfo);

        // Get the source object being moved
        var src = (ProfileViewModel)dropInfo.Data;

        // Move the object to the new position in the app data
        var objects = Services.App.UserData.App_Profiles;
        objects.Move(objects.IndexOf(src.Profile), dropInfo.InsertIndex);
    }
}