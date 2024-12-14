using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Knight : Piece
    {
        public Knight(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }

        List<Move> possibleMoves = new List<Move>();
        public override List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();

            if (!this.IsAlive() || GetPieceType() != PieceType.Knight)
                return possibleMoves;

            Block block = board.GetBlock(this);

            int rank = block.GetRank();
            int file = block.GetFile();

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

                    // Ensure move does not leave the king in check
                    if ((endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor())
                        && board.IsSafeMove(this, endBlock))
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }

            return possibleMoves;
        }

        public override bool CanAttack(Block targetBlock, Board board)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int targetRank = targetBlock.GetRank();
            int targetFile = targetBlock.GetFile();

            int rankDiff = Math.Abs(rank - targetRank);
            int fileDiff = Math.Abs(file - targetFile);

            return (rankDiff == 2 && fileDiff == 1) || (rankDiff == 1 && fileDiff == 2);
        }
    }
}
