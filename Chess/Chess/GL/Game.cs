using Chess.DS;
using System.Windows.Documents;
using System;

namespace Chess.GL
{
    public enum GameStatus
    {
        ACTIVE,
        BLACK_WIN,
        WHITE_WIN,
        FORFEIT,
        STALEMATE,
        RESIGNATION
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
        Castling
    }

    public enum CastlingType
    {
        ShortCastle,
        LongCastle,
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
            IsGameOver = false;
            Status = GameStatus.ACTIVE;
            CurrentMove = PlayerOne.GetColor().ToString().Trim() == "White" ? PlayerOne : PlayerTwo;
            Move.SetBoard(Board);
        }

        public void MakeMove(int prevRank, int prevFile, int newRank, int newFile, MoveType moveType, PieceType pieceType = PieceType.Queen, int enPassantTargetRow = -1)
        {
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

                // update move movement
                if (pieceAtPrev.GetPieceType() == PieceType.Pawn)
                {
                    Pawn pawn = (Pawn)prevBlock.GetPiece();
                    pawn.SetPawnMoved();
                    if (prevRank == 1 || prevRank == 6) pawn.PawnMoved(Math.Abs(prevRank - newRank));
                }
                //Console.WriteLine("Board Before");
                //Board.DisplayBoard();
                if (moveType == MoveType.EnPassant && Board.GetBlock(enPassantTargetRow, newFile) != null)
                {
                    // target row is the row where the pawn will move to, and 
                    // enpassassant target row is where the piece to be captured
                    Block targetBlock = Board.GetBlock(enPassantTargetRow, newFile);
                    Piece pieceAtTarget = targetBlock.GetPiece();
                    pieceAtTarget?.Kill();

                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtTarget);

                    if (CurrentMove == PlayerOne) PlayerTwo.KillPiece(pieceAtTarget);
                    else PlayerOne.KillPiece(pieceAtTarget);
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                    Board.GetBlock(enPassantTargetRow, newFile).SetPiece(null);
                }
                else if (Board.GetBlock(newRank, newFile).GetPiece() == null && moveType != MoveType.EnPassant) // if the target block is empty
                {
                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev);
                    if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        // here update piece at piece at prev to a new piece
                        pieceAtPrev = new Piece(pieceAtPrev.GetColor(), pieceType, true);
                    }
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else // if the target block is not empty (kill piece)
                {
                    Piece pieceAtNew = newBlock.GetPiece();
                    pieceAtNew?.Kill();

                    if(moveType != MoveType.Promotion) AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew);
                    else AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        if (pieceType == PieceType.Queen)
                           pieceAtPrev = new Queen(pieceAtPrev.GetColor(), pieceType, true);
                        else if (pieceType == PieceType.Bishop)
                            pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), pieceType, true);
                        else if (pieceType == PieceType.Rook)
                            pieceAtPrev = new Rook(pieceAtPrev.GetColor(), pieceType, true);
                        else if (pieceType == PieceType.Knight)
                            pieceAtPrev = new Knight(pieceAtPrev.GetColor(), pieceType, true);
                    };
                    if (CurrentMove == PlayerOne) PlayerTwo.KillPiece(pieceAtNew);
                    else PlayerOne.KillPiece(pieceAtNew);
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                //Console.WriteLine("Board After");
                //Board.DisplayBoard();
                if (CurrentMove.GetColor() == PlayerColor.Black)
                {
                    if (PlayerOne.GetColor() == PlayerColor.White)
                        Moves.Push(FirstPlayerMove, SecondPlayerMove);
                    else
                        Moves.Push(SecondPlayerMove, FirstPlayerMove);
                }
                if (CurrentMove == PlayerTwo)
                {
                    CurrentMove = PlayerOne;
                }
                else
                {
                    CurrentMove = PlayerTwo;
                }
                DisplayMoves();
                //PlayerOne.DisplayDeadPieces();
                //PlayerTwo.DisplayDeadPieces();
            }
        }

        public void AddMove(Block prevBlock, Block newBlock, MoveType moveType, Piece prevBlockPiece, Piece capturedPiece = null, PieceType promotedPieceType = PieceType.Queen)
        {
            // first check if the previous move was of pawn and it moved 2 blocks and player didn't kill it by en passant
            SetPawnUnEnPassantable();
            Move move;
            if (moveType == MoveType.Normal)
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece);
            }
            else if (moveType == MoveType.EnPassant) move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType);
            else move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType, promotedPieceType);

            if (CurrentMove == PlayerOne) FirstPlayerMove = move;
            else SecondPlayerMove = move;
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
                if(PlayerOne.GetColor() == PlayerColor.White)
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.Black ? 1 : 2;
                else
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.White ? 1 : 2;

                prevNotation = tokens[tokenForPlayer];
                if (prevNotation.Length != 2) return;
                int prevFile = Board.GetFileInInt(prevNotation[0].ToString());
                int prevRank = Board.TranslateRank(int.Parse(prevNotation[1].ToString()));
                if(PlayerOne.GetColor() == PlayerColor.Black)
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
    }
}
