using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class King : Piece
    {
        private CastlingType CastlingType; // short or long
        private bool HasMoved;
        private bool InCheck;
        
        public King(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            CastlingType = CastlingType.None;
            HasMoved = false;
            InCheck = false;
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

            // AddCastlingMoves(possibleMoves, board, currentBlock);

            return possibleMoves;
        }

        public void AddCastlingMoves(List<Move> moves, Board board, Block kingBlock)
        {
            int rank = kingBlock.GetRank();
            int file = kingBlock.GetFile();

            // kingside (short castling)
            if (CanCastle(board, kingBlock, rank, file, true))
            {
                Block rookBlock = board.GetBlock(rank, 7); // rook at file 7
                Block castlingEndBlock = board.GetBlock(rank, file + 2); // king moves two squares right
                moves.Add(new Move(kingBlock, castlingEndBlock, this, null));
            }

            // queenside (long castling)
            if (CanCastle(board, kingBlock, rank, file, false))
            {
                Block rookBlock = board.GetBlock(rank, 0); // rook at file 0
                Block castlingEndBlock = board.GetBlock(rank, file - 2); // king moves two squares left
                moves.Add(new Move(kingBlock, castlingEndBlock, this, null));
            }
        }

        private bool CanCastle(Board board, Block kingBlock, int rank, int file, bool isKingside)
        {
            if (HasMoved) return false; // King has moved, no castling allowed

            int rookFile = isKingside ? 7 : 0; // kingside or queenside rook position
            Block rookBlock = board.GetBlock(rank, rookFile);

            // Check if the rook exists, hasn't moved, and is the same color as the king
            if (rookBlock == null || rookBlock.IsEmpty() || rookBlock.GetPiece().GetPieceType() != PieceType.Rook) return false;
            if (rookBlock.GetPiece().GetColor() != GetColor() || ((Rook)rookBlock.GetPiece()).GetHasMoved()) return false;
            if (InCheck) return false;

            // Check if the squares between the king and rook are empty
            int start = Math.Min(file, rookFile) + 1;
            int end = Math.Max(file, rookFile);
            for (int i = start; i < end; i++)
            {
                if (!board.GetBlock(rank, i).IsEmpty()) return false;
            }

            int direction = isKingside ? 1 : -1; // Kingside moves right, queenside moves left
            for (int i = 0; i <= 2; i++)
            {
                int newFile = file + (i * direction);
                Block currentBlock = board.GetBlock(rank, newFile);
                if (board.IsUnderAttack(currentBlock, GetColor())) return false;
            }

            return true;
        }


        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();
            int kingRank = kingBlock.GetRank();
            int kingFile = kingBlock.GetFile();

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
                if (!board.WithinBounds(rank, file)) continue;
                if (newRank == kingRank && newFile == kingFile)
                {
                    return true;
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

        public CastlingType GetCastlingType()
        {
            return CastlingType;
        }

        public void SetCastlingType(CastlingType type)
        {
            CastlingType = type;
        }

        public void SetCheck(bool check)
        {
            InCheck = check;
        }

        public bool IsInCheck()
        {
            return InCheck;
        }
    }
}
