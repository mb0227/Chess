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

        List<Move> possibleMoves = new List<Move>();
        public List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (!this.IsAlive() || GetPieceType() != PieceType.Knight)
                return possibleMoves;

            Block block = board.GetBlock(this);
            int rank = block.GetRank();
            int file = block.GetFile();
            System.Console.WriteLine("Knight at " + rank + ", " + file);

            int[,] directions = {
                {2, 1}, {2, -1}, {-2, 1}, {-2, -1},
                {1, 2}, {1, -2}, {-1, 2}, {-1, -2}
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newRank = rank + directions[i, 0];
                int newFile = file + directions[i, 1];

                if (board.WithinBounds(newRank, newFile))
                {
                    Block endBlock = board.GetBlock(newRank, newFile);
                    if (endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                    {
                        Console.WriteLine($"Move to {newRank}, {newFile}");
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }

            return possibleMoves;
        }

        public override bool ValidMove(Board board, Block start, Block targetBlock)
        {
            //GetPossibleMoves(board);
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
