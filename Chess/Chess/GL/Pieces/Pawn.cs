using System.Collections.Generic;
using Chess.Interfaces;

namespace Chess.GL
{
    public class Pawn : Piece, IMove
    {
        private bool HasMoved;
        private bool IsEnPassantable;
        public Pawn(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            HasMoved = false;
            IsEnPassantable = false;
        }

        public List <Move> GetPossibleMoves(Board board)
        {
            List <Move> possibleMoves = new List <Move>();
            if (IsAlive() && GetPieceType() == PieceType.Pawn) // if a piece is alive and is a pawn
            {
                Block block = board.GetBlock(this); // get the block of the piece
                System.Console.WriteLine("Pawn at " + block.GetRank() + ", " + block.GetFile());
                int rank = block.GetRank();
                int file = block.GetFile();
                if (board.GetFirstPlayerColor() == PlayerColor.White) // if first player's color is white and white pieces are on rank 6, 7 we check for ranks before it for white and after it for black
                {
                    // For White Pawns
                    // if pawn on the selected block is white so we check for ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank - 1) + ", " + file);
                        Block endBlock = board.GetBlock(rank - 1, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is white and hasn't moved yet so we check for 2 ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.White && !HasMoved && board.GetBlock(rank - 1, file).IsEmpty() && board.GetBlock(rank - 2, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank - 2) + ", " + file);
                        Block endBlock = board.GetBlock(rank - 2, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is white and there is a black piece on it's left diagonal
                    if(block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        //System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn on the selected block is white and there is a black piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        //System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn is on last rank first we check for just the next rank on same file and if it's empty we add a move to it
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file) && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file);
                        Move move = new Move(block, endBlock, block.GetPiece(), null);
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on left file
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on right file
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // check for white piece en passant on left diagonal
                    if (rank == 3 &&block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank, file - 1) && !board.GetBlock(rank, file - 1).IsEmpty() && board.GetBlock(rank, file - 1).GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank, file - 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file - 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file - 1).GetPiece()));
                    }
                    // check for white piece en passant on right diagonal
                    if (rank == 3 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank, file + 1) && !board.GetBlock(rank, file + 1).IsEmpty() && board.GetBlock(rank, file + 1).GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank, file + 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file + 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file + 1).GetPiece()));
                    }

                    // For Black Pawns
                    // if pawn on the selected block is black so we check for ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank + 1) + ", " + file);
                        Block endBlock = board.GetBlock(rank + 1, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is black and hasn't moved yet so we check for 2 ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.Black && !HasMoved && board.GetBlock(rank + 1, file).IsEmpty() && board.GetBlock(rank + 2, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank + 2) + ", " + file);
                        Block endBlock = board.GetBlock(rank + 2, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        //System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        //System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn is on last rank first we check for just the next rank on same file and if it's empty we add a move to it
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file) && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file);
                        Move move = new Move(block, endBlock, block.GetPiece(), null);
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on left file
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on right file
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // check for black piece en passant on left diagonal
                    if (rank == 4 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank, file - 1) && !board.GetBlock(rank, file - 1).IsEmpty() && board.GetBlock(rank, file - 1).GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank, file - 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file - 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file - 1).GetPiece()));
                    }
                    // check for black piece en passant on right diagonal
                    if (rank == 4 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank, file + 1) && !board.GetBlock(rank, file + 1).IsEmpty() && board.GetBlock(rank, file + 1).GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank, file + 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file + 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file + 1).GetPiece()));
                    }   
                }
                else // if first player's color is black and white pieces are on rank 1, 0 we check for ranks after it for white and before for black
                {
                    // For White pawns
                    // if pawn on the selected block is white so we check for ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank + 1) + ", " + file);
                        Block endBlock = board.GetBlock(rank + 1, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is white and hasn't moved yet so we check for 2 ranks after it
                    if (block.GetPiece().GetColor() == PieceColor.White && !HasMoved && board.GetBlock(rank + 1, file).IsEmpty() && board.GetBlock(rank + 2, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank + 2) + ", " + file);
                        Block endBlock = board.GetBlock(rank + 2, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on selected block is white and there is a black piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        //System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn on selected block is white and there is a black piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        //System.Console.WriteLine("Attack to " + (rank + 1) + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn is on last rank first we check for just the next rank on same file and if it's empty we add a move to it
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file) && board.GetBlock(rank + 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file);
                        Move move = new Move(block, endBlock, block.GetPiece(), null);
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on left file
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file - 1) && !board.GetBlock(rank + 1, file - 1).IsEmpty() && board.GetBlock(rank + 1, file - 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on right file
                    if (rank == 6 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank + 1, file + 1) && !board.GetBlock(rank + 1, file + 1).IsEmpty() && board.GetBlock(rank + 1, file + 1).GetPiece().GetColor() == PieceColor.Black)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // check for white piece en passant on left diagonal
                    if (rank == 4 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank, file - 1) && !board.GetBlock(rank, file - 1).IsEmpty() && board.GetBlock(rank, file - 1).GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank, file - 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file - 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank + 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file - 1).GetPiece()));
                    }
                    // check for white piece en passant on right diagonal
                    if (rank == 4 && block.GetPiece().GetColor() == PieceColor.White && board.WithinBounds(rank, file + 1) && !board.GetBlock(rank, file + 1).IsEmpty() && board.GetBlock(rank, file + 1).GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank, file + 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file + 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank + 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file + 1).GetPiece()));
                    }

                    // For Black Pawns
                    // if pawn on the selected block is black so we check for ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank - 1) + ", " + file);
                        Block endBlock = board.GetBlock(rank - 1, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is black and hasn't moved yet so we check for 2 ranks before it
                    if (block.GetPiece().GetColor() == PieceColor.Black && !HasMoved && board.GetBlock(rank - 1, file).IsEmpty() && board.GetBlock(rank - 2, file).IsEmpty())
                    {
                        //System.Console.WriteLine("Move to " + (rank - 2) + ", " + file);
                        Block endBlock = board.GetBlock(rank - 2, file);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), null));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's left diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        //System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn on the selected block is black and there is a white piece on it's right diagonal
                    if (block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        //System.Console.WriteLine("Attack to " + (rank - 1) + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece()));
                    }
                    // if pawn is on last rank first we check for just the next rank on same file and if it's empty we add a move to it
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file) && board.GetBlock(rank - 1, file).IsEmpty())
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file);
                        Move move = new Move(block, endBlock, block.GetPiece(), null);
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on left file
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file - 1) && !board.GetBlock(rank - 1, file - 1).IsEmpty() && board.GetBlock(rank - 1, file - 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // if pawn is on last rank now we check for next rank on right file
                    if (rank == 1 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank - 1, file + 1) && !board.GetBlock(rank - 1, file + 1).IsEmpty() && board.GetBlock(rank - 1, file + 1).GetPiece().GetColor() == PieceColor.White)
                    {
                        System.Console.WriteLine("Promotion");
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        Move move = new Move(block, endBlock, block.GetPiece(), endBlock.GetPiece());
                        move.SetIsPromotion(true);
                        possibleMoves.Add(move);
                    }
                    // check for black piece en passant on left diagonal
                    if (rank == 3 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank, file - 1) && !board.GetBlock(rank, file - 1).IsEmpty() && board.GetBlock(rank, file - 1).GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank, file - 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file - 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file - 1));
                        Block endBlock = board.GetBlock(rank - 1, file - 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file - 1).GetPiece()));
                    }
                    // check for black piece en passant on right diagonal
                    if (rank == 3 && block.GetPiece().GetColor() == PieceColor.Black && board.WithinBounds(rank, file + 1) && !board.GetBlock(rank, file + 1).IsEmpty() && board.GetBlock(rank, file + 1).GetPiece().GetColor() == PieceColor.White && board.GetBlock(rank, file + 1).GetPiece().GetPieceType() == PieceType.Pawn && ((Pawn)board.GetBlock(rank, file + 1).GetPiece()).GetEnPassantable())
                    {
                        System.Console.WriteLine("En Passant to " + rank + ", " + (file + 1));
                        Block endBlock = board.GetBlock(rank - 1, file + 1);
                        possibleMoves.Add(new Move(block, endBlock, block.GetPiece(), board.GetBlock(rank, file + 1).GetPiece()));
                    }
                }
            }
            return possibleMoves;
        }

        public void PawnMoved(int rank)
        {
            if (rank == 2) IsEnPassantable = true; 
            HasMoved = true;
        }

        public bool GetHasMoved()
        {
            return HasMoved;
        }

        public bool GetEnPassantable()
        {
            return IsEnPassantable;
        }

        public void SetUnEnPassantable()
        {
            IsEnPassantable = false;
        }
    }
}
