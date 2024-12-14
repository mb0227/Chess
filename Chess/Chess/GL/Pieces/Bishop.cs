using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Bishop : Piece
    {
        public Bishop(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }

        List<Move> possibleMoves = new List<Move>();

        public override List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (!IsAlive() || GetPieceType() != PieceType.Bishop)
                return possibleMoves;

            Block block = board.GetBlock(this);
            int rank = block.GetRank();
            int file = block.GetFile();

            // Define directions for bishop movement (diagonals)
            int[][] directions = new int[][]
            {
                new int[] {-1, -1}, // Top-left
                new int[] {-1,  1}, // Top-right
                new int[] { 1, -1}, // Bottom-left
                new int[] { 1,  1}  // Bottom-right
            };

            foreach (var dir in directions)
            {
                ExploreDirection(board, block, rank, file, dir[0], dir[1]);
            }

            return possibleMoves;
        }

        private void ExploreDirection(Board board, Block startBlock, int rank, int file, int rankIncrement, int fileIncrement)
        {
            for (int i = 1; i < 8; i++)
            {
                int newRank = rank + i * rankIncrement;
                int newFile = file + i * fileIncrement;

                if (!board.WithinBounds(newRank, newFile))
                    break;

                Block endBlock = board.GetBlock(newRank, newFile);

                if (endBlock.IsEmpty() && board.IsSafeMove(this, endBlock))
                {
                    possibleMoves.Add(new Move(startBlock, endBlock, this, null));
                }
                else
                {
                    if (endBlock?.GetPiece() != null) // stop further movement in this direction
                    {
                        if (endBlock.GetPiece().GetColor() != this.GetColor() && board.IsSafeMove(this, endBlock))
                        {
                            possibleMoves.Add(new Move(startBlock, endBlock, this, endBlock.GetPiece()));
                        }
                        break;
                    }
                    if ((endBlock.GetPiece() != null && endBlock.GetPiece().GetColor() != this.GetColor()) && board.IsSafeMove(this, endBlock))
                    {
                        possibleMoves.Add(new Move(startBlock, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }
        }

        public override bool CanAttack(Block targetBlock, Board board)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int targetRank = targetBlock.GetRank();
            int targetFile = targetBlock.GetFile();
            if (Math.Abs(rank - targetRank) == Math.Abs(file - targetFile)) // diagonal move
            {
                return board.IsPathClear(rank, file, targetRank, targetFile); // Ensuring no blocking pieces
            }
            return false;
        }
    }
}