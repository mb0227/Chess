using Chess.Interfaces;
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
                    if ((endBlock.GetPiece() == null || endBlock.GetPiece().GetColor() != this.GetColor()) && board.IsSafeMove(this, endBlock))
                    {
                        possibleMoves.Add(new Move(block, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }

            return possibleMoves;
        }

        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block pieceBlock = board.GetBlock(this); // Get current position of the piece
            int pieceRank = pieceBlock.GetRank();
            int pieceFile = pieceBlock.GetFile();

            // Handle knight-specific logic
            if (this.GetPieceType() == PieceType.Knight)
            {
                int[,] directions = {
                    {2, 1}, {2, -1}, {-2, 1}, {-2, -1},
                    {1, 2}, {1, -2}, {-1, 2}, {-1, -2}
                };

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int newRank = pieceRank + directions[i, 0];
                    int newFile = pieceFile + directions[i, 1];
                    Block block;

                    if (board.WithinBounds(newRank, newFile))
                        block = board.GetBlock(newRank, newFile);
                    else
                        continue;

                    if (newRank == kingBlock?.GetRank() && newFile == kingBlock?.GetFile() && board.IsSafeMove(this, block))
                    {
                        return true; // Knight can attack the king
                    }
                }
            }
            return false;
        }
    }
}
