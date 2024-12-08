using Chess.Interfaces;
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
            Console.WriteLine($"Bishop at {rank}, {file}");

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
                    break; // stop exploring further in this direction if blocked
                }
            }
        }

        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block pieceBlock = board.GetBlock(this); // Get current position of the piece
            int pieceRank = pieceBlock.GetRank();
            int pieceFile = pieceBlock.GetFile();

            // Handle bishop-specific logic (check all diagonal directions)
            if (this.GetPieceType() == PieceType.Bishop)
            {
                int[][] directions = {
                     new int[] {-1, -1}, new int[] {-1, 1}, new int[] {1, -1}, new int[] {1, 1}
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
                            if (block.GetPiece().GetColor() != this.GetColor() && rank == kingBlock.GetRank() && file == kingBlock.GetFile() && board.IsSafeMove(this, block))
                            {
                                return true; // Bishop can attack the king
                            }
                            break; // Stop at the first blocked piece
                        }
                    }
                }
            }
            return false;
        }
    }
}