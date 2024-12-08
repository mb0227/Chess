using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Rook : Piece
    {
        List<Move> possibleMoves = new List<Move>();
        private bool HasMoved;
        public Rook(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            HasMoved = false;
        }

        public override List<Move> GetPossibleMoves(Board board)
        {
            possibleMoves.Clear();
            if (!IsAlive() || GetPieceType() != PieceType.Rook)
                return possibleMoves;

            Block block = board.GetBlock(this);
            int rank = block.GetRank();
            int file = block.GetFile();
            Console.WriteLine($"Rook at {rank}, {file}");

            int[][] directions = new int[][]
            {
                new int[] { 1,  0}, // Up
                new int[] {-1,  0}, // Down
                new int[] { 0, -1}, // Left
                new int[] { 0,  1}  // Right
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
                    if (endBlock.GetPiece()?.GetColor() != this.GetColor() && board.IsSafeMove(this, endBlock) && board.IsSafeMove(this, endBlock))
                    {
                        Console.WriteLine($"Attack to {newRank}, {newFile}");
                        possibleMoves.Add(new Move(startBlock, endBlock, this, endBlock.GetPiece()));
                    }
                    break; // stop exploring further in this direction if a piece is encountered
                } 
            }
        }

        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block pieceBlock = board.GetBlock(this);
            int pieceRank = pieceBlock.GetRank();
            int pieceFile = pieceBlock.GetFile();

            if (this.GetPieceType() == PieceType.Rook)
            {
                int[][] directions = {
                new int[] {-1, 0}, // Up
                new int[] {1, 0},  // Down
                new int[] {0, -1}, // Left
                new int[] {0, 1}   // Right
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
                            if (block.GetPiece()?.GetColor() != this.GetColor() &&
                                rank == kingBlock.GetRank() && file == kingBlock.GetFile()
                                && board.IsSafeMove(this, block))
                            {
                                return true; // rook attacks the king
                            }
                            break; 
                        }
                    }
                }
            }
            return false;
        }

        public void SetHasMoved()
        {
            HasMoved = true;
        }

        public bool GetHasMoved()
        {
            return HasMoved;
        }
    }
}
