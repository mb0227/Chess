using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chess.GL
{
    public class Board
    {
        private Dictionary<string, Block> Blocks;
        private PlayerColor FirstPlayerColor;
        public bool is960;

        private Board OpeningState;
        private int QueenSideRookFile = -1;
        private int KingSideRookFile = -1;

       public Board(PlayerColor FirstPlayerColor, bool isChess960 = false)
       {
           Blocks = new Dictionary<string, Block>();
           this.FirstPlayerColor = FirstPlayerColor;
           is960 = isChess960;
           if (isChess960)
           {
               InitializeChess960Board();
               OpeningState = this;
               AssignRookPositions();
            }
           else
           {
               InitializeStandardChessBoard();
           }
       }

       private void InitializeChess960Board()
       {
           int[] backRankPositions = GenerateRandomBackRank();

           for (int rank = 0; rank < 8; rank++)
           {
               for (int file = 0; file < 8; file++)
               {
                   string blockKey = GetBlockKey(rank, file);
                   Blocks[blockKey] = CreateChess960BlockAtPosition(rank, file, backRankPositions);
               }
           }
       }

       private int[] GenerateRandomBackRank()
       {
           Random random = new Random();
           int[] backRankPositions = new int[8];

           int lightSquareBishop = random.Next(0, 4) * 2;
           int darkSquareBishop = random.Next(0, 4) * 2 + 1;

           List<int> remainingFiles = Enumerable.Range(0, 8).ToList();
           backRankPositions[lightSquareBishop] = (int)PieceType.Bishop;
           remainingFiles.Remove(lightSquareBishop);

           backRankPositions[darkSquareBishop] = (int)PieceType.Bishop;
           remainingFiles.Remove(darkSquareBishop);

           int queenFile = remainingFiles[random.Next(remainingFiles.Count)];
           backRankPositions[queenFile] = (int)PieceType.Queen;
           remainingFiles.Remove(queenFile);

           for (int i = 0; i < 2; i++)
           {
               int knightFile = remainingFiles[random.Next(remainingFiles.Count)];
               backRankPositions[knightFile] = (int)PieceType.Knight;
               remainingFiles.Remove(knightFile);
           }

           int kingFile = remainingFiles[random.Next(remainingFiles.Count)];
           backRankPositions[kingFile] = (int)PieceType.King;
           remainingFiles.Remove(kingFile);

           backRankPositions[remainingFiles[0]] = (int)PieceType.Rook;
           backRankPositions[remainingFiles[1]] = (int)PieceType.Rook;
           return backRankPositions;
       }
        
        private void AssignRookPositions()
        {
            int rooksFound = 0; 
            for (int i = 0; i < 8; i++)
            {
                if (this.GetBlock(0, i).GetPiece().GetPieceType() == PieceType.Rook)
                {
                    if (FirstPlayerColor == PlayerColor.White)
                    {
                        if (QueenSideRookFile == -1)
                            QueenSideRookFile = i;
                        else
                            KingSideRookFile = i;
                    }
                    else if(FirstPlayerColor == PlayerColor.Black)
                    {
                        if (KingSideRookFile == -1)
                            KingSideRookFile = i;
                        else
                            QueenSideRookFile = i;
                    }
                    rooksFound++;
                }
                if (rooksFound == 2) break;
            }
        }

        private Block CreateChess960BlockAtPosition(int rank, int file, int[] backRankPositions)
        {
            Block block = new Block(rank, file);
            PieceColor whiteColor = PieceColor.White;
            PieceColor blackColor = PieceColor.Black;

            int whitePawnsRank = FirstPlayerColor == PlayerColor.White ? 6 : 1;
            int whitePiecesRank = FirstPlayerColor == PlayerColor.White ? 7 : 0;
            int blackPawnsRank = FirstPlayerColor == PlayerColor.White ? 1 : 6;
            int blackPiecesRank = FirstPlayerColor == PlayerColor.White ? 0 : 7;

            if (rank == whitePiecesRank)
            {
                switch ((PieceType)backRankPositions[file])
                {
                    case PieceType.Rook:
                        block.SetPiece(new Rook(whiteColor, PieceType.Rook, true));
                        break;
                    case PieceType.Knight:
                        block.SetPiece(new Knight(whiteColor, PieceType.Knight, true));
                        break;
                    case PieceType.Bishop:
                        block.SetPiece(new Bishop(whiteColor, PieceType.Bishop, true));
                        break;
                    case PieceType.Queen:
                        block.SetPiece(new Queen(whiteColor, PieceType.Queen, true));
                        break;
                    case PieceType.King:
                        block.SetPiece(new King(whiteColor, PieceType.King, true));
                        break;
                }
            }
            else if (rank == blackPiecesRank)
            {
                switch ((PieceType)backRankPositions[file])
                {
                    case PieceType.Rook:
                        block.SetPiece(new Rook(blackColor, PieceType.Rook, true));
                        break;
                    case PieceType.Knight:
                        block.SetPiece(new Knight(blackColor, PieceType.Knight, true));
                        break;
                    case PieceType.Bishop:
                        block.SetPiece(new Bishop(blackColor, PieceType.Bishop, true));
                        break;
                    case PieceType.Queen:
                        block.SetPiece(new Queen(blackColor, PieceType.Queen, true));
                        break;
                    case PieceType.King:
                        block.SetPiece(new King(blackColor, PieceType.King, true));
                        break;
                }
            }
            else if (rank == whitePawnsRank)
            {
                block.SetPiece(new Pawn(whiteColor, PieceType.Pawn, true));
            }
            else if (rank == blackPawnsRank)
            {
                block.SetPiece(new Pawn(blackColor, PieceType.Pawn, true));
            }

            return block;
        }


        private void InitializeStandardChessBoard()
       {
           int blackPawnsRank, blackPiecesRank, whitePawnsRank, whitePiecesRank;
           int kingIndex, queenIndex;

           if (FirstPlayerColor == PlayerColor.White)
           {
               (blackPawnsRank, blackPiecesRank, whitePawnsRank, whitePiecesRank) = (1, 0, 6, 7);
               (kingIndex, queenIndex) = (4, 3);
           }
           else
           {
               (blackPawnsRank, blackPiecesRank, whitePawnsRank, whitePiecesRank) = (6, 7, 1, 0);
               (kingIndex, queenIndex) = (3, 4);
           }

           for (int rank = 0; rank < 8; rank++)
           {
               for (int file = 0; file < 8; file++)
               {
                   string blockKey = GetBlockKey(rank, file);
                   Blocks[blockKey] = CreateBlockAtPosition(rank, file, blackPawnsRank, blackPiecesRank,
                                                            whitePawnsRank, whitePiecesRank,
                                                            kingIndex, queenIndex);
               }
           }
       }
        
        private Block CreateBlockAtPosition(int rank, int file, int blackPawnsRank, int blackPiecesRank, int whitePawnsRank, int whitePiecesRank, int kingIndex, int queenIndex)
        {
            Block block = new Block(rank, file);

            if (kingIndex == 4 && queenIndex == 3 && rank == whitePiecesRank)
            {
                if (file == 4)
                    block.SetPiece(new King(PieceColor.White, PieceType.King, true));
                if (file == 3)
                    block.SetPiece(new Queen(PieceColor.White, PieceType.Queen, true));
            }
            else if (kingIndex == 3 && queenIndex == 4 && rank == whitePiecesRank)
            {
                if (file == 3)
                    block.SetPiece(new King(PieceColor.White, PieceType.King, true));
                if (file == 4)
                    block.SetPiece(new Queen(PieceColor.White, PieceType.Queen, true));
            }
            if (kingIndex == 4 && queenIndex == 3 && rank == blackPiecesRank)
            {
                if (file == 4)
                    block.SetPiece(new King(PieceColor.Black, PieceType.King, true));
                if (file == 3)
                    block.SetPiece(new Queen(PieceColor.Black, PieceType.Queen, true));
            }
            else if (kingIndex == 3 && queenIndex == 4 && rank == blackPiecesRank)
            {
                if (file == 3)
                    block.SetPiece(new King(PieceColor.Black, PieceType.King, true));
                if (file == 4)
                    block.SetPiece(new Queen(PieceColor.Black, PieceType.Queen, true));
            }

            if (rank == whitePiecesRank)
            {
                switch (file)
                {
                    case 0: block.SetPiece(new Rook(PieceColor.White, PieceType.Rook, true)); break;
                    case 1: block.SetPiece(new Knight(PieceColor.White, PieceType.Knight, true)); break;
                    case 2: block.SetPiece(new Bishop(PieceColor.White, PieceType.Bishop, true)); break;
                    case 5: block.SetPiece(new Bishop(PieceColor.White, PieceType.Bishop, true)); break;
                    case 6: block.SetPiece(new Knight(PieceColor.White, PieceType.Knight, true)); break;
                    case 7: block.SetPiece(new Rook(PieceColor.White, PieceType.Rook, true)); break;
                }
            }
            else if (rank == whitePawnsRank)
            {
                block.SetPiece(new Pawn(PieceColor.White, PieceType.Pawn, true));
            }

            if (rank == blackPiecesRank)
            {
                switch (file)
                {
                    case 0: block.SetPiece(new Rook(PieceColor.Black, PieceType.Rook, true)); break;
                    case 1: block.SetPiece(new Knight(PieceColor.Black, PieceType.Knight, true)); break;
                    case 2: block.SetPiece(new Bishop(PieceColor.Black, PieceType.Bishop, true)); break;
                    case 5: block.SetPiece(new Bishop(PieceColor.Black, PieceType.Bishop, true)); break;
                    case 6: block.SetPiece(new Knight(PieceColor.Black, PieceType.Knight, true)); break;
                    case 7: block.SetPiece(new Rook(PieceColor.Black, PieceType.Rook, true)); break;
                }
            }
            else if (rank == blackPawnsRank)
            {
                block.SetPiece(new Pawn(PieceColor.Black, PieceType.Pawn, true));
            }

            return block;
        }

        // Getters
        public Block GetBlock(int rank, int file)
        {
            if (WithinBounds(rank, file))
                return Blocks[GetBlockKey(rank, file)];
            return null;
        }

        public Block GetBlock(Piece piece)
        {
            return Blocks.Values.FirstOrDefault(block => block.GetPiece() == piece);
        }

        private string GetBlockKey(int rank, int file)
        {
            return $"{rank},{file}";
        }

        public bool GetFinalStatus(PieceColor pieceColor)
        {
            foreach (var block in Blocks.Values)
            {
                Piece piece = block.GetPiece();
                if (piece == null || piece.GetColor() != pieceColor)
                    continue;
                if (piece?.GetPossibleMoves(this).Count > 0)
                    return false;
            }
            return true;
        }

        public Dictionary<string, Block> GetBlocks()
        {
            return Blocks;
        }

        public PlayerColor GetFirstPlayerColor()
        {
            return FirstPlayerColor;
        }

        // State Related Functions
        public Block FindKing(PieceColor pieceColor)
        {
            return Blocks.Values.FirstOrDefault(block =>
            {
                Piece piece = block.GetPiece();
                return piece?.GetPieceType() == PieceType.King && piece.GetColor() == pieceColor;
            });
        }

        public bool IsKingInCheck(PieceColor pieceColor)
        {
            Block kingBlock = FindKing(pieceColor);
            if (kingBlock == null)
                return false;

            foreach (var block in Blocks.Values)
            {
                Piece attackingPiece = block.GetPiece();
                if (attackingPiece == null || attackingPiece.GetColor() == pieceColor)
                    continue;

                if (attackingPiece.GetPieceType() == PieceType.Pawn && attackingPiece.IsAttackingKing(this, kingBlock))
                    return true;

                if (attackingPiece.CanAttack(kingBlock, this))
                    return true;
            }
            return false;
        }

        public bool IsSafeMove(Piece pieceToMove, Block endBlock)
        {
            Block startBlock = GetBlock(pieceToMove);
            if (startBlock == null || endBlock == null)
                return false;

            Piece capturedPiece = endBlock.GetPiece();
            startBlock.SetPiece(null);
            endBlock.SetPiece(pieceToMove);

            bool isSafe = !IsKingInCheck(pieceToMove.GetColor());

            // Restore the original board state
            endBlock.SetPiece(capturedPiece);
            startBlock.SetPiece(pieceToMove);

            return isSafe;
        }

        public bool IsCheck(Piece pieceToMove, Block endBlock)
        {
            Block startBlock = GetBlock(pieceToMove);
            if (startBlock == null || endBlock == null)
                return false;

            Piece capturedPiece = endBlock.GetPiece();
            startBlock.SetPiece(null);
            endBlock.SetPiece(pieceToMove);

            bool isCheck = IsKingInCheck(pieceToMove.GetColor() == PieceColor.White ? PieceColor.Black : PieceColor.White);

            startBlock.SetPiece(pieceToMove);
            endBlock.SetPiece(capturedPiece);

            return isCheck;
        }

        public bool IsUnderAttack(Block block, PieceColor pieceColor)
        {
            foreach (var b in Blocks.Values)
            {
                Piece piece = b.GetPiece();
                if (piece == null || piece.GetColor() == pieceColor)
                    continue;

                if (piece.CanAttack(block, this))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSquareSafeForCastling(Block block, PieceColor pieceColor)
        {
            Block kingBlock = FindKing(pieceColor);
            foreach (var b in Blocks.Values)
            {
                Piece piece = b.GetPiece();
                if (piece == null || piece.GetColor() == pieceColor)
                    continue;

                if (piece.GetColor() != pieceColor
                    && piece.GetPieceType() == PieceType.Pawn
                    && piece.IsAttackingKing(this, block))
                    return false;

                if (piece.CanAttack(block, this))
                {
                    return false;
                }
            }
            return true;
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

        // Board Related Functions
        public bool WithinBounds(int rank, int file)
        {
            return rank >= 0 && rank < 8 && file >= 0 && file < 8;
        }

        public Piece OtherPieceCanMove(Piece piece, Block endBlock)
        {
            // Now this function is used to check two pieces of the same types can move to
            // same block, in parameter we are getting the actual piece that moved and 
            // the second parameter is the end block where the piece is moved
            foreach (Block block in Blocks.Values)
            {
                if (!block.IsEmpty() && block.GetPiece().GetColor() == piece.GetColor()
                    && block.GetPiece() != piece && block.GetPiece().GetPieceType() == piece.GetPieceType())
                {
                    Piece otherPiece = block.GetPiece();
                    if (otherPiece.CanAttack(endBlock, this))
                    {
                        return otherPiece;
                    }
                }
            }
            return null;
        }

        public int GetFileInInt(string file)
        {
            switch (file)
            {
                case "a": return 0;
                case "b": return 1;
                case "c": return 2;
                case "d": return 3;
                case "e": return 4;
                case "f": return 5;
                case "g": return 6;
                case "h": return 7;
                default: return -1;
            }
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

        public Block FindRookUsingOpeningState(bool isKingSide, PieceColor pieceColor)
        {
            int rank = pieceColor == PieceColor.White ? 7 : 0;
            if (FirstPlayerColor != PlayerColor.White) 
                rank = pieceColor == PieceColor.White ? 0 : 7;
            return isKingSide ? GetBlock(rank, KingSideRookFile) : GetBlock(rank, QueenSideRookFile);
        }


        public void DisplayBoard()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Block block = Blocks[GetBlockKey(rank, file)];
                    if (block.IsEmpty())
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Piece piece = block.GetPiece();
                        if (piece.GetColor() == PieceColor.White)
                            Console.Write("W ");
                        else
                            Console.Write("B ");
                        Console.Write(piece.GetPieceType().ToString() + " ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}