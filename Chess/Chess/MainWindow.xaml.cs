using Chess.GL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Chess
{
    public partial class MainWindow : Window
    {
        bool FirstPlayerSelectedColorWhite = true;
        bool IsMoving = false;
        int SelectedRow, SelectedCol;
        Game Game;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            PlayerColor FirstPlayerColor, SecondPlayerColor;
            if (FirstPlayerSelectedColorWhite)
            {
                FirstPlayerColor = PlayerColor.White;
                SecondPlayerColor = PlayerColor.Black;
            }
            else
            {
                FirstPlayerColor = PlayerColor.Black;
                SecondPlayerColor = PlayerColor.White;
            }
            Game = Game.MakeGame(new Human(FirstPlayerColor), new Human(SecondPlayerColor));
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
                if (Game.GetBoard().GetBlock(row, col).GetPiece() != null && !IsMoving && !Game.IsTurn(Game.GetBoard().GetBlock(row, col).GetPiece().GetColor().ToString()))
                {
                    Console.WriteLine(Game.GetBoard().GetBlock(row, col).GetPiece().ToString());
                    return;
                }
                if (IsMoving)
                {
                    IsMoving = false;
                    MakeMove(SelectedRow, SelectedCol, row, col);
                    RemoveHighlights();
                    return;
                }
                SelectedRow = row;
                SelectedCol = col;
                bool hasImage = false;
                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == SelectedRow && Grid.GetColumn(element) == SelectedCol && element is Image)
                    {
                        hasImage = true;
                        Block block = Game.GetBoard().GetBlock(SelectedRow, SelectedCol);
                        Console.WriteLine(block.GetPiece());
                        if (block.GetPiece() != null)
                        {
                            if(block.GetPiece().GetPieceType() == PieceType.Pawn)
                            {
                                Pawn pawn = (Pawn)block.GetPiece();
                                List <Move> moves = pawn.GetPossibleMoves(Game.GetBoard());
                                if(moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
                HandleSquareClick(SelectedRow, SelectedCol, hasImage);
            }
        }
        
        private void HandleSquareClick(int row, int col, bool hasImage)
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

        private void HighlightSquares(int row, int col)
        {
            Border border = new Border
            {
                Background = Brushes.LightGreen,
                Opacity = 0.5
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            ChessGrid.Children.Add(border);
        }

        private void RemoveHighlights()
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement element in ChessGrid.Children)
            {
                if (element is Border border && border.Background == Brushes.LightGreen)
                {
                    elementsToRemove.Add(element);
                }
            }
            foreach (UIElement element in elementsToRemove)
            {
                ChessGrid.Children.Remove(element);
            }
        }


        private bool IsValidMove(int targetRow, int targetCol)
        {
            foreach (UIElement element in ChessGrid.Children)
            {
                if (element is Border border && border.Background == Brushes.LightGreen)
                {
                    int row = Grid.GetRow(border);
                    int col = Grid.GetColumn(border);

                    if (row == targetRow && col == targetCol)
                    {
                        return true;
                    }
                }
            }
            return false; 
        }

        private void MakeMove(int previousRow, int previousCol, int targetRow, int targetCol)
        {
            Image pieceToMove = null;
            Image capturedPiece = null;

            Block prevBlock = Game.GetBoard().GetBlock(previousRow, previousCol);
            Block targetBlock = Game.GetBoard().GetBlock(targetRow, targetCol);

            if (targetBlock.GetPiece() != null && targetBlock.GetPiece().GetColor() == prevBlock.GetPiece().GetColor())
            {
                Console.WriteLine("Invalid move. Target block has a piece of the same color.");
                return;
            }
            if (!IsValidMove(targetRow, targetCol))
            {
                Console.WriteLine("Invalid block selected.");
                return;
            }

            foreach (UIElement element in ChessGrid.Children)
            {
                if (Grid.GetRow(element) == previousRow && Grid.GetColumn(element) == previousCol && element is Image)
                {
                    pieceToMove = (Image)element;
                    break;
                }
            }

            foreach (UIElement element in ChessGrid.Children)
            {
                if (Grid.GetRow(element) == targetRow && Grid.GetColumn(element) == targetCol && element is Image)
                {
                    capturedPiece = (Image)element;
                    break;
                }
            }

            if (pieceToMove != null)
            {
                if (capturedPiece != null)
                {
                    ChessGrid.Children.Remove(capturedPiece);
                    Console.WriteLine($"Captured piece at Row: {targetRow}, Column: {targetCol}");
                }

                ChessGrid.Children.Remove(pieceToMove);
                Grid.SetRow(pieceToMove, targetRow);
                Grid.SetColumn(pieceToMove, targetCol);
                Game.MakeMove(previousRow, previousCol, targetRow, targetCol);
                ChessGrid.Children.Add(pieceToMove);
            }
            else
            {
                Console.WriteLine($"No piece found at Row: {previousRow}, Column: {previousCol}.");
            }
        }
    }
}