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
                Console.WriteLine("Queen at " + rank + ", " + file);

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
                    Console.WriteLine($"Move to {newRank}, {newFile}");
                    possibleMoves.Add(new Move(startBlock, endBlock, this, null));
                }
                else
                {
                    if ((endBlock.GetPiece() != null && endBlock.GetPiece().GetColor() != this.GetColor()) && board.IsSafeMove(this, endBlock))
                    {
                        Console.WriteLine($"Attack to {newRank}, {newFile}");
                        possibleMoves.Add(new Move(startBlock, endBlock, this, endBlock.GetPiece()));
                    }
                    break; // stop further movement in this direction
                }
            }
        }

        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block pieceBlock = board.GetBlock(this);
            int pieceRank = pieceBlock.GetRank();
            int pieceFile = pieceBlock.GetFile();

            if (this.GetPieceType() == PieceType.Queen)
            {
                int[][] directions = {
                    new int[] {-1, 0}, // Up
                    new int[] {1, 0},  // Down
                    new int[] {0, -1}, // Left
                    new int[] {0, 1},  // Right
                    new int[] {-1, -1}, // Up-Left (Diagonal)
                    new int[] {-1, 1},  // Up-Right (Diagonal)
                    new int[] {1, -1},  // Down-Left (Diagonal)
                    new int[] {1, 1}    // Down-Right (Diagonal)
                };

                foreach (var dir in directions)
                {
                    int rank = pieceRank;
                    int file = pieceFile;

                    while (true)
                    {
                        rank += dir[0];
                        file += dir[1];

                        if (!board.WithinBounds(rank, file))
                            break;

                        Block block = board.GetBlock(rank, file);

                        if (block.GetPiece() != null)
                        {
                            if (block.GetPiece().GetColor() != this.GetColor() &&
                                rank == kingBlock.GetRank() && file == kingBlock.GetFile()
                                && board.IsSafeMove(this, block))
                            {
                                return true; // queen can attack the king
                            }
                            break; // stop if a piece blocks the path (no further movement in this direction)
                        }
                    }
                }
            }
            return false;
        }

    }
}
