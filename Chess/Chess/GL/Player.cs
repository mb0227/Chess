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
        protected PlayerColor Color;
        protected LinkedList DeadPieces;
        protected PlayerType PlayerType;

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

        public void SetPlayerType(PlayerType type)
        {
            PlayerType = type;
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

        public string GetFirstDeadPiece()
        {
            return DeadPieces.GetFirstPiece();
        }

        public override string ToString()
        {
            return $"Player Color: {Color.ToString()} Player Type: {PlayerType.ToString()}";
        }

        public Piece GetLatestDeadPiece()
        {
            Piece piece = null;
            if (DeadPieces.GetSize() > 0)
            {
                piece = DeadPieces.RemoveFirstPiece();
                return piece;
            }
            return piece;
        }
    }
}
