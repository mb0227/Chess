using Chess.GL;
using System;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess.Views
{
    public partial class PromotionWindow : Window
    {
        public string SelectedPiece { get; private set; }

        public PromotionWindow()
        {
            InitializeComponent();
            InitializePromotionPieces();
        }

        private void InitializePromotionPieces()
        {
            // List of piece names
            string[] pieceNames = { "Queen", "Rook", "Bishop", "Knight" };
            string[] pieceFileNames = { "white_queen", "white_rook", "white_bishop", "white_knight" };

            for (int i = 0; i < pieceNames.Length; i++)
            {
                Button pieceButton = (Button)this.FindName($"{pieceNames[i]}Button");
                var imageSource = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(pieceFileNames[i]);

                Image pieceImage = new Image
                {
                    Source = UtilityFunctions.BitmapToBitmapImage(imageSource),
                    Width = 60,
                    Height = 60
                };

                // Create TextBlock for the piece name
                TextBlock pieceText = new TextBlock
                {
                    Text = pieceNames[i],
                    Style = (Style)this.Resources["PieceNameStyle"]
                };

                // Add Image and TextBlock to the Button's content using a StackPanel layout
                StackPanel buttonStackPanel = new StackPanel();
                buttonStackPanel.Children.Add(pieceText);
                buttonStackPanel.Children.Add(pieceImage);
                pieceButton.Content = buttonStackPanel;
            }
        }

        private void OnPromotionSelected(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SelectedPiece = button.Name.Replace("Button", "");

                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
