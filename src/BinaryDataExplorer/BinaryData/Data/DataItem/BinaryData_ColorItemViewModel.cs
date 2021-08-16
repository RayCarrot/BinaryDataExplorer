using BinarySerializer;
using System.Windows.Media;

namespace BinaryDataExplorer
{
    public class BinaryData_ColorItemViewModel : BinaryData_DefaultItemViewModel
    {
        public BinaryData_ColorItemViewModel(BinaryData_BaseItemViewModel parent, Pointer address, string name, BaseColor color) 
            : base(parent: parent, address: address, type: color.GetType(), name: name, value: color.ShortLog)
        {
            BaseColor = color;
        }

        public BaseColor BaseColor { get; }
        public Color Color
        {
            get
            {
                return Services.App.UserData.DataGrid_ColorMode switch
                {
                    UserData_DataGrid_ColorMode.Show => 
                        Color.FromArgb((byte) (BaseColor.Alpha * 255), (byte) (BaseColor.Red * 255), (byte) (BaseColor.Green * 255), (byte) (BaseColor.Blue * 255)),
                    
                    UserData_DataGrid_ColorMode.Show_IgnoreAlpha => 
                        Color.FromRgb((byte) (BaseColor.Red * 255), (byte) (BaseColor.Green * 255), (byte) (BaseColor.Blue * 255)),
                    
                    _ => default
                };
            }
        }
        public Brush ColorBrush => new SolidColorBrush(Color);
    }
}