using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Computer : Player
    {
        public Computer(PlayerColor color) : base(color)
        {
            PlayerType = PlayerType.Computer;
        }

        private static Board Board;

        public static void SetBoard(Board board)
        {
            Board = board;
        }

        public List<Move> GetAllPossibleMoves(PieceColor pieceColor)
        {
            List<Move> moves = new List<Move>();

            foreach (var block in Board.GetBlocks().Values)
            {
                var piece = block.GetPiece();
                if (piece?.GetColor() == pieceColor)
                {
                    var possibleMoves = piece.GetPossibleMoves(Board) ?? new List<Move>();
                    moves.AddRange(possibleMoves);
                }
            }
            return moves;
        }

        private int EvaluateBoard(PlayerColor aiColor)
        {
            int score = 0;

            foreach (var block in Board.GetBlocks().Values)
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
    }
}
