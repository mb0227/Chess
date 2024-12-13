using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.GL
{
    public class Board
    {
        // Graph representation using an adjacency list
        private Dictionary<string, Block> Blocks;
        private PlayerColor FirstPlayerColor;

        public Board(PlayerColor FirstPlayerColor)
        {
            Blocks = new Dictionary<string, Block>();
            this.FirstPlayerColor = FirstPlayerColor;
            int blackPiecesRank, blackPawnsRank, whitePawnsRank, whitePiecesRank;
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

            // Create all possible board positions
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

        private Block CreateBlockAtPosition(int rank, int file, int blackPawnsRank, int blackPiecesRank,
                                            int whitePawnsRank, int whitePiecesRank,
                                            int kingIndex, int queenIndex)
        {
            Block block = new Block(rank, file);

            if (kingIndex == 4 && queenIndex == 3 && rank == whitePiecesRank)
            {
                if(file == 4)
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

        public Block GetBlock(int rank, int file)
        {
           return Blocks[GetBlockKey(rank, file)];
        }

        public Block GetBlock(Piece piece)
        {
            return Blocks.Values.FirstOrDefault(block => block.GetPiece() == piece);
        }

        private string GetBlockKey(int rank, int file)
        {
            return $"{rank},{file}";
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

        // Most other methods remain largely the same, just replace array iterations with dictionary iterations
        public PlayerColor GetFirstPlayerColor()
        {
            return FirstPlayerColor;
        }

        public bool WithinBounds(int rank, int file)
        {
            return rank >= 0 && rank < 8 && file >= 0 && file < 8;
        }

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

        // Additional helper methods to maintain previous functionality
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

        public List<Move> GetAllPossibleMoves(PieceColor pieceColor)
        {
            List<Move> moves = new List<Move>();

            foreach (var block in Blocks.Values)
            {
                if(block.GetPiece()?.GetColor() == pieceColor)
                {
                    moves.AddRange(block.GetPiece().GetPossibleMoves(this));
                }
            }
            return moves;
        }

        private int EvaluateBoard(PlayerColor aiColor)
        {
            int score = 0;

            foreach (var block in Blocks.Values)
            {
                if (!block.IsEmpty())
                {
                    Piece piece = block.GetPiece();
                    int pieceValue = GetPieceValue(piece.GetPieceType());
                    int positionalBonus = GetPositionalValue(piece, block);

                    score += (piece.GetColor().ToString() == aiColor.ToString())
                             ? (pieceValue + positionalBonus)
                             : -(pieceValue + positionalBonus);
                }
            }
            return score;
        }

        private int GetPositionalValue(Piece piece, Block block)
        {
            // Reward control of the center
            int[,] positionalValues = new int[8, 8]
            {
                { 1, 1,  1,  1,  1,  1, 1, 1 },
                { 1, 5,  5,  5,  5,  5, 5, 1 },
                { 1, 5, 10, 10, 10, 10, 5, 1 },
                { 1, 5, 10, 20, 20, 10, 5, 1 },
                { 1, 5, 10, 20, 20, 10, 5, 1 },
                { 1, 5, 10, 10, 10, 10, 5, 1 },
                { 1, 5,  5,  5,  5,  5, 5, 1 },
                { 1, 1,  1,  1,  1,  1, 1, 1 }
            };

            return positionalValues[block.GetRank(), block.GetFile()];
        }


        private int GetPieceValue(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return 1;
                case PieceType.Knight:
                    return 3;
                case PieceType.Bishop:
                    return 3;
                case PieceType.Rook:
                    return 5;
                case PieceType.Queen:
                    return 9;
                case PieceType.King:
                    return 90;
                default:
                    return 0;
            }
        }

        private void SimulateMove(Block startBlock, Block endBlock, out Piece capturedPiece)
        {
            capturedPiece = endBlock.GetPiece();
            endBlock.SetPiece(startBlock.GetPiece());
            startBlock.SetPiece(null);
        }

        private void UndoSimulatedMove(Block startBlock, Block endBlock, Piece capturedPiece)
        {
            startBlock.SetPiece(endBlock.GetPiece());
            endBlock.SetPiece(capturedPiece);
        }

        private int Minimax(int depth, bool isMaximizing, PlayerColor aiColor, PlayerColor opponentColor, int alpha, int beta)
        {
            if (depth == 0)
                return EvaluateBoard(aiColor);

            PieceColor pieceColor = (isMaximizing ? aiColor : opponentColor) == PlayerColor.White ? PieceColor.White : PieceColor.Black;

            List<Move> moves = GetAllPossibleMoves(pieceColor);

            int bestValue = isMaximizing ? int.MinValue : int.MaxValue;

            foreach (var move in moves)
            {
                SimulateMove(move.GetStartBlock(), move.GetEndBlock(), out Piece capturedPiece);

                int boardValue = Minimax(depth - 1, !isMaximizing, aiColor, opponentColor, alpha, beta);

                UndoSimulatedMove(move.GetStartBlock(), move.GetEndBlock(), capturedPiece);

                if (isMaximizing)
                {
                    bestValue = Math.Max(bestValue, boardValue);
                    alpha = Math.Max(alpha, boardValue);
                }
                else
                {
                    bestValue = Math.Min(bestValue, boardValue);
                    beta = Math.Min(beta, boardValue);
                }

                if (beta <= alpha)
                    break;
            }

            return bestValue;
        }

        public Move MakeAIMove(PlayerColor aiColor, int depth)
        {
            PlayerColor opponentColor = aiColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            PieceColor aiPieceColor = aiColor == PlayerColor.White ? PieceColor.White : PieceColor.Black;

            List<Move> moves = GetAllPossibleMoves(aiPieceColor);

            int bestValue = int.MinValue;
            Move bestMove = null;

            foreach (var move in moves)
            {
                // Simulate the move
                SimulateMove(move.GetStartBlock(), move.GetEndBlock(), out Piece capturedPiece);

                // Evaluate the board using Minimax
                int boardValue = Minimax(depth - 1, false, aiColor, opponentColor, int.MinValue, int.MaxValue);

                // Undo the simulated move
                UndoSimulatedMove(move.GetStartBlock(), move.GetEndBlock(), capturedPiece);

                // Keep track of the best move
                if (boardValue > bestValue)
                {
                    bestValue = boardValue;
                    bestMove = move;
                }
            }
            
            Console.WriteLine("AI's move: " + bestMove.GetNotation());
            return bestMove;
        }

        public void PlayMove(Move move)
        {
            if (move != null)
            {
                Block startBlock = move.GetStartBlock();
                Block endBlock = move.GetEndBlock();

                endBlock.SetPiece(startBlock.GetPiece());
                startBlock.SetPiece(null);
            }
        }

        public Dictionary<string, Block> GetBlocks()
        {
            return Blocks;
        }
    }
}