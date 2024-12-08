using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Bishop : Piece, IMove
    {
        public Bishop(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }
            List<Move> possibleMoves = new List<Move>();

        public List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (IsAlive() && this.GetPieceType() == PieceType.Bishop)
            {
                Block block = board.GetBlock(this); // get the block of the piece
                int rank = block.GetRank();
                int file = block.GetFile();
                System.Console.WriteLine("Bishop at " + rank + ", " + file);
                // bishops can move diagonally so we check all 4 diagonals
                // top left
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank - i, file - i) && board.GetBlock(rank - i, file - i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank - i).ToString() + ", " + (file - i).ToString());
                        Block endBlock = board.GetBlock(rank - i, file - i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank - i, file - i) && board.GetBlock(rank - i, file - i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank - i).ToString() + ", " + (file - i).ToString());
                            Block endBlock = board.GetBlock(rank - i, file - i);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // top right
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank - i, file + i) && board.GetBlock(rank - i, file + i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank - i).ToString() + ", " + (file + i).ToString());
                        Block endBlock = board.GetBlock(rank - i, file + i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank - i, file + i) && board.GetBlock(rank - i, file + i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank - i).ToString() + ", " + (file + i).ToString());
                            Block endBlock = board.GetBlock(rank - i, file + i);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // bottom left
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank + i, file - i) && board.GetBlock(rank + i, file - i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank + i).ToString() + ", " + (file - i).ToString());
                        Block endBlock = board.GetBlock(rank + i, file - i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank + i, file - i) && board.GetBlock(rank + i, file - i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank + i).ToString() + ", " + (file - i).ToString());
                            Block endBlock = board.GetBlock(rank + i, file - i);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // bottom right
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank + i, file + i) && board.GetBlock(rank + i, file + i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank + i).ToString() + ", " + (file + i).ToString());
                        Block endBlock = board.GetBlock(rank + i, file + i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank + i, file + i) && board.GetBlock(rank + i, file + i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank + i).ToString() + ", " + (file + i).ToString());
                            Block endBlock = board.GetBlock(rank + i, file + i);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
            }
            return possibleMoves;
        }

        public override bool ValidMove(Board board, Block start, Block targetBlock)
        {
            Console.WriteLine("func");
            Console.WriteLine("start: " + start.GetRank() + ", " + start.GetFile());
            Console.WriteLine("target: " + targetBlock.GetRank() + ", " + targetBlock.GetFile());
            foreach (Move move in possibleMoves)
            {
                Console.WriteLine(move.GetNotation());
                if (move.GetStartBlock().GetRank() == start.GetRank() 
                    && move.GetStartBlock().GetFile() == start.GetFile()
                    && move.GetEndBlock().GetRank() == targetBlock.GetRank()
                    && move.GetEndBlock().GetFile() == targetBlock.GetFile())
                {
                    return true;
                }
            }
            return false;
        }
    }
}