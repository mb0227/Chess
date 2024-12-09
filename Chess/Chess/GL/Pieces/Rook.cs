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
                    if (endBlock.GetPiece()?.GetColor() != this.GetColor() && board.IsSafeMove(this, endBlock) && board.IsSafeMove(this, endBlock))
                    {
                        possibleMoves.Add(new Move(startBlock, endBlock, this, endBlock.GetPiece()));
                    }
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
                                rank == kingBlock?.GetRank() && file == kingBlock?.GetFile()
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

        public override bool CanAttack(Block targetBlock, Board board)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int targetRank = targetBlock.GetRank();
            int targetFile = targetBlock.GetFile();

            if (rank == targetRank || file == targetFile) // vertical or horizontal move
            {
                return board.IsPathClear(rank, file, targetRank, targetFile);
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
