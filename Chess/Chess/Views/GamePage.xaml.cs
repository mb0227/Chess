using Chess.GL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chess.Views
{
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<string> _moves;

        public ObservableCollection<string> Moves
        {
            get => _moves;
            set
            {
                if (_moves != value)
                {
                    _moves = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddMove(string move)
        {
            Moves.Add(move);  // Add the move to the list
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool FirstPlayerSelectedColorWhite = true;
        bool PromotionPossible = false;
        bool enPassantPossible = false;
        bool castlingPossible = false;
        bool IsMoving = false;
        int SelectedRow, SelectedCol;
        Piece SelectedPiece;
        Game Game;

        public GamePage()
        {
            InitializeComponent();
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
            InitializeBoard();
            DataContext = this;
            Moves = new ObservableCollection<string>();

            // Subscribe to the MoveMade event of the Game class
            Game.MoveMade += AddMove;
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
                if (FirstPlayerSelectedColorWhite) color = (row == 0) ? "black" : "white";
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
                if (FirstPlayerSelectedColorWhite)
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
                    return;
                }

                if (!IsMoving)
                {
                    SelectedPiece = Game.GetBoard().GetBlock(row, col).GetPiece();
                }

                if (CanEnPassant(row, col))
                {
                    enPassantPossible = true;
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
                        if (block.GetPiece() != null)
                        {
                            if (block.GetPiece().GetPieceType() == PieceType.Pawn)
                            {
                                Pawn pawn = (Pawn)block.GetPiece();
                                List<Move> moves = pawn.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                        if (move.GetIsPromotion()) PromotionPossible = true;
                                    }
                                }
                                else return;
                            }
                            else if (block.GetPiece().GetPieceType() == PieceType.Knight)
                            {
                                Knight knight = (Knight)block.GetPiece();
                                List<Move> moves = knight.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                                else return;
                            }
                            else if (block.GetPiece().GetPieceType() == PieceType.Bishop)
                            {
                                Bishop bishop = (Bishop)block.GetPiece();
                                List<Move> moves = bishop.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                                else return;
                            }
                            else if (block.GetPiece().GetPieceType() == PieceType.Rook)
                            {
                                Rook rook = (Rook)block.GetPiece();
                                List<Move> moves = rook.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                                else return;
                            }
                            else if (block.GetPiece().GetPieceType() == PieceType.Queen)
                            {
                                Queen queen = (Queen)block.GetPiece();
                                List<Move> moves = queen.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                                else return;
                            }
                            else if (block.GetPiece().GetPieceType() == PieceType.King)
                            {
                                King king = (King)block.GetPiece();
                                List<Move> moves = king.GetPossibleMoves(Game.GetBoard());
                                if (moves.Count > 0)
                                {
                                    IsMoving = true;
                                    foreach (Move move in moves)
                                    {
                                        if (Math.Abs(move.GetStartBlock().GetFile() - move.GetEndBlock().GetFile()) == 2)
                                        {
                                            castlingPossible = true;
                                        }
                                        HighlightSquares(move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                                    }
                                }
                                else return;
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
                //ClickedBoxTextBlock.Text = $"Row: {row} Column: {col} Piece.";
            }
            else
            {
                //ClickedBoxTextBlock.Text = $"Row: {row} Column: {col} Empty.";
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
            if (!IsValidMove(targetRow, targetCol)) return;
            Image pieceToMove = null;
            Image capturedPiece = null;

            Block prevBlock = Game.GetBoard().GetBlock(previousRow, previousCol);
            Block targetBlock = Game.GetBoard().GetBlock(targetRow, targetCol);

            string optionSelected = null;
            int enPassantTargetRow = -1;
            int castlingTargetFileRook = -1, rookCurrentFile = -1;

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

            if (PromotionPossible && prevBlock.GetPiece().GetPieceType() == PieceType.Pawn)
            {
                int promotionRow = (prevBlock.GetPiece().GetColor() == PieceColor.White) ? 1 : 6;

                if (Game.GetPlayerOne().GetColor() == PlayerColor.Black)
                    promotionRow = (prevBlock.GetPiece().GetColor() == PieceColor.White) ? 6 : 1;

                Console.WriteLine("Promotion possible.");
                Console.WriteLine(promotionRow);
                if (previousRow == promotionRow)
                {
                    PromotionPossible = false;
                    optionSelected = PromotePawn();
                    if (optionSelected == null) return;
                    optionSelected = optionSelected.ToLower();
                }
            }


            if (enPassantPossible && prevBlock.GetPiece().GetPieceType() == PieceType.Pawn)
            {
                if (FirstPlayerSelectedColorWhite && targetRow == 2) enPassantTargetRow = 3;
                else if (FirstPlayerSelectedColorWhite && targetRow == 5) enPassantTargetRow = 4;
                else if (!FirstPlayerSelectedColorWhite && targetRow == 2) enPassantTargetRow = 3;
                else if (!FirstPlayerSelectedColorWhite && targetRow == 5) enPassantTargetRow = 4;
            }

            if (castlingPossible)
            {
                if (Game.GetPlayerOne().GetColor() == PlayerColor.White && targetCol == 6) // short castle 
                {
                    castlingTargetFileRook = targetCol - 1;
                    rookCurrentFile = 7;
                }
                else if (Game.GetPlayerOne().GetColor() == PlayerColor.White && targetCol == 2) // long castle 
                {
                    castlingTargetFileRook = targetCol + 1;
                    rookCurrentFile = 0;
                }
                else if (Game.GetPlayerOne().GetColor() == PlayerColor.Black && targetCol == 1) // short castle 
                {
                    castlingTargetFileRook = targetCol + 1;
                    rookCurrentFile = 0;
                }
                else if (Game.GetPlayerOne().GetColor() == PlayerColor.Black && targetCol == 5) // long castle 
                {
                    castlingTargetFileRook = targetCol - 1;
                    rookCurrentFile = 7;
                }
            }

            Image rookImageForCastling = null;

            foreach (UIElement element in ChessGrid.Children)
            {
                if (Grid.GetRow(element) == previousRow && Grid.GetColumn(element) == previousCol && element is Image)
                {
                    pieceToMove = (Image)element;
                    break;
                }
            }

            if (castlingPossible)
            {
                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == previousRow && Grid.GetColumn(element) == rookCurrentFile && element is Image)
                    {
                        rookImageForCastling = (Image)element;
                        break;
                    }
                }
            }

            foreach (UIElement element in ChessGrid.Children)
            {
                if (enPassantPossible)
                {
                    if (Grid.GetRow(element) == enPassantTargetRow && Grid.GetColumn(element) == targetCol && element is Image)
                    {
                        enPassantPossible = false;
                        capturedPiece = (Image)element;
                        break;
                    }
                }
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
                }

                if (rookImageForCastling != null)
                {
                    ChessGrid.Children.Remove(rookImageForCastling);
                    Grid.SetColumn(rookImageForCastling, castlingTargetFileRook);
                    ChessGrid.Children.Add(rookImageForCastling);
                }

                ChessGrid.Children.Remove(pieceToMove);

                if (optionSelected != null)
                {
                    string imagePath = System.IO.Path.Combine("..\\..\\Images", $"{Game.GetCurrentPlayer().GetColor().ToString().ToLower()}-{optionSelected}.png");
                    imagePath = System.IO.Path.GetFullPath(imagePath);
                    pieceToMove = new Image
                    {
                        Width = 53,
                        Height = 53,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                        IsHitTestVisible = false,
                        Name = $"{optionSelected}"
                    };
                }

                Grid.SetRow(pieceToMove, targetRow);
                Grid.SetColumn(pieceToMove, targetCol);

                if (optionSelected != null) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Promotion, Game.GetPieceTypeByString(optionSelected));
                else if (enPassantTargetRow != -1) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.EnPassant, PieceType.Pawn, enPassantTargetRow);
                else if (capturedPiece != null) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Kill);
                else if (rookImageForCastling != null && (targetCol == 6 || targetCol == 1)) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Castling, PieceType.King, -1, CastlingType.KingSideCastle);
                else if (rookImageForCastling != null && (targetCol == 5 || targetCol == 2)) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Castling, PieceType.King, -1, CastlingType.QueenSideCastle);
                else Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Normal);
                ChessGrid.Children.Add(pieceToMove);
            }
            else
            {
                Console.WriteLine($"No piece found at Row: {previousRow}, Column: {previousCol}.");
            }
            if (enPassantPossible) enPassantPossible = false;
            if (PromotionPossible) PromotionPossible = false;
            if (IsMoving) IsMoving = false;
            if (castlingPossible) castlingPossible = false;
        }

        public string PromotePawn()
        {
            PromotionWindow promotionWindow = new PromotionWindow();
            if (promotionWindow.ShowDialog() == true)
            {
                return promotionWindow.SelectedPiece;
            }
            else
            {
                return null;
            }
        }

        private bool CanEnPassant(int targetRow, int targetCol)
        {
            //if (SelectedRow != targetRow && (targetCol != SelectedCol + 1 || targetCol != SelectedCol - 1)) return false;
            Block block = Game.GetBoard().GetBlock(targetRow, targetCol);
            if ((FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.White && targetRow == 2)
                || (FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.Black && targetRow == 5)
                || (!FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.White && targetRow == 5)
                || (!FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.Black && targetRow == 2))
            {
                Block blockToCheckForPawn;

                if (FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.White)
                    blockToCheckForPawn = Game.GetBoard().GetBlock(targetRow + 1, targetCol);
                else if (FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.Black)
                    blockToCheckForPawn = Game.GetBoard().GetBlock(targetRow - 1, targetCol);
                else if (!FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.White)
                    blockToCheckForPawn = Game.GetBoard().GetBlock(targetRow - 1, targetCol);
                else if (!FirstPlayerSelectedColorWhite && Game.GetCurrentPlayer().GetColor() == PlayerColor.Black)
                    blockToCheckForPawn = Game.GetBoard().GetBlock(targetRow + 1, targetCol);
                else
                    return false;

                if (SelectedPiece == null || SelectedPiece == blockToCheckForPawn?.GetPiece()) return false;

                if (block.IsEmpty()
                && (blockToCheckForPawn.GetPiece() != null
                && blockToCheckForPawn.GetPiece().GetPieceType() == PieceType.Pawn)
                && ((Pawn)blockToCheckForPawn.GetPiece()).GetEnPassantable())
                {
                    return true;
                }
            }
            return false;
        }
    }
}