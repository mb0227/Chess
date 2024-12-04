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
            CurrentMove = playerOne;
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

        public void UpdateStatus(GameStatus status)
        {
            Status = status;
        }

        public void SetIsGameOver(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }
    }
}
