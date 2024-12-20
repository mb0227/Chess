using System.Windows.Media.Imaging;

namespace Chess.GL
{
    public class UtilityFunctions
    {
        public static PieceType GetPieceTypeByString(string pieceName)
        {
            switch (pieceName)
            {
                case "queen":
                    return PieceType.Queen;
                case "bishop":
                    return PieceType.Bishop;
                case "rook":
                    return PieceType.Rook;
                case "knight":
                    return PieceType.Knight;
                default:
                    return PieceType.Queen;
            }
        }

        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
