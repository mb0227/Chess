using System;
using System.Collections.Generic;
using Chess.Interfaces;

namespace Chess.GL
{
    public class Pawn : Piece
    {
        private bool HasMoved;
        private bool IsEnPassantable;
        public Pawn(PieceColor color, PieceType type, bool alive) : base(color, type, alive)
        {
            HasMoved = false;
            IsEnPassantable = false;
        }

        public override List<Move> GetPossibleMoves(Board board)
        {
            List<Move> possibleMoves = new List<Move>();
            if (!IsAlive() || GetPieceType() != PieceType.Pawn)
                return possibleMoves;

            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();
            int direction = (board.GetFirstPlayerColor() == PlayerColor.White) ?
                            (GetColor() == PieceColor.White ? -1 : 1) :
                            (GetColor() == PieceColor.White ? 1 : -1);

            AddForwardMoves(possibleMoves, board, currentBlock, rank, file, direction);
            AddCaptures(possibleMoves, board, currentBlock, rank, file, direction);
            AddPromotions(possibleMoves, board, currentBlock, rank, file, direction);
            AddEnPassantMoves(possibleMoves, board, currentBlock, rank, file, direction);

            return possibleMoves;
        }

        private void AddForwardMoves(List<Move> moves, Board board, Block currentBlock, int rank, int file, int direction)
        {
            Block oneStep = (board.WithinBounds(rank + direction, file)) ? board.GetBlock(rank + direction, file) : null;
            if (oneStep != null && oneStep.IsEmpty())
            {
                if(board.IsSafeMove(this, oneStep))
                    moves.Add(new Move(currentBlock, oneStep, this, null));
                if (!HasMoved)
                {
                    Block twoSteps = (board.WithinBounds(rank + 2 * direction, file)) ? board.GetBlock(rank + 2 * direction, file) : null;
                    if (twoSteps != null && twoSteps.IsEmpty() && board.IsSafeMove(this, twoSteps))
                        moves.Add(new Move(currentBlock, twoSteps, this, null));
                }
            }
        }

        private void AddCaptures(List<Move> moves, Board board, Block currentBlock, int rank, int file, int direction)
        {
            foreach (int offset in new[] { -1, 1 })
            {
                int targetRank = rank + direction;
                int targetFile = file + offset;

                if (board.WithinBounds(targetRank, targetFile))
                {
                    Block captureBlock = board.GetBlock(targetRank, targetFile);
                    if (captureBlock != null && !captureBlock.IsEmpty() &&
                        captureBlock.GetPiece().GetColor() != GetColor()
                        && board.IsSafeMove(this, captureBlock))
                    {
                        moves.Add(new Move(currentBlock, captureBlock, this, captureBlock.GetPiece()));
                    }
                }
            }
        }

        private void AddPromotions(List<Move> moves, Board board, Block currentBlock, int rank, int file, int direction)
        {
            int promotionRank = (GetColor() == PieceColor.White) ? 0 : 7;
            if (rank + direction == promotionRank)
            {
                Block promotionBlock = (board.WithinBounds(rank + direction, file)) ? board.GetBlock(rank + direction, file) : null;
                if (promotionBlock != null && promotionBlock.IsEmpty() && board.IsSafeMove(this, promotionBlock))
                {
                    Move move = new Move(currentBlock, promotionBlock, this, null);
                    move.SetIsPromotion(true);
                    moves.Add(move);
                }
                foreach (int offset in new[] { -1, 1 })
                {
                    Block captureBlock = (board.WithinBounds(rank + direction, file + offset)) ? board.GetBlock(rank + direction, file + offset) : null;
                    if (captureBlock != null && !captureBlock.IsEmpty() &&
                        captureBlock.GetPiece().GetColor() != GetColor()
                        && board.IsSafeMove(this, captureBlock))
                    {
                        Move move = new Move(currentBlock, captureBlock, this, captureBlock.GetPiece());
                        move.SetIsPromotion(true);
                        moves.Add(move);
                    }
                }
            }
        }

        private void AddEnPassantMoves(List<Move> moves, Board board, Block currentBlock, int rank, int file, int direction)
        {
            int enPassantRank = (GetColor() == PieceColor.White) ? 3 : 4;
            if (rank == enPassantRank)
            {
                foreach (int offset in new[] { -1, 1 })
                {
                    Block sideBlock = board.GetBlock(rank, file + offset);
                    if (sideBlock != null && !sideBlock.IsEmpty() &&
                        sideBlock.GetPiece().GetPieceType() == PieceType.Pawn &&
                        ((Pawn)sideBlock.GetPiece()).GetEnPassantable() &&
                        sideBlock.GetPiece().GetColor() != GetColor())
                    {
                        Block endBlock = board.GetBlock(rank + direction, file + offset);
                        if(board.IsSafeMove(this, endBlock))
                            moves.Add(new Move(currentBlock, endBlock, this, sideBlock.GetPiece()));
                    }
                }
            }
        }

        public override bool IsAttackingKing(Board board, Block kingBlock)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();
            int kingRank = kingBlock.GetRank();
            int kingFile = kingBlock.GetFile();

            int direction = (board.GetFirstPlayerColor() == PlayerColor.White) ?
                            (GetColor() == PieceColor.White ? -1 : 1) :
                            (GetColor() == PieceColor.White ? 1 : -1);

            foreach (int offset in new[] { -1, 1 })
            {
                if (!board.WithinBounds(rank + direction, file + offset)) continue;
                if (rank + direction == kingRank && file + offset == kingFile)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanAttack(Block targetBlock, Board board)
        {
            Block currentBlock = board.GetBlock(this);
            int rank = currentBlock.GetRank();
            int file = currentBlock.GetFile();

            int targetRank = targetBlock.GetRank();
            int targetFile = targetBlock.GetFile();

            int direction = (GetColor() == PieceColor.White) ? 1 : -1; // Determine movement direction

            if (Math.Abs(file - targetFile) == 1 && targetRank - rank == direction)
            {
                Piece targetPiece = board.GetBlock(targetRank, targetFile).GetPiece();
                return targetPiece != null && targetPiece.GetColor() != GetColor();
            }
            return false;
        }

        public void PawnMoved(int rank)
        {
            if (rank == 2) IsEnPassantable = true; 
        }

        public void SetPawnMoved()
        {
            HasMoved = true;
        }

        public bool GetHasMoved()
        {
            return HasMoved;
        }

        public bool GetEnPassantable()
        {
            return IsEnPassantable;
        }

        public void SetUnEnPassantable()
        {
            IsEnPassantable = false;
        }
    }
}
