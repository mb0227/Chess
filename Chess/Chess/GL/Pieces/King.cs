using System;
using System.Collections.Generic;
using System.Windows;

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

            if (!board.is960 && (board.GetFirstPlayerColor() != PlayerColor.Black && file != 4) || (board.GetFirstPlayerColor() == PlayerColor.Black && file != 3))
                HasMoved = true;

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

            if (!board.is960) AddCastlingMoves(possibleMoves, board, currentBlock);
            else AddChess960CastlingMoves(possibleMoves, board, currentBlock);

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
                if (board.GetFirstPlayerColor() != PlayerColor.White)
                {
                    rookBlock = board.GetBlock(rank, 0); // rook at file 0
                    castlingEndBlock = board.GetBlock(rank, file - 2); // king moves two squares left
                }
                moves.Add(new Move(kingBlock, castlingEndBlock, this, null));
            }

            // queenside (long castling)
            if (CanCastle(board, kingBlock, rank, file, false))
            {
                Block rookBlock = board.GetBlock(rank, 0); // rook at file 0
                Block castlingEndBlock = board.GetBlock(rank, file - 2); // king moves two squares left
                if (board.GetFirstPlayerColor() != PlayerColor.White)
                {
                    rookBlock = board.GetBlock(rank, 7); // rook at file 7
                    castlingEndBlock = board.GetBlock(rank, file + 2); // king moves two squares RIGHT
                }
                moves.Add(new Move(kingBlock, castlingEndBlock, this, null));
            }
        }

        private bool CanCastle(Board board, Block kingBlock, int rank, int file, bool isKingside)
        {
            if (HasMoved) return false; // King has moved, no castling allowed

            int rookFile = isKingside ? 7 : 0; // kingside or queenside rook position
            if (board.GetFirstPlayerColor() == PlayerColor.Black)
            {
                rookFile = isKingside ? 0 : 7;
            }

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
            if (board.GetFirstPlayerColor() == PlayerColor.Black)
                direction = isKingside ? -1 : 1;

            for (int i = 0; i <= 2; i++)
            {
                int newFile = file + (i * direction);

                if (!board.WithinBounds(rank, newFile)) break;

                Block currentBlock = board.GetBlock(rank, newFile);
                if (!board.IsSquareSafeForCastling(currentBlock, GetColor()))
                {
                    return false;
                }
            }
            return true;
        }

        // Helper function to find the rook in Chess960

        public override bool CanAttack(Block targetBlock, Board board)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int targetRank = targetBlock.GetRank();
            int targetFile = targetBlock.GetFile();

            return Math.Abs(rank - targetRank) <= 1 && Math.Abs(file - targetFile) <= 1;
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

        public void UndoMove()
        {
            HasMoved = false;
        }

        public void AddChess960CastlingMoves(List<Move> moves, Board board, Block kingBlock)
        {
            int rank = kingBlock.GetRank();
            int file = kingBlock.GetFile();
            // king side check
            Can960Castle(board, moves, kingBlock, rank, file, true);
            // queen side check
            Can960Castle(board, moves, kingBlock, rank, file, false);
        }

        private bool Can960Castle(Board board, List<Move> possibleMoves, Block kingBlock, int rank, int file, bool isKingside)
        {
            Block rookBlock = null;

            if (HasMoved || InCheck) return false; // King has moved or is in check

            rookBlock = board.FindRookUsingOpeningState(isKingside, GetColor());

            if (rookBlock == null || rookBlock.IsEmpty() || rookBlock.GetPiece().GetPieceType() != PieceType.Rook) return false;
            if (rookBlock.GetPiece().GetColor() != GetColor() || ((Rook)rookBlock.GetPiece()).GetHasMoved()) return false;
            // Calculate the king's end position and validate the path
            int kingTargetFile, rookTargetFile;
            if (board.GetFirstPlayerColor() == PlayerColor.White && isKingside)
                (kingTargetFile, rookTargetFile) = (6, 5);
            else if (board.GetFirstPlayerColor() == PlayerColor.White && !isKingside)
                (kingTargetFile, rookTargetFile) = (2, 3);
            else if (board.GetFirstPlayerColor() == PlayerColor.Black && isKingside)
                (kingTargetFile, rookTargetFile) = (1, 2);
            else
                (kingTargetFile, rookTargetFile) = (5, 4);

            if (kingBlock.GetFile() == kingTargetFile && rookBlock.GetFile() == rookTargetFile) return false;

            if (!IsPathClear(board, rank, file, rookBlock.GetFile())) return false;
            if (!IsKingPathSafe(board, rank, file, kingTargetFile, isKingside)) return false;
            if (!AreTargetSquaresClear(board, rank, isKingside, kingBlock, rookBlock)) return false;

            possibleMoves.Add(new Move(kingBlock, board.GetBlock(rank, kingTargetFile), rookBlock, board.GetBlock(rank, rookTargetFile), isKingside ? CastlingType.KingSideCastle: CastlingType.QueenSideCastle));

            return true;
        }

        private bool IsPathClear(Board board, int rank, int kingFile, int rookFile)
        {
            int startFile = Math.Min(kingFile, rookFile) + 1;
            int endFile = Math.Max(kingFile, rookFile) - 1;

            for (int file = startFile; file <= endFile; file++)
            {
                if (!board.GetBlock(rank, file).IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreTargetSquaresClear(Board board, int rank, bool isKingSide, Block kingBlock, Block rookBlock)
        {
            int[] files;
            if(board.GetFirstPlayerColor() == PlayerColor.White)
                files = isKingSide ? new int[] { 6, 5 } : new int[] { 2, 3};
            else
                files = isKingSide ? new int[] { 1, 2 } :new int[] { 5, 4 };
            foreach (var file in files)
            {
                Block block = board.GetBlock(rank, file);
                if (kingBlock == block || rookBlock == block) continue;
                if (!block.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsKingPathSafe(Board board, int rank, int kingFile, int kingTargetFile, bool isKingside)
        {
            int direction = isKingside ? 1 : -1;

            for (int file = kingFile; file != kingTargetFile + direction; file += direction)
            {
                if (!board.WithinBounds(rank, file)) continue;
                Block currentBlock = board.GetBlock(rank, file);
                if (!board.IsSquareSafeForCastling(currentBlock, GetColor()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
