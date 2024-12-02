using Chess.Interfaces;

namespace Chess.GL
{
    public class Pawn : Piece, IMove
    {
        public Pawn(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }

        public void GetPossibleMoves(Board board)
        {
            if (IsAlive() && GetPieceType() == PieceType.Pawn) // if a piece is alive and is a pawn
            {
                Block block = board.GetBlock(this); // get the block of the piece
            }
        }
    }
}
