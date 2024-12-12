using Chess.DS;
using System.Windows.Documents;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Chess.GL
{
    public enum GameStatus
    {
        ACTIVE,
        BLACK_WIN,
        WHITE_WIN,
        FORFEIT,
        STALEMATE,
        RESIGNATION,
        DRAW
    }

    public enum MoveType
    {
        Normal,
        Kill,
        Check,
        Checkmate,
        Stalemate,
        Promotion,
        EnPassant,
        Castling,
        PromotionCheck,
        Draw
    }

    public enum CastlingType
    {
        KingSideCastle,
        QueenSideCastle,
        None
    }

    public class Game
    {
        private Player PlayerOne;
        private Player PlayerTwo;
        private Player CurrentMove;
        private Board Board;
        private Stack Moves;
        private Move FirstPlayerMove;
        private Move SecondPlayerMove;
        private bool IsGameOver;
        private GameStatus Status;
        private MovesStack MovesStack;

        public event Action<string> MoveMade;
        // dead pieces
        public event Action<string> PlayerOneDeadPieces;
        public event Action<string> PlayerTwoDeadPieces;

        private static Game GameInstance;

        public static Game MakeGame(Player playerOne, Player playerTwo) //Singleton Pattern
        {
            if (GameInstance == null)
            {
                GameInstance = new Game(playerOne, playerTwo);
            }
            return GameInstance;
        }

        private Game(Player playerOne, Player playerTwo)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            Board = new Board(playerOne.GetColor());
            Moves = new Stack();
            MovesStack = new MovesStack();
            IsGameOver = false;
            Status = GameStatus.ACTIVE;
            CurrentMove = PlayerOne.GetColor().ToString().Trim() == "White" ? PlayerOne : PlayerTwo;
            Move.SetBoard(Board);
        }

        public void MakeMove(int prevRank, int prevFile, int newRank, int newFile, MoveType moveType, PieceType pieceType = PieceType.Queen, int enPassantTargetRow = -1, CastlingType castlingType = CastlingType.None)
        {
            if (moveType == MoveType.Draw)
            {
                Move move = new Move(moveType);
                move.SetNotation("1/2");
                Moves.Push(move, move);
                MoveMade?.Invoke(Moves.Peek());
                MovesStack.Push(move);
                return;
            }

            if (!IsGameOver && Status == GameStatus.ACTIVE && Board.WithinBounds(prevRank, prevFile) && Board.WithinBounds(newRank, newFile))
            {
                if(Board.GetBlock(prevRank, prevFile).IsEmpty())
                {
                    Console.WriteLine("No piece at the given position.");
                    return;
                }
                Block prevBlock = Board.GetBlock(prevRank, prevFile);
                Piece pieceAtPrev = prevBlock.GetPiece();
                Block newBlock = Board.GetBlock(newRank, newFile);

                if(newBlock.GetPiece() != null && newBlock.GetPiece().GetColor() == pieceAtPrev.GetColor())
                {
                    Console.WriteLine("Cannot place pieces of same color on eachother.");
                    return;
                }
                if (pieceAtPrev.GetColor().ToString().Trim() != CurrentMove.GetColor().ToString().Trim())
                {
                    Console.WriteLine("It is not your turn.");
                    return;
                }


                if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn)
                {
                    CheckForPromotionCheck(prevBlock, newBlock, pieceAtPrev, ref moveType,ref pieceType);
                }

                // update move movement
                if (pieceAtPrev.GetPieceType() == PieceType.Pawn)
                {
                    Pawn pawn = (Pawn)prevBlock.GetPiece();
                    pawn.SetPawnMoved();
                    if (prevRank == 1 || prevRank == 6) 
                        pawn.PawnMoved(Math.Abs(prevRank - newRank));
                }
                else
                {
                    if (pieceAtPrev.GetPieceType() == PieceType.Rook)
                    {
                        Rook rook = (Rook)prevBlock.GetPiece();
                        rook.SetHasMoved();
                    }
                    else if (pieceAtPrev.GetPieceType() == PieceType.King)
                    {
                        King king = (King)prevBlock.GetPiece();
                        king.SetHasMoved();
                    }
                }

                if (Board.IsCheck(pieceAtPrev, newBlock))
                {
                    moveType = MoveType.Check;
                    King king = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.White ? PieceColor.Black : PieceColor.White).GetPiece();
                    king.SetCheck(true);
                }
                else
                {
                    King king = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.Black : PieceColor.White).GetPiece();
                    king.SetCheck(false);
                }

                //Console.WriteLine("Board Before");
                //Board.DisplayBoard();
                if (moveType == MoveType.EnPassant && Board.GetBlock(enPassantTargetRow, newFile) != null)
                {
                    Block targetBlock = Board.GetBlock(enPassantTargetRow, newFile);
                    Piece pieceAtTarget = targetBlock.GetPiece();
                    pieceAtTarget?.Kill();

                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtTarget);

                    if (CurrentMove == PlayerOne)
                    {
                        PlayerTwo.KillPiece(pieceAtTarget);
                        PlayerTwoDeadPieces.Invoke(PlayerTwo.GetFirstDeadPiece());
                    }
                    else
                    {
                        PlayerOne.KillPiece(pieceAtTarget);
                        PlayerOneDeadPieces.Invoke(PlayerOne.GetFirstDeadPiece());
                    }
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                    Board.GetBlock(enPassantTargetRow, newFile).SetPiece(null);
                }
                else if (moveType == MoveType.Castling)
                {
                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev);

                    int rank = (CurrentMove.GetColor() == PlayerColor.White) ? 7 : 0; 
                    int kingStartFile = prevFile;
                    int rookStartFile = (castlingType == CastlingType.KingSideCastle) ? 7 : 0;
                    int kingEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile + 2 : kingStartFile - 2;
                    int rookEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile + 1 : kingStartFile - 1;

                    if(PlayerOne.GetColor() == PlayerColor.Black)
                    {
                        rank = (CurrentMove.GetColor() == PlayerColor.White) ? 0 : 7;
                        rookStartFile = (castlingType == CastlingType.KingSideCastle) ? 0 : 7;
                        kingEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile - 2 : kingStartFile + 2;
                        rookEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile - 1 : kingStartFile + 1;
                    }

                    if(!Board.WithinBounds(kingStartFile, kingEndFile) || !Board.WithinBounds(rookStartFile, rookEndFile))
                    {
                        Console.WriteLine("Invalid castling move");
                        return;
                    }

                    Board.GetBlock(rank, rookEndFile).SetPiece(Board.GetBlock(rank, rookStartFile).GetPiece());
                    Board.GetBlock(rank, rookStartFile).SetPiece(null);
                    Board.GetBlock(rank, kingEndFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else if (moveType == MoveType.PromotionCheck)
                {
                    Piece pieceAtNew = newBlock.GetPiece();

                    if (pieceAtNew?.GetPieceType() != PieceType.King)
                        pieceAtNew?.Kill();

                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    if (pieceType == PieceType.Queen)
                        pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                    else if (pieceType == PieceType.Bishop)
                        pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                    else if (pieceType == PieceType.Rook)
                        pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                    else if (pieceType == PieceType.Knight)
                        pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);

                    if (CurrentMove == PlayerOne && pieceAtNew != null)
                    {
                        PlayerTwo.KillPiece(pieceAtNew);
                        PlayerTwoDeadPieces.Invoke(PlayerTwo.GetFirstDeadPiece());
                    }
                    else if (CurrentMove == PlayerTwo && pieceAtNew != null)
                    {
                        PlayerOne.KillPiece(pieceAtNew);
                        PlayerOneDeadPieces.Invoke(PlayerOne.GetFirstDeadPiece());
                    }

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else if (newBlock.GetPiece() == null && moveType != MoveType.EnPassant) // if the target block is empty
                {
                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev);

                    if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        pieceAtPrev = new Piece(pieceAtPrev.GetColor(), pieceType, true);
                    }

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else // if the target block is not empty (kill piece)
                {
                    Piece pieceAtNew = newBlock.GetPiece();

                    if (pieceAtNew?.GetPieceType() != PieceType.King)
                        pieceAtNew.Kill();

                    if(moveType != MoveType.Promotion) 
                        AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew);
                    else 
                        AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    if ((moveType == MoveType.Promotion) && (prevBlock?.GetPiece().GetPieceType() == PieceType.Pawn 
                        && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) 
                        || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.White)
                        || prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.Black)
                        || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        if (pieceType == PieceType.Queen)
                            pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                        else if (pieceType == PieceType.Bishop)
                            pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                        else if (pieceType == PieceType.Rook)
                            pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                        else if (pieceType == PieceType.Knight)
                            pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);    
                    };

                    if (CurrentMove == PlayerOne)
                    {
                        PlayerTwo.KillPiece(pieceAtNew);
                        Console.WriteLine(PlayerTwo.GetFirstDeadPiece());
                        PlayerTwoDeadPieces.Invoke(PlayerTwo.GetFirstDeadPiece());
                    }
                    else
                    {
                        PlayerOne.KillPiece(pieceAtNew);
                        PlayerOneDeadPieces.Invoke(PlayerOne.GetFirstDeadPiece());
                    }

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                //Console.WriteLine("Board After");
                //Board.DisplayBoard();
                if (CurrentMove.GetColor() == PlayerColor.White)
                {
                    if ((moveType == MoveType.Check || moveType == MoveType.PromotionCheck) && Board.GetFinalStatus(PieceColor.Black)) // Scan for Checkmate
                    {
                        moveType = MoveType.Checkmate;
                        Status = GameStatus.WHITE_WIN;
                        IsGameOver = true;
                    }
                    else if(Board.GetFinalStatus(PieceColor.White)) // Scan for Stalemate
                    {
                        moveType = MoveType.Stalemate;
                        Status = GameStatus.STALEMATE;
                        IsGameOver = true;
                    }
                }
                else
                {
                    if ((moveType == MoveType.Check || moveType == MoveType.PromotionCheck) && Board.GetFinalStatus(PieceColor.White)) // Scan for Checkmate
                    {
                        moveType = MoveType.Checkmate;
                        Status = GameStatus.BLACK_WIN;
                        IsGameOver = true;
                    }
                    else if (Board.GetFinalStatus(PieceColor.Black)) // Scan for Stalemate
                    {
                        moveType = MoveType.Stalemate;
                        Status = GameStatus.STALEMATE;
                        IsGameOver = true;
                    }
                }

                if (CurrentMove.GetColor() == PlayerColor.Black)
                {
                    if (PlayerOne.GetColor() == PlayerColor.White)
                    {
                        if(moveType == MoveType.Checkmate)
                        {
                            SecondPlayerMove.SetMoveType(MoveType.Checkmate);
                            SecondPlayerMove.SetNotation(SecondPlayerMove.GetNotation().Replace("+", "#"));
                        }
                        Moves.Push(FirstPlayerMove, SecondPlayerMove);
                        OnMoveMade(Moves.Peek());
                    }
                    else
                    {
                        if (moveType == MoveType.Checkmate)
                        {
                            FirstPlayerMove.SetMoveType(MoveType.Checkmate);
                            FirstPlayerMove.SetNotation(FirstPlayerMove.GetNotation().Replace("+", "#"));
                        }
                        Moves.Push(SecondPlayerMove, FirstPlayerMove);
                        OnMoveMade(Moves.Peek()); 
                    }
                }

                if(CurrentMove.GetColor() == PlayerColor.White && PlayerOne.GetColor() == PlayerColor.White && moveType == MoveType.Checkmate)
                {
                    FirstPlayerMove.SetMoveType(MoveType.Checkmate);
                    FirstPlayerMove.SetNotation(FirstPlayerMove.GetNotation().Replace("+", "#"));
                    Moves.Push(FirstPlayerMove);
                    OnMoveMade(Moves.Peek());   
                }

                if (CurrentMove == PlayerTwo)
                {
                    CurrentMove = PlayerOne;
                    MovesStack.Push(SecondPlayerMove);
                }
                else
                {
                    CurrentMove = PlayerTwo;
                    MovesStack.Push(FirstPlayerMove);
                }
                DisplayMoves();
                MovesStack.Display();
                //PlayerOne.DisplayDeadPieces();
                //PlayerTwo.DisplayDeadPieces();
            }
        }

        public void CheckForPromotionCheck(Block prevBlock, Block newBlock, Piece pieceAtPrev, ref MoveType moveType,ref PieceType pieceType)
        {
            Piece pieceAtNew = newBlock.GetPiece(); 
            Pawn pawn = (Pawn)prevBlock.GetPiece();
            if  (prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White
                || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.White
                || prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.Black
                || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)
            {
                if (pieceType == PieceType.Queen)
                    pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                else if (pieceType == PieceType.Bishop)
                    pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                else if (pieceType == PieceType.Rook)
                    pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                else if (pieceType == PieceType.Knight)
                    pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);
                else
                    return;
            }

            Board.GetBlock(newBlock.GetRank(), newBlock.GetFile()).SetPiece(pieceAtPrev);
            Board.GetBlock(prevBlock.GetRank(), prevBlock.GetFile()).SetPiece(null);

            PieceColor opponentKingColor = CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.White : PieceColor.Black;
            King king = (King)Board.FindKing(opponentKingColor).GetPiece();
            if (Board.IsKingInCheck(opponentKingColor))
            {
                moveType = MoveType.PromotionCheck;
                king.SetCheck(true);
            }
            else
            {
                king.SetCheck(false);
            }

            // reset board
            Board.GetBlock(newBlock.GetRank(), newBlock.GetFile()).SetPiece(pieceAtNew);
            Board.GetBlock(prevBlock.GetRank(), prevBlock.GetFile()).SetPiece(pawn);
        }

        public void AddMove(Block prevBlock, Block newBlock, MoveType moveType, Piece prevBlockPiece, Piece capturedPiece = null, PieceType promotedPieceType = PieceType.Queen)
        {
            // first check if the previous move was of pawn and it moved 2 blocks and player didn't kill it by en passant
            SetPawnUnEnPassantable();
            Move move;
            if (moveType == MoveType.Normal || moveType == MoveType.Check)
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType);
            }
            else if (moveType == MoveType.Castling)
            {
                CastlingType castlingType = CastlingType.None;

                if (newBlock.GetFile() == 6 || newBlock.GetFile() == 1) 
                    castlingType = CastlingType.KingSideCastle;
                else if (newBlock.GetFile() == 2 || newBlock.GetFile() == 5) 
                    castlingType = CastlingType.QueenSideCastle;

                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, castlingType);
            }
            else if (moveType == MoveType.EnPassant)
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType);
            }
            else
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType, promotedPieceType);
            }

            if (CurrentMove == PlayerOne) 
                FirstPlayerMove = move;
            else 
                SecondPlayerMove = move;
        }

        private void SetPawnUnEnPassantable()
        {
            if (Moves.GetSize() == 0) return;
            string lastMove = Moves.Peek();
            string[] tokens = lastMove.Split(' ');
            string prevNotation;
            int tokenForPlayer = -1;
            if (tokens.Length == 3)
            {
                if (PlayerOne.GetColor() == PlayerColor.White)
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.Black ? 1 : 2;
                else
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.White ? 2 : 1;

                prevNotation = tokens[tokenForPlayer];
                if (prevNotation.Length != 2) return;
                int prevFile = Board.GetFileInInt(prevNotation[0].ToString());
                int prevRank = Board.TranslateRank(int.Parse(prevNotation[1].ToString()));
                if (PlayerOne.GetColor() == PlayerColor.Black)
                {
                    prevRank = int.Parse(prevNotation[1].ToString()) - 1;
                    prevFile = ReverseBlockValue(prevFile);
                }
                if (!Board.WithinBounds(prevRank, prevFile)) return;
                Block block = Board.GetBlock(prevRank, prevFile);
                if (!block.IsEmpty() && block.GetPiece().GetPieceType() == PieceType.Pawn)
                {
                    Pawn piece = (Pawn)block.GetPiece();
                    if(piece.GetEnPassantable())
                    {
                        piece.SetUnEnPassantable();
                    }
                }
            }
        }

        public void DisplayMoves()
        {
            Moves.Display();
        }

        public Player GetPlayerOne()
        {
            return PlayerOne;
        }

        public Player GetPlayerTwo()
        {
            return PlayerTwo;
        }

        public Board GetBoard()
        {
            return Board;
        }

        public Stack GetMoves()
        {
            return Moves;
        }

        public bool GetIsGameOver()
        {
            return IsGameOver;
        }

        public GameStatus GetStatus()
        {
            return Status;
        }

        public Player GetCurrentPlayer()
        {
            return CurrentMove;
        }

        public void UpdateStatus(GameStatus status)
        {
            Status = status;
        }

        public void SetIsGameOver(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }

        public bool IsTurn(string pieceColor)
        {
            return CurrentMove.GetColor().ToString().Trim() == pieceColor.Trim();
        }

        public static PieceType GetPieceTypeByString(string pieceName)
        {
            switch(pieceName)
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

        public int ReverseBlockValue(int value)
        {
            switch(value)
            {
                case 0:
                    return 7;
                case 1:
                    return 6;
                case 2:
                    return 5;
                case 3:
                    return 4;
                case 4:
                    return 3;
                case 5:
                    return 2;
                case 6:
                    return 1;
                case 7:
                    return 0;
                default:
                    return -1;
            }
        }

        private void OnMoveMade(string move)
        {
            MoveMade?.Invoke(move);
        }

        public MovesStack GetMovesStack()
        {
            return MovesStack;
        }

        public void SetNextPlayer()
        {
            if (CurrentMove == PlayerOne)
            {
                CurrentMove = PlayerTwo;
            }
            else
            {
                CurrentMove = PlayerOne;
            }
        }
    }
}
