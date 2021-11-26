using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BinaryDataExplorer;

/// <summary>
/// A base window to inherit from
/// </summary>
public class BaseWindow : MetroWindow
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public BaseWindow()
    {
        // Default to true
        CloseWithEscape = true;

        // Set minimum size
        MinWidth = 400;
        MinHeight = 300;

        // Set title style
        TitleCharacterCasing = CharacterCasing.Normal;

        // Set owner window
        var owner = Application.Current?.Windows.Cast<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current?.MainWindow;

        if (owner != this)
            Owner = owner;

        // Do not show in the task bar if the window has a owner, is not the main window and a main window has been created
        if (Owner != null && Application.Current?.MainWindow != null && this != Application.Current.MainWindow)
            ShowInTaskbar = false;

        // Due to a WPF glitch the main window needs to be focused upon closing
        Closed += (_, _) =>
        {
            if (this != Application.Current.MainWindow)
                Application.Current.MainWindow?.Focus();
        };

        PreviewKeyDown += (_, e) =>
        {
            if (CloseWithEscape && e.Key == Key.Escape)
                Close();
        };
    }

    /// <summary>
    /// Shows the <see cref="Window"/> as a dialog
    /// </summary>
    public new void ShowDialog()
    {
        // Set startup location
        WindowStartupLocation = Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;

        // Show the window as a dialog
        base.ShowDialog();
    }

    /// <summary>
    /// Indicates if the escape key can be used to close the window
    /// </summary>
    public bool CloseWithEscape { get; set; }
}