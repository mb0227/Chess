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
    public class Game
    {
        private Player PlayerOne;
        private Player PlayerTwo;
        private Player CurrentMove;
        private Board Board;
        private Stack Moves;
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

        public void MakeMove(int prevRank, int prevFile, int newRank, int newFile)
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
                Console.WriteLine("Board Before");
                Board.DisplayBoard();
                if (Board.GetBlock(newRank, newFile).GetPiece() == null)
                {
                    if(pieceAtPrev.GetPieceType() == PieceType.Pawn && (prevRank == 1 || prevRank == 6))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        pawn.SetHasMoved();
                    }
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                    Moves.Push(new Move(prevBlock, newBlock, pieceAtPrev, null));
                    DisplayMoves();
                }
                else
                {
                    Piece pieceAtNew = newBlock.GetPiece();
                    pieceAtNew.Kill();
                    if(CurrentMove == PlayerOne) PlayerTwo.KillPiece(pieceAtNew);
                    else PlayerOne.KillPiece(pieceAtNew);
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                    Moves.Push(new Move(prevBlock, newBlock, pieceAtPrev, pieceAtNew));
                    DisplayMoves();
                }
                Console.WriteLine("Board After");
                Board.DisplayBoard();
                if (CurrentMove == PlayerOne) CurrentMove = PlayerTwo;
                else CurrentMove = PlayerOne;
                // handle killing functionality
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
    }
}
