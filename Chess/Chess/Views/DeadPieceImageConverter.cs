using System;
using Chess.GL;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Chess.Views
{
    public class DeadPieceImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() != "")
            {
                var imageSource = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(value.ToString());
                BitmapImage bitmapImage = UtilityFunctions.BitmapToBitmapImage(imageSource);
                return bitmapImage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
