using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    public partial class MainWindow : Window
    {
        bool FirstPlayerSelectedColorWhite = true;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            string[] pieces = { "rook", "knight", "bishop", "queen", "king", "bishop", "knight", "rook" };
            if (!FirstPlayerSelectedColorWhite)
            {
                pieces = new string[] { "rook", "knight", "bishop", "king", "queen", "bishop", "knight", "rook" };
            }
            string imageFolderPath = "..\\..\\Images", color;
            Image image;

            // Placing pieces
            for (int row = 0; row < 8; row += 7) // Row 0 (White) and 7 (Black)
            {
                if(FirstPlayerSelectedColorWhite) color = (row == 0) ? "black" : "white";
                else color = (row == 0) ? "white" : "black";

                for (int col = 0; col < 8; col++)
                {
                    string piece = pieces[col];
                    string imagePath = System.IO.Path.Combine(imageFolderPath, $"{color}-{piece}.png");
                    imagePath = System.IO.Path.GetFullPath(imagePath);

                    image = new Image
                    {
                        Width = 53,
                        Height = 53,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                        IsHitTestVisible = false,
                        Name = $"{piece}"
                    };
                    Grid.SetRow(image, row);
                    Grid.SetColumn(image, col);
                    ChessGrid.Children.Add(image);
                }
            }

            // Placing pawns
            for (int row = 1; row <= 6; row += 5) // Rows 1 (White) and 6 (Black)
            {
                if(FirstPlayerSelectedColorWhite)
                    color = (row == 1) ? "black" : "white";
                else
                    color = (row == 1) ? "white" : "black";

                string piece = "pawn";

                for (int col = 0; col < 8; col++)
                {
                    string imagePath = System.IO.Path.Combine(imageFolderPath, $"{color}-{piece}.png");
                    imagePath = System.IO.Path.GetFullPath(imagePath);
                    image = new Image
                    {
                        Width = 53,
                        Height = 53,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                        IsHitTestVisible = false,
                        Name = $"{piece}"
                    };
                    Grid.SetRow(image, row);
                    Grid.SetColumn(image, col);
                    ChessGrid.Children.Add(image);
                }
            }
            AddRanksAndFilesLabels();
        }

        private void AddRanksAndFilesLabels()
        {
            string[] ranks = { "8", "7", "6", "5", "4", "3", "2", "1" };
            string[] files = { "a", "b", "c", "d", "e", "f", "g", "h" };
            if (!FirstPlayerSelectedColorWhite)
            {
                ranks = ranks.Reverse().ToArray();
                files = files.Reverse().ToArray();
            }
            for (int i = 0; i < 8; i++)
            {
                TextBlock rankTextBlock = new TextBlock
                {
                    Text = ranks[i],
                    Foreground = Brushes.SaddleBrown,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 13
                };
                Grid.SetRow(rankTextBlock, i);
                Grid.SetColumn(rankTextBlock, 0);
                ChessGrid.Children.Add(rankTextBlock);
                TextBlock fileTextBlock = new TextBlock
                {
                    Text = files[i],
                    Foreground = Brushes.SaddleBrown,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 5, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 13
                };
                Grid.SetRow(fileTextBlock, 8);
                Grid.SetColumn(fileTextBlock, i);
                ChessGrid.Children.Add(fileTextBlock);
            }
        }

        private void Chessboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = sender as Border;
            if (clickedElement != null)
            {
                int row = Grid.GetRow(clickedElement);
                int col = Grid.GetColumn(clickedElement);

                bool hasImage = false;
                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == row && Grid.GetColumn(element) == col && element is Image)
                    {
                        hasImage = true;
                        Console.Write((element as Image).Name);
                        Console.WriteLine(" " + row.ToString()  + col.ToString());
                        break;
                    }
                }
                HandleSquareClick(row, col, hasImage);
            }
        }

        void HandleSquareClick(int row, int col, bool hasImage)
        {
            if (hasImage)
            {
                ClickedBoxTextBlock.Text = $"Row: {row} Column: {col} Piece.";
            }
            else
            {
                ClickedBoxTextBlock.Text = $"Row: {row} Column: {col} Empty.";
            }
        }
    }
}
