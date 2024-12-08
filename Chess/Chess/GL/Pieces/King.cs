using Chess.Interfaces;
using System.Collections.Generic;

namespace Chess.GL
{
    public class King : Piece
    {
        private CastlingType CastlingType; // short or long
        private bool HasMoved;

        public King(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            CastlingType = CastlingType.None;
            HasMoved = false;
        }

        public override List<Move> GetPossibleMoves(Board board)
        {
            List<Move> possibleMoves = new List<Move>();

            if (!IsAlive() || GetPieceType() != PieceType.King)
                return possibleMoves;

            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int[][] directions = {
                new int[] {-1, -1}, // Top-left
                new int[] {-1, 0},  // Top
                new int[] {-1, 1},  // Top-right
                new int[] {0, -1},  // Left
                new int[] {0, 1},   // Right
                new int[] {1, -1},  // Bottom-left
                new int[] {1, 0},   // Bottom
                new int[] {1, 1}    // Bottom-right
            };

            foreach (var dir in directions)
            {
                int newRank = rank + dir[0];
                int newFile = file + dir[1];

                if (board.WithinBounds(newRank, newFile))
                {
                    Block endBlock = board.GetBlock(newRank, newFile);
                    if ((endBlock.IsEmpty() || endBlock.GetPiece().GetColor() != this.GetColor()) && board.IsSafeMove(this, endBlock))
                    {
                        possibleMoves.Add(new Move(currentBlock, endBlock, this, endBlock.GetPiece()));
                    }
                }
            }
            return possibleMoves;
        }

        public void SetHasMoved()
        {
            HasMoved = true;
        }

        public bool GetHasMoved()
        {
            return HasMoved;
        }

        public CastlingType GetCastlingType()
        {
            return CastlingType;
        }

        public void SetCastlingType(CastlingType type)
        {
            CastlingType = type;
        }
    }
}
