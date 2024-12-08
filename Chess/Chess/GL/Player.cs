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
        private bool InCheck;

        public Player(PlayerColor color)
        {
            Color = color;
            DeadPieces = new LinkedList();
            InCheck = false;
        }

        public void KillPiece(Piece piece)
        {
            DeadPieces.InsertAtHead(piece);
        }

        public void SetCheck(bool check)
        {
            InCheck = check;
        }

        public bool IsInCheck()
        {
            return InCheck;
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
