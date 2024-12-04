using Chess.Interfaces;

namespace Chess.GL
{
    public class Pawn : Piece, IMove
    {
        private bool HasMoved;
        public Pawn(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            HasMoved = false;
        }

        public void GetPossibleMoves(Board board)
        {
            if (IsAlive() && GetPieceType() == PieceType.Pawn) // if a piece is alive and is a pawn
            {
                Block block = board.GetBlock(this); // get the block of the piece
                System.Console.WriteLine("Pawn at " + block.GetRank() + ", " + block.GetFile());
                int rank = block.GetRank();
                int file = block.GetFile();
                if (board.GetFirstPlayerColor() == PlayerColor.White) // if first player's color is white and white pieces are on rank 6, 7 we check for ranks before it for white and after it for black
                {
                    // FOR WHITE PAWNS
                    // if pawn on the selected block is white so we check for ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank - 1) + ", " + file);
                    }
                    // if pawn on the selected block is white and hasn't moved yet so we check for 2 ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.White && !HasMoved && board.GetBlock(rank - 1, file).IsEmpty() && board.GetBlock(rank - 2, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank - 2) + ", " + file);
                    }
                    // if pawn on the selected block is white and there is a black piece on it's left diagonal
                    if(block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file - 1));
                    }
                    // if pawn on the selected block is white and there is a black piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file + 1));
                    }
                    
                    // FOR BLACK PAWNS
                    // if pawn on the selected block is black so we check for ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank + 1) + ", " + file);
                    }
                    // if pawn on the selected block is black and hasn't moved yet so we check for 2 ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.Black && !HasMoved && board.GetBlock(rank + 1, file).IsEmpty() && board.GetBlock(rank + 2, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank + 2) + ", " + file);
                    }
                    // if pawn on the selected block is black and there is a white piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file - 1));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file + 1));
                    }
                }
                else // if first player's color is black and white pieces are on rank 1, 0 we check for ranks after it for white and before for black
                {
                    // FOR WHITE PAWNS
                    // if pawn on the selected block is white so we check for ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank + 1) + ", " + file);
                    }
                    // if pawn on the selected block is white and hasn't moved yet so we check for 2 ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.White && !HasMoved && board.GetBlock(rank + 1, file).IsEmpty() && board.GetBlock(rank + 2, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank + 2) + ", " + file);
                    }
                    // if pawn on selected block is white and there is a black piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file - 1));
                    }
                    // if pawn on selected block is white and there is a black piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file + 1));
                    }

                    // FOR BLACK PAWNS
                    // if pawn on the selected block is black so we check for ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank - 1) + ", " + file);
                    }
                    // if pawn on the selected block is black and hasn't moved yet so we check for 2 ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.Black && !HasMoved && board.GetBlock(rank - 1, file).IsEmpty() && board.GetBlock(rank - 2, file).IsEmpty())
                    {
                        System.Console.WriteLine("Move to " + (rank - 2) + ", " + file);
                    }
                    // if pawn on the selected block is black and there is a white piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file - 1));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file + 1));
                    }
                }
            }
        }

        public void SetHasMoved(bool hasMoved)
        {
            HasMoved = hasMoved;
        }

        public bool GetHasMoved()
        {
            return HasMoved;
        }
    }
}
