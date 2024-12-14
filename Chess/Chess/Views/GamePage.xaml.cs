using Chess.GL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Chess.Views
{
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        bool FirstPlayerSelectedColorWhite;
        bool PromotionPossible = false;
        bool enPassantPossible = false;
        bool castlingPossible = false;
        bool isInCheck = false;
        bool IsMoving = false;

        int SelectedRow, SelectedCol;
        private int TimeControl = 1;

        Piece SelectedPiece;

        Game Game;

        public GamePage(PlayerColor firstPlayerColor, int timeControl, int difficulty = -1)
        {
            InitializeComponent();

            PlayerColor FirstPlayerColor, SecondPlayerColor;

            TimeControl = timeControl;

            if (firstPlayerColor == PlayerColor.White)
            {
                FirstPlayerColor = PlayerColor.White;
                SecondPlayerColor = PlayerColor.Black;
                FirstPlayerSelectedColorWhite = true;
            }
            else
            {
                FirstPlayerColor = PlayerColor.Black;
                SecondPlayerColor = PlayerColor.White;
                FirstPlayerSelectedColorWhite = false;
            }

            if (difficulty != -1)
                Game = Game.MakeGame(new Human(FirstPlayerColor), new Computer(SecondPlayerColor), difficulty);
            else
                Game = Game.MakeGame(new Human(FirstPlayerColor), new Human(SecondPlayerColor));

            //if (FirstPlayerSelectedColorWhite)
            //{
            //    FirstPlayerColor = PlayerColor.White;
            //    SecondPlayerColor = PlayerColor.Black;
            //}
            //else
            //{
            //    FirstPlayerColor = PlayerColor.Black;
            //    SecondPlayerColor = PlayerColor.White;
            //}

            //Game = Game.MakeGame(new Human(FirstPlayerColor), new Human(SecondPlayerColor));

            InitializeBoard();

            DataContext = this;
            Moves = new ObservableCollection<string>();
            PlayerOneDeadPieces = new ObservableCollection<string>();
            PlayerTwoDeadPieces = new ObservableCollection<string>();

            Game.MoveMade += HandlesMovesChanged;
            Game.PlayerOneDeadPiecesChanged += HandlePlayerOneDeadPiecesChanged;
            Game.PlayerTwoDeadPiecesChanged += HandlePlayerTwoDeadPiecesChanged;

            PlayerOneTimeTextBox.Text = FirstPlayerColor.ToString() + "'s Time";
            PlayerOneDeadPiecesTextBox.Text = FirstPlayerColor.ToString() + "'s Dead Pieces";
            PlayerOneDeadPiecesTextBox.HorizontalAlignment = HorizontalAlignment.Center;

            PlayerTwoTimeTextBox.Text = SecondPlayerColor.ToString() + "'s Time";
            PlayerTwoDeadPiecesTextBox.Text = SecondPlayerColor.ToString() + "'s Dead Pieces";
            PlayerTwoDeadPiecesTextBox.HorizontalAlignment = HorizontalAlignment.Center;

            if (TimeControl < 10)
            {
                PlayerOneTimeControl.Text = $"0{TimeControl}:00";
                PlayerTwoTimeControl.Text = $"0{TimeControl}:00";
            }
            else
            {
                PlayerOneTimeControl.Text = $"{TimeControl}:00";
                PlayerTwoTimeControl.Text = $"{TimeControl}:00";
            }

            _playerOneTime = TimeSpan.FromMinutes(TimeControl);
            _playerTwoTime = TimeSpan.FromMinutes(TimeControl);

            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Tick every second
            };

            _countdownTimer.Tick += CountdownTimer_Tick;

            UpdateTimeDisplays();

            DisplayComputerMove(); // if computer is first
        }

        // UI Functions
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

        private void HighlightSquares(int row, int col, Brush brush = null)
        {
            if (brush == null)
            {
                brush = Brushes.LightGreen;
            }
            Border border = new Border
            {
                Background = brush,
                Opacity = 0.5
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            ChessGrid.Children.Add(border);
        }

        private void RemoveHighlights(Brush brush = null)
        {
            if (brush == null)
            {
                brush = Brushes.LightGreen;
            }
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement element in ChessGrid.Children)
            {
                if (element is Border border && border.Background == brush)
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
            if (Game.GetCurrentPlayer().GetPlayerType() == PlayerType.Computer)
                return true;

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

        private Image GetImage(string color, string piece)
        {
            Image image = null;
            string imagePath = System.IO.Path.Combine("..\\..\\Images", $"{color}-{piece}.png");
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
            return image;
        }

        private void ScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            movesViewer.ScrollToBottom();
        }

        // Move Handling Functions
        private void Chessboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = sender as Border;
            if (clickedElement != null && !Game.GetIsGameOver())
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
                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == SelectedRow && Grid.GetColumn(element) == SelectedCol && element is Image)
                    {
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
            }
        }

        private void MakeMove(int previousRow, int previousCol, int targetRow, int targetCol)
        {
            if (!IsValidMove(targetRow, targetCol)) return;

            PieceColor currentPieceColor = Game.GetCurrentPlayer().GetColor() == PlayerColor.White ? PieceColor.White : PieceColor.Black;

            if (isInCheck && Game.GetBoard().IsKingInCheck(currentPieceColor))
            {
                Block kingBlock = Game.GetBoard().FindKing(currentPieceColor);
                isInCheck = false;
                RemoveHighlights(Brushes.Red);
            }

            Image pieceToMove = null;
            Image capturedPiece = null;

            Block prevBlock = Game.GetBoard().GetBlock(previousRow, previousCol);
            Block targetBlock = Game.GetBoard().GetBlock(targetRow, targetCol);

            string optionSelected = null;
            int enPassantTargetRow = -1;
            int castlingTargetFileRook = -1, rookCurrentFile = -1;

            if (targetBlock.GetPiece() != null && targetBlock.GetPiece().GetColor() == prevBlock.GetPiece().GetColor() && Game.GetCurrentPlayer().GetPlayerType() != PlayerType.Computer)
            {
                return;
            }

            if (PromotionPossible && prevBlock.GetPiece().GetPieceType() == PieceType.Pawn)
            {
                int promotionRow = (prevBlock.GetPiece().GetColor() == PieceColor.White) ? 1 : 6;

                if (Game.GetPlayerOne().GetColor() == PlayerColor.Black)
                    promotionRow = (prevBlock.GetPiece().GetColor() == PieceColor.White) ? 6 : 1;

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

                if (optionSelected != null) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Promotion, UtilityFunctions.GetPieceTypeByString(optionSelected));
                else if (enPassantTargetRow != -1) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.EnPassant, PieceType.Pawn, enPassantTargetRow);
                else if (capturedPiece != null) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Kill);
                else if (rookImageForCastling != null && (targetCol == 6 || targetCol == 1)) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Castling, PieceType.King, -1, CastlingType.KingSideCastle);
                else if (rookImageForCastling != null && (targetCol == 5 || targetCol == 2)) Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Castling, PieceType.King, -1, CastlingType.QueenSideCastle);
                else Game.MakeMove(previousRow, previousCol, targetRow, targetCol, MoveType.Normal);
                ChessGrid.Children.Add(pieceToMove);
            }
            else
            {
                // Console.WriteLine($"No piece found at Row: {previousRow}, Column: {previousCol}.");
            }

            PieceColor opponentPieceColor = Game.GetCurrentPlayer().GetColor() == PlayerColor.White ? PieceColor.White : PieceColor.Black;
            if (Game.GetBoard().IsKingInCheck(opponentPieceColor))
            {
                Block kingBlock = Game.GetBoard().FindKing(opponentPieceColor);
                isInCheck = true;
                HighlightSquares(kingBlock.GetRank(), kingBlock.GetFile(), Brushes.Red);
            }

            if (enPassantPossible) enPassantPossible = false;
            if (PromotionPossible) PromotionPossible = false;
            if (IsMoving) IsMoving = false;
            if (castlingPossible) castlingPossible = false;


            if (Game.GetCurrentPlayer() == Game.GetPlayerOne())
            {
                StartPlayerOneTurn();
            }
            else
            {
                StartPlayerTwoTurn();
            }

            GetGameStatus();

            DisplayComputerMove();

            if (Game.CheckDraw())
            {
                Draw();
                NavigateToHomePage();
            }
        }

        public void DisplayComputerMove()
        {
            if (Game.GetCurrentPlayer().GetPlayerType() == PlayerType.Computer)
            {
                Move move = Game.MakeComputerMove();
                if (move != null)
                {
                    MakeMove(move.GetStartBlock().GetRank(), move.GetStartBlock().GetFile(), move.GetEndBlock().GetRank(), move.GetEndBlock().GetFile());
                }
            }
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

        private void ResignClick(object sender, RoutedEventArgs e)
        {
            if (Game.GetIsGameOver()) return;
            var result = MessageBox.Show("Are you sure you want to resign?", "Resign", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            Game.SetIsGameOver(true);
            Player winner = Game.GetCurrentPlayer() == Game.GetPlayerOne() ? Game.GetPlayerTwo() : Game.GetPlayerOne();
            MessageBox.Show($"{Game.GetCurrentPlayer().GetColor().ToString()} has resigned. {winner.GetColor().ToString()} wins!", "Resign", MessageBoxButton.OK, MessageBoxImage.Information);
            Game.UpdateStatus(GameStatus.RESIGNATION);
            NavigateToHomePage();
        }

        private void DrawClick(object sender, RoutedEventArgs e)
        {
            if (Game.GetIsGameOver()) return;

            Player nextPlayer = Game.GetCurrentPlayer() == Game.GetPlayerOne() ? Game.GetPlayerTwo() : Game.GetPlayerOne();
            var result = MessageBox.Show($"{Game.GetCurrentPlayer().GetColor().ToString()} has offered a draw. Do you want to accept?",
                                  "Draw Offer", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Draw();
                NavigateToHomePage();
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GetGameStatus()
        {
            if (Game.GetIsGameOver())
            {
                if (Game.GetStatus() == GameStatus.BLACK_WIN || Game.GetStatus() == GameStatus.WHITE_WIN)
                {
                    Player winner = Game.GetCurrentPlayer() == Game.GetPlayerOne() ? Game.GetPlayerTwo() : Game.GetPlayerOne();
                    RemoveHighlights();
                    DisplayLoseMessage(Game.GetCurrentPlayer().GetColor());
                    NavigateToHomePage();
                }
                else if (Game.GetStatus() == GameStatus.STALEMATE)
                {
                    RemoveHighlights();
                    MessageBox.Show("Game is a draw by stalemate!", "Game Draw", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigateToHomePage();
                }
            }
        }

        private void UndoClick(object sender, RoutedEventArgs e)
        {
            if (Game.GetMovesStack().GetSize() > 0)
            {
                Move move = Game.GetMovesStack().Peek();
                if (move != null)
                {
                    Piece killedPiece = null;
                    if (move.GetNotation().Contains("x"))
                        killedPiece = move.GetPieceKilled();
                    UndoMove(move.GetEndBlock(), move.GetStartBlock(), move.GetPieceMoved(), killedPiece, move.GetMoveType(), move.GetCastlingType());
                    Game.UndoMove(move);
                    RemoveHighlights();
                    // if after undoing the move, player is in check
                    if (Game.GetBoard().IsKingInCheck(move.GetPieceMoved().GetColor()))
                    {
                        Block kingBlock = Game.GetBoard().FindKing(move.GetPieceMoved().GetColor());
                        isInCheck = true;
                        HighlightSquares(kingBlock.GetRank(), kingBlock.GetFile(), Brushes.Red);
                    }
                    else
                    {
                        isInCheck = false;
                        RemoveHighlights(Brushes.Red);
                    }
                }
            }
        }

        private void UndoMove(Block endBlock, Block startBlock, Piece piece, Piece pieceKilled, MoveType moveType, CastlingType castlingType)
        {
            int prevRank = endBlock.GetRank();
            int prevFile = endBlock.GetFile();
            int targetRank = startBlock.GetRank();
            int targetFile = startBlock.GetFile();

            int rookTargetFile = -1;
            int rookCurrentFile = -1;

            Image pieceToMove = null;
            Image capturedPiece = null;
            Image promotedPiece = null;
            Image rookImage = null;

            if (pieceKilled != null)
                capturedPiece = GetImage(pieceKilled.GetColor().ToString().ToLower(), pieceKilled.GetPieceType().ToString().ToLower());

            if (moveType == MoveType.EnPassant)
            {
                capturedPiece = GetImage((pieceKilled.GetColor() == PieceColor.White ? PieceColor.White : PieceColor.Black).ToString().ToLower(), "pawn");
                if (capturedPiece != null)
                {
                    Grid.SetRow(capturedPiece, targetRank);
                    Grid.SetColumn(capturedPiece, prevFile);
                    ChessGrid.Children.Add(capturedPiece);
                }
            }

            if (moveType == MoveType.Castling)
            {
                if (castlingType == CastlingType.KingSideCastle && FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 5;
                    rookTargetFile = 7;
                }
                else if (castlingType == CastlingType.KingSideCastle && !FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 2;
                    rookTargetFile = 0;
                }
                else if (castlingType == CastlingType.QueenSideCastle && FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 3;
                    rookTargetFile = 0;
                }
                else if (castlingType == CastlingType.QueenSideCastle && !FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 4;
                    rookTargetFile = 7;
                }

                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == prevRank && Grid.GetColumn(element) == rookCurrentFile && element is Image)
                    {
                        pieceToMove = (Image)element;
                        if (moveType == MoveType.Castling)
                        {
                            rookImage = (Image)element;
                        }
                        break;
                    }
                }
                if (rookImage != null)
                {
                    ChessGrid.Children.Remove(rookImage);
                    Grid.SetColumn(rookImage, rookTargetFile);
                    Grid.SetRow(rookImage, prevRank);
                    ChessGrid.Children.Add(rookImage);
                }
            }

            if (piece != null)
            {
                foreach (UIElement element in ChessGrid.Children)
                {
                    if (Grid.GetRow(element) == prevRank && Grid.GetColumn(element) == prevFile && element is Image)
                    {
                        pieceToMove = (Image)element;
                        if (moveType == MoveType.Promotion || moveType == MoveType.PromotionCheck)
                        {
                            if (moveType == MoveType.PromotionCheck)
                                RemoveHighlights(Brushes.Red);
                            promotedPiece = GetImage((endBlock.GetPiece().GetColor() == PieceColor.White ? PieceColor.White : PieceColor.Black).ToString().ToLower(), "pawn");
                        }
                        break;
                    }
                }

                if (pieceToMove != null)
                {
                    ChessGrid.Children.Remove(pieceToMove);
                    if (promotedPiece != null && (moveType == MoveType.Promotion || moveType == MoveType.PromotionCheck))
                    {
                        Grid.SetRow(promotedPiece, targetRank);
                        Grid.SetColumn(promotedPiece, targetFile);
                        ChessGrid.Children.Add(promotedPiece);
                    }
                    else
                    {
                        Grid.SetRow(pieceToMove, targetRank);
                        Grid.SetColumn(pieceToMove, targetFile);
                        ChessGrid.Children.Add(pieceToMove);
                    }
                    if (capturedPiece != null && moveType != MoveType.EnPassant)
                    {
                        Grid.SetRow(capturedPiece, prevRank);
                        Grid.SetColumn(capturedPiece, prevFile);
                        ChessGrid.Children.Add(capturedPiece);
                    }
                }
            }
        }

        // Game status related functions
        public void Draw()
        {
            Game.MakeMove(-1, -1, -1, -1, MoveType.Draw);
            Game.SetIsGameOver(true);
            MessageBox.Show("Game is a draw!", "Game Draw", MessageBoxButton.OK, MessageBoxImage.Information);
            Game.UpdateStatus(GameStatus.DRAW);
            RemoveHighlights();
        }

        public void DisplayLoseMessage(PlayerColor playerColor, string reason = " by Checkmate!")
        {
            Game.SetIsGameOver(true);
            MessageBox.Show($"{playerColor} lost{reason}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            RemoveHighlights();
        }

        private ObservableCollection<string> _moves;
        private ObservableCollection<string> _playerOneDeadPieces;
        private ObservableCollection<string> _playerTwoDeadPieces;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public ObservableCollection<string> PlayerOneDeadPieces
        {
            get => _playerOneDeadPieces;
            set
            {
                if (_playerOneDeadPieces != value)
                {
                    _playerOneDeadPieces = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> PlayerTwoDeadPieces
        {
            get => _playerTwoDeadPieces;
            set
            {
                if (_playerTwoDeadPieces != value)
                {
                    _playerTwoDeadPieces = value;
                    OnPropertyChanged();
                }
            }
        }

        private void HandlesMovesChanged(string move, bool isAdd)
        {
            if (isAdd)
            {
                Moves.Add(move);
            }
            else // remove the move which contains this exact text
            {
                if (Moves.Count > 0)
                {
                    string lastMove = Moves[Moves.Count - 1];
                    string[] tokens = lastMove.Split(' ');
                    if (tokens[1] == move || tokens[2] == move)
                    {
                        Moves.RemoveAt(Moves.Count - 1);
                    }
                }
            }
        }

        private void HandlePlayerOneDeadPiecesChanged(string piece, bool isAdd)
        {
            if (isAdd)
            {
                PlayerOneDeadPieces.Add(piece);
            }
            else
            {
                PlayerOneDeadPieces.Remove(piece);
            }
        }

        private void HandlePlayerTwoDeadPiecesChanged(string piece, bool isAdd)
        {
            if (isAdd)
            {
                PlayerTwoDeadPieces.Add(piece);
            }
            else
            {
                PlayerTwoDeadPieces.Remove(piece);
            }
        }

        // Timer functionality
        private DispatcherTimer _countdownTimer;
        private TimeSpan _playerOneTime;
        private TimeSpan _playerTwoTime;
        private bool _isPlayerOneTurn = true;

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (_isPlayerOneTurn)
            {
                _playerOneTime = _playerOneTime.Subtract(TimeSpan.FromSeconds(1));
                if (_playerOneTime.TotalSeconds <= 0)
                {
                    _countdownTimer.Stop();
                    PlayerOneTimeTextBox.Text = "Time's Up!";
                    DisplayLoseMessage(FirstPlayerSelectedColorWhite ? PlayerColor.White : PlayerColor.Black, " on Time!!");
                    return;
                }
            }
            else
            {
                _playerTwoTime = _playerTwoTime.Subtract(TimeSpan.FromSeconds(1));
                if (_playerTwoTime.TotalSeconds <= 0)
                {
                    _countdownTimer.Stop();
                    PlayerTwoTimeTextBox.Text = "Time's Up!";
                    DisplayLoseMessage(FirstPlayerSelectedColorWhite ? PlayerColor.Black : PlayerColor.White, " on Time!!");
                    return;
                }
            }

            UpdateTimeDisplays();
        }

        private void UpdateTimeDisplays()
        {
            PlayerOneTimeControl.Text = _playerOneTime.ToString(@"mm\:ss");
            PlayerTwoTimeControl.Text = _playerTwoTime.ToString(@"mm\:ss");
        }

        public void StartPlayerOneTurn()
        {
            _isPlayerOneTurn = true;
            _countdownTimer.Start();
        }

        public void StartPlayerTwoTurn()
        {
            _isPlayerOneTurn = false;
            _countdownTimer.Start();
        }

        public void PauseTimer()
        {
            _countdownTimer.Stop();
        }

        public void ResumeTimer()
        {
            _countdownTimer.Start();
        }

        public void NavigateToHomePage()
        {
            _countdownTimer.Stop();
            NavigationService.Navigate(new Homepage());
        }
    }
}