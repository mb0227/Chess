using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Board
    {
        private Block[,] Blocks;
        private PlayerColor FirstPlayerColor;

        public Board(PlayerColor FirstPlayerColor)
        {
            Blocks = new Block[8, 8];
            this.FirstPlayerColor = FirstPlayerColor;
            int blackPiecesRank, blackPawnsRank, whitePawnsRank, whitePiecesRank;

            if (FirstPlayerColor == PlayerColor.White)
            {
                (blackPawnsRank, blackPiecesRank, whitePawnsRank, whitePiecesRank) = (1, 0, 6, 7);
            }
            else
            {
                (blackPawnsRank, blackPiecesRank, whitePawnsRank, whitePiecesRank) = (6, 7, 1, 0);
            }

            // Creating White Pieces
            Blocks[whitePiecesRank, 0] = new Block(whitePiecesRank, 0, new Rook(PieceColor.White, PieceType.Rook, true));
            Blocks[whitePiecesRank, 1] = new Block(whitePiecesRank, 1, new Knight(PieceColor.White, PieceType.Knight, true));
            Blocks[whitePiecesRank, 2] = new Block(whitePiecesRank, 2, new Bishop(PieceColor.White, PieceType.Bishop, true));
            Blocks[whitePiecesRank, 3] = new Block(whitePiecesRank, 3, new Queen(PieceColor.White, PieceType.Queen, true));
            Blocks[whitePiecesRank, 4] = new Block(whitePiecesRank, 4, new King(PieceColor.White, PieceType.King, true));
            Blocks[whitePiecesRank, 5] = new Block(whitePiecesRank, 5, new Bishop(PieceColor.White, PieceType.Bishop, true));
            Blocks[whitePiecesRank, 6] = new Block(whitePiecesRank, 6, new Knight(PieceColor.White, PieceType.Knight, true));
            Blocks[whitePiecesRank, 7] = new Block(whitePiecesRank, 7, new Rook(PieceColor.White, PieceType.Rook, true));
            for (int file = 0; file < 8; file++)
            {
                Blocks[whitePawnsRank, file] = new Block(whitePawnsRank, file, new Pawn(PieceColor.White, PieceType.Pawn, true));
            }

            // Creating Black Pieces
            Blocks[blackPiecesRank, 0] = new Block(blackPiecesRank, 0, new Rook(PieceColor.Black, PieceType.Rook, true));
            Blocks[blackPiecesRank, 1] = new Block(blackPiecesRank, 1, new Knight(PieceColor.Black, PieceType.Knight, true));
            Blocks[blackPiecesRank, 2] = new Block(blackPiecesRank, 2, new Bishop(PieceColor.Black, PieceType.Bishop, true));
            Blocks[blackPiecesRank, 3] = new Block(blackPiecesRank, 3, new Queen(PieceColor.Black, PieceType.Queen, true));
            Blocks[blackPiecesRank, 4] = new Block(blackPiecesRank, 4, new King(PieceColor.Black, PieceType.King, true));
            Blocks[blackPiecesRank, 5] = new Block(blackPiecesRank, 5, new Bishop(PieceColor.Black, PieceType.Bishop, true));
            Blocks[blackPiecesRank, 6] = new Block(blackPiecesRank, 6, new Knight(PieceColor.Black, PieceType.Knight, true));
            Blocks[blackPiecesRank, 7] = new Block(blackPiecesRank, 7, new Rook(PieceColor.Black, PieceType.Rook, true));
            for (int file = 0; file < 8; file++)
            {
                Blocks[blackPawnsRank, file] = new Block(blackPawnsRank, file, new Pawn(PieceColor.Black, PieceType.Pawn, true));
            }

            // Creating Empty Blocks
            for (int rank = 2; rank < 6; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Blocks[rank, file] = new Block(rank, file);
                }
            }
        }

        public Block GetBlock(int rank, int file)
        {
            return Blocks[rank, file];
        }

        public Block GetBlock(Piece piece)
        {
            // search the board and return the block that contains the piece
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (Blocks[rank, file].GetPiece() == piece)
                    {
                        return Blocks[rank, file];
                    }
                }
            }
            return null;
        }

        public void DisplayBoard()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (Blocks[rank, file].IsEmpty())
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Piece piece = Blocks[rank, file].GetPiece();
                        if (piece.GetColor() == PieceColor.White)
                        {
                            Console.Write("W ");
                        }
                        else
                        {
                            Console.Write("B ");
                        }
                        Console.Write(piece.GetPieceType().ToString() + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        public PlayerColor GetFirstPlayerColor()
        {
            return FirstPlayerColor;
        }

        public bool WithinBounds(int rank, int file)
        {
            return rank >= 0 && rank < 8 && file >= 0 && file < 8;
        }

        public int TranslateRank(int rank)
        {
            return FirstPlayerColor == PlayerColor.White ? 8 - rank : rank + 1;
        }

        public char TranslateFile(int file)
        {
            if (FirstPlayerColor == PlayerColor.White)
            {
                return ((char)('a' + file));
            }
            else
            {
                return ((char)('h' - file));
            }
        }

        public int GetFileInInt(string file)
        {
            switch (file)
            {
                case "a":
                    return 0;
                case "b":
                    return 1;
                case "c":
                    return 2;
                case "d":
                    return 3;
                case "e":
                    return 4;
                case "f":
                    return 5;
                case "g":
                    return 6;
                case "h":
                    return 7;
                default:
                    return -1;
            }
        }

        public bool IsKingInCheck(PieceColor pieceColor)
        {
            Block kingBlock = FindKing(pieceColor);
            // Console.WriteLine(kingBlock?.ToString());
            foreach (var block in Blocks)
            {
                Piece attackingPiece = block.GetPiece();
                if (attackingPiece == null || attackingPiece.GetColor() == pieceColor || kingBlock == null)
                    continue;

                if (attackingPiece.IsAttackingKing(this, kingBlock))
                {
                    return true;
                }
            }
            return false;
        }

        public Block FindKing(PieceColor pieceColor)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = Blocks[rank, file].GetPiece();
                    if (piece != null && piece.GetPieceType() == PieceType.King && piece.GetColor() == pieceColor)
                    {
                        return Blocks[rank, file];
                    }
                }
            }
            return null;
        }

        public bool IsSafeMove(Piece pieceToMove, Block endBlock)
        {
            // this function will check if the move is safe or not (safe here means if making this move will leave your king in check)
            // we will make the move and check if the king is in check or not
            Block startBlock = GetBlock(pieceToMove);
            Piece capturedPiece = endBlock.GetPiece();
            endBlock.SetPiece(pieceToMove);
            startBlock.SetPiece(null);
            bool isSafe = !IsKingInCheck(pieceToMove.GetColor());
            startBlock.SetPiece(pieceToMove);
            endBlock.SetPiece(capturedPiece);
            return isSafe;
        }

        public bool IsCheck(Piece pieceToMove, Block endBlock)
        {
            Block startBlock = GetBlock(pieceToMove);
            Piece capturedPiece = endBlock.GetPiece();
            endBlock.SetPiece(pieceToMove);
            startBlock.SetPiece(null);
            bool isCheck = IsKingInCheck(pieceToMove.GetColor() == PieceColor.White ? PieceColor.Black: PieceColor.White );
            startBlock.SetPiece(pieceToMove);
            endBlock.SetPiece(capturedPiece);
            return isCheck;
        }

        public bool IsUnderAttack(Block block, PieceColor pieceColor)
        {
            foreach (var b in Blocks)
            {
                Piece piece = b.GetPiece();
                if (piece == null || piece.GetColor() == pieceColor)
                    continue;

                if (piece.CanAttack(block, this))
                    return true;
            }
            return false;
        }

        public bool IsPathClear(int startRank, int startFile, int endRank, int endFile)
        {
            int rankStep = (endRank - startRank) != 0 ? (endRank - startRank) / Math.Abs(endRank - startRank) : 0;
            int fileStep = (endFile - startFile) != 0 ? (endFile - startFile) / Math.Abs(endFile - startFile) : 0;

            int currentRank = startRank + rankStep;
            int currentFile = startFile + fileStep;

            while (currentRank != endRank || currentFile != endFile)
            {
                if (!GetBlock(currentRank, currentFile).IsEmpty())
                    return false;

                currentRank += rankStep;
                currentFile += fileStep;
            }

            return true;
        }


        //public bool IsPlayerInCheck(PieceColor pieceColor)
        //{
        //    if (pieceColor.ToString() == FirstPlayerColor.ToString() && PlayerOne.IsInCheck())
        //    {
        //        return true;
        //    }
        //    else if (pieceColor.ToString() == PlayerTwo.GetColor().ToString() && PlayerTwo.IsInCheck())
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool IsCheckMate(PieceColor pieceColor)
        //{
        //    int count = 0;
        //    foreach (var block in Blocks)
        //    {
        //        Piece piece = block.GetPiece();
        //        if (piece == null || piece.GetColor() != pieceColor)
        //            continue;
        //        count += piece.GetPossibleMoves(this).Count;
        //    }
        //    if (count == 0)
        //    {
        //        Console.WriteLine("Checkmate");
        //        return true;
        //    }
        //    else Console.WriteLine("Not Checkmate");
        //    Console.WriteLine("Possible Moves " + count);
        //    return false;
        //}
    }
}
