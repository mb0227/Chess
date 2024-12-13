using Chess.DS;

namespace Chess.GL
{
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

        public PlayerColor GetColor()
        {
            return Color;
        }

        public PlayerType GetPlayerType()
        {
            return PlayerType;
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

        public void KillPiece(Piece piece)
        {
            DeadPieces.InsertAtHead(piece);
        }

        public string GetFirstDeadPiece()
        {
            return DeadPieces.GetFirstPiece();
        }

        public void DisplayDeadPieces()
        {
            DeadPieces.Display();
        }

        public override string ToString()
        {
            return $"Player Color: {Color.ToString()} Player Type: {PlayerType.ToString()}";
        }
    }
}
