using Chess.DS;

namespace Chess.GL
{
    public enum PlayerColor
    {
        White,
        Black
    }

    public enum PlayerType
    {
        Human,
        Computer
    }

    public class Player
    {
        private PlayerColor Color;
        private LinkedList DeadPieces;

        public Player(PlayerColor color)
        {
            Color = color;
        }

        public PlayerColor GetColor()
        {
            return Color;
        }

        public void AddDeadPiece(Piece piece)
        {
            DeadPieces.InsertAtHead(piece);
        }

        public void DisplayDeadPieces()
        {
            DeadPieces.Display();
        }

        public override string ToString()
        {
            return $"Player Color: {Color.ToString()}";
        }
    }
}
