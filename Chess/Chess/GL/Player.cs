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
            DeadPieces = new LinkedList();
        }

        public void KillPiece(Piece piece)
        {
            DeadPieces.InsertAtHead(piece);
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

        public string GetFirstDeadPiece()
        {
            return DeadPieces.GetFirstPiece();
        }

        public override string ToString()
        {
            return $"Player Color: {Color.ToString()}";
        }

        public bool GetLatestDeadPiece()
        {
            if (DeadPieces.GetSize() > 0)
            {
                DeadPieces.RemoveFirstPiece();
                return true;
            }
            return false;
        }
    }
}
