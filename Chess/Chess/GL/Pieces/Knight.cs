using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Knight : Piece, IMove
    {
        public Knight(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }

        public List<Move> GetPossibleMoves(Board board)
        {
            List<Move> possibleMoves = new List<Move>();
            if(this.IsAlive() && GetPieceType() == PieceType.Knight)
            {
                Block block = board.GetBlock(this); // get the block of the piece
                int rank = block.GetRank();
                int file = block.GetFile();
                System.Console.WriteLine("Knight at " + rank + ", " + file);
                // 2 up 1 right
                if (board.WithinBounds(rank + 2, file + 1))
                {
                    Console.WriteLine("2 up 1 right");
                    Block endBlock = board.GetBlock(rank + 2, file + 1);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 2 up 1 left
                if (board.WithinBounds(rank + 2, file - 1))
                {
                    Console.WriteLine("2 up 1 left");
                    Block endBlock = board.GetBlock(rank + 2, file - 1);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 2 down 1 right
                if (board.WithinBounds(rank - 2, file + 1))
                {
                    Console.WriteLine("2 down 1 right");
                    Block endBlock = board.GetBlock(rank - 2, file + 1);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 2 down 1 left
                if (board.WithinBounds(rank - 2, file - 1))
                {
                    Console.WriteLine("2 down 1 left");
                    Block endBlock = board.GetBlock(rank - 2, file - 1);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 1 up 2 right
                if (board.WithinBounds(rank + 1, file + 2))
                {
                    Console.WriteLine("1 up 2 right");
                    Block endBlock = board.GetBlock(rank + 1, file + 2);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 1 up 2 left
                if (board.WithinBounds(rank + 1, file - 2))
                {
                    Console.WriteLine("1 up 2 left");
                    Block endBlock = board.GetBlock(rank + 1, file - 2);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 1 down 2 right
                if (board.WithinBounds(rank - 1, file + 2))
                {
                    Console.WriteLine("1 down 2 right");
                    Block endBlock = board.GetBlock(rank - 1, file + 2);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
                // 1 down 2 left
                if (board.WithinBounds(rank - 1, file - 2))
                {
                    Console.WriteLine("1 down 2 left");
                    Block endBlock = board.GetBlock(rank - 1, file - 2);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }
            return possibleMoves;
        }
    }
}
