using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Queen : Piece
    {
        public Queen(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
        }
            List<Move> possibleMoves = new List<Move>();

        public override List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (IsAlive() && this.GetPieceType() == PieceType.Queen)
            {
                Block block = board.GetBlock(this);
                int rank = block.GetRank();
                int file = block.GetFile();

                // Directions for Queen's movement
                int[][] directions =
                {
                    new int[] { -1, -1 }, // Top-left
                    new int[] { -1, 1 },  // Top-right
                    new int[] { 1, -1 },  // Bottom-left
                    new int[] { 1, 1 },   // Bottom-right
                    new int[] { 1, 0 },   // Down
                    new int[] { -1, 0 },  // Up
                    new int[] { 0, -1 },  // Left
                    new int[] { 0, 1 }    // Right
                };

                foreach (var direction in directions)
                {
                    int deltaRank = direction[0];
                    int deltaFile = direction[1];
                    AddMovesInDirection(rank, file, deltaRank, deltaFile, block, board);
                }
            }
            return possibleMoves;
        }

        private void AddMovesInDirection(int rank, int file, int deltaRank, int deltaFile, Block startBlock, Board board)
        {
            for (int i = 1; i < 8; i++)
            {
                int newRank = rank + i * deltaRank;
                int newFile = file + i * deltaFile;

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

            if (Math.Abs(rank - targetRank) == Math.Abs(file - targetFile)) // Diagonal movement
            {
                return board.IsPathClear(rank, file, targetRank, targetFile);
            }
            else if (rank == targetRank || file == targetFile) // Horizontal or vertical movement
            {
                return board.IsPathClear(rank, file, targetRank, targetFile); 
            }

            return false;
        }
    }
}
