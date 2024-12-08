using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Queen : Piece, IMove
    {
        public Queen(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }
            List<Move> possibleMoves = new List<Move>();

        public List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (IsAlive() && this.GetPieceType() == PieceType.Queen)
            {
                Block block = board.GetBlock(this); // get the block of the piece
                int rank = block.GetRank();
                int file = block.GetFile();
                System.Console.WriteLine("Queen at " + rank + ", " + file);
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
                // up
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank + i, file) && board.GetBlock(rank + i, file).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank + i).ToString() + ", " + file.ToString());
                        Block endBlock = board.GetBlock(rank + i, file);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank + i, file) && board.GetBlock(rank + i, file).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank + i).ToString() + ", " + file.ToString());
                            Block endBlock = board.GetBlock(rank + i, file);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // down
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank - i, file) && board.GetBlock(rank - i, file).IsEmpty())
                    {
                        Console.WriteLine("Move to " + (rank - i).ToString() + ", " + file.ToString());
                        Block endBlock = board.GetBlock(rank - i, file);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank - i, file) && board.GetBlock(rank - i, file).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + (rank - i).ToString() + ", " + file.ToString());
                            Block endBlock = board.GetBlock(rank - i, file);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // left
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank, file - i) && board.GetBlock(rank, file - i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + rank.ToString() + ", " + (file - i).ToString());
                        Block endBlock = board.GetBlock(rank, file - i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank, file - i) && board.GetBlock(rank, file - i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + rank.ToString() + ", " + (file - i).ToString());
                            Block endBlock = board.GetBlock(rank, file - i);
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                }
                // right
                for (int i = 1; i < 8; i++)
                {
                    if (board.WithinBounds(rank, file + i) && board.GetBlock(rank, file + i).IsEmpty())
                    {
                        Console.WriteLine("Move to " + rank.ToString() + ", " + (file + i).ToString());
                        Block endBlock = board.GetBlock(rank, file + i);
                        if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        {
                            possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                        }
                    }
                    else
                    {
                        if (board.WithinBounds(rank, file + i) && board.GetBlock(rank, file + i).GetPiece().GetColor() != this.GetColor())
                        {
                            Console.WriteLine("Attack to " + rank.ToString() + ", " + (file + i).ToString());
                            Block endBlock = board.GetBlock(rank, file + i);
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
            foreach (Move move in possibleMoves)
            {
                if (move.GetStartBlock() == move.GetEndBlock() && move.GetEndBlock() == targetBlock)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
