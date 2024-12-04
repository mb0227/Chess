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
        private PlayerType PlayerType;
        private LinkedList DeadPieces;

        public Player(PlayerColor color, PlayerType playerType)
        {
            Color = color;
            PlayerType = playerType;
        }

        public PlayerColor GetColor()
        {
            return Color;
        }

        public PlayerType GetPlayerType()
        {
            return PlayerType;
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
            return $"Color: {Color.ToString()} PlayerType: {PlayerType.ToString()}";
        }
    }
}
