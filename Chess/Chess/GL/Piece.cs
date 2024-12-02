namespace Chess.GL
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public enum PieceColor
    {
        White,
        Black
    }

    public class Piece
    {
        private PieceColor Color;
        private PieceType PieceType;
        private bool Alive;
        public Piece(PieceColor color, PieceType type, bool alive)
        {
            Color = color;
            PieceType = type;
            Alive = alive;
        }
        public PieceColor GetColor()
        {
            return Color;
        }
        public PieceType GetPieceType()
        {
            return PieceType;
        }
        public bool IsAlive()
        {
            return Alive;
        }
    }
}
