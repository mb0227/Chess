﻿using Chess.DS;
using System.Windows.Documents;
using System;

namespace Chess.GL
{
    public enum GameStatus
    {
        ACTIVE,
        BLACK_WIN,
        WHITE_WIN,
        FORFEIT,
        STALEMATE,
        RESIGNATION
    }

    public enum MoveType
    {
        Normal,
        Kill,
        Check,
        Checkmate,
        Stalemate,
        Promotion,
        EnPassant,
        Castling
    }

    public enum CastlingType
    {
        ShortCastle,
        LongCastle
    }

    public class Game
    {
        private Player PlayerOne;
        private Player PlayerTwo;
        private Player CurrentMove;
        private Board Board;
        private Stack Moves;
        private Move FirstPlayerMove;
        private Move SecondPlayerMove;
        private bool IsGameOver;
        private GameStatus Status;

        private static Game GameInstance;

        public static Game MakeGame(Player playerOne, Player playerTwo) //Singleton Pattern
        {
            if (GameInstance == null)
            {
                GameInstance = new Game(playerOne, playerTwo);
            }
            return GameInstance;
        }

        private Game(Player playerOne, Player playerTwo)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            Board = new Board(playerOne.GetColor());
            Moves = new Stack();
            IsGameOver = false;
            Status = GameStatus.ACTIVE;
            CurrentMove = PlayerOne.GetColor().ToString().Trim() == "White" ? PlayerOne : PlayerTwo;
            Move.SetBoard(Board);
        }

        public void MakeMove(int prevRank, int prevFile, int newRank, int newFile, MoveType moveType, PieceType pieceType = PieceType.Queen)
        {
            if (!IsGameOver && Status == GameStatus.ACTIVE && Board.WithinBounds(prevRank, prevFile) && Board.WithinBounds(newRank, newFile))
            {
                if(Board.GetBlock(prevRank, prevFile).IsEmpty())
                {
                    Console.WriteLine("No piece at the given position.");
                    return;
                }
                Block prevBlock = Board.GetBlock(prevRank, prevFile);
                Piece pieceAtPrev = prevBlock.GetPiece();
                Block newBlock = Board.GetBlock(newRank, newFile);
                if(newBlock.GetPiece() != null && newBlock.GetPiece().GetColor() == pieceAtPrev.GetColor())
                {
                    Console.WriteLine("Cannot place pieces of same color on eachother.");
                    return;
                }
                if (pieceAtPrev.GetColor().ToString().Trim() != CurrentMove.GetColor().ToString().Trim())
                {
                    Console.WriteLine("It is not your turn.");
                    return;
                }
                Console.WriteLine("Board Before");
                //Board.DisplayBoard();
                if (Board.GetBlock(newRank, newFile).GetPiece() == null)
                {
                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev);
                    if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        // here update piece at piece at prev to a new piece
                        pieceAtPrev = new Piece(pieceAtPrev.GetColor(), pieceType, true);
                    }
                    else if(pieceAtPrev.GetPieceType() == PieceType.Pawn && (prevRank == 1 || prevRank == 6))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        pawn.SetHasMoved();
                        if (Math.Abs(prevRank - newRank) == 2)
                        {
                            pawn.SetEnPassantable();
                        }
                    }
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else
                {
                    Piece pieceAtNew = newBlock.GetPiece();
                    pieceAtNew.Kill();

                    if(moveType != MoveType.Promotion)AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew);
                    else AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White) || (prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        pieceAtPrev = new Piece(pieceAtPrev.GetColor(), pieceType, true);
                    }
                    if(CurrentMove == PlayerOne) PlayerTwo.KillPiece(pieceAtNew);
                    else PlayerOne.KillPiece(pieceAtNew);


                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                Console.WriteLine("Board After");
                //Board.DisplayBoard();
                if (CurrentMove == PlayerTwo)
                {
                    Moves.Push(FirstPlayerMove, SecondPlayerMove);
                    CurrentMove = PlayerOne;
                }
                else
                {
                    CurrentMove = PlayerTwo;
                }
                DisplayMoves();
            }
        }

        public void AddMove(Block prevBlock, Block newBlock, MoveType moveType, Piece prevBlockPiece, Piece capturedPiece = null, PieceType promotedPieceType = PieceType.Queen)
        {
            Move move;
            
            if (moveType == MoveType.Normal) move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece);
            else move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType, promotedPieceType);
            if (CurrentMove == PlayerOne) FirstPlayerMove = move;
            else SecondPlayerMove = move;
        }

        public void DisplayMoves()
        {
            Moves.Display();
        }

        public Player GetPlayerOne()
        {
            return PlayerOne;
        }

        public Player GetPlayerTwo()
        {
            return PlayerTwo;
        }

        public Board GetBoard()
        {
            return Board;
        }

        public Stack GetMoves()
        {
            return Moves;
        }

        public bool GetIsGameOver()
        {
            return IsGameOver;
        }

        public GameStatus GetStatus()
        {
            return Status;
        }

        public Player GetCurrentPlayer()
        {
            return CurrentMove;
        }

        public void UpdateStatus(GameStatus status)
        {
            Status = status;
        }

        public void SetIsGameOver(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }

        public bool IsTurn(string pieceColor)
        {
            return CurrentMove.GetColor().ToString().Trim() == pieceColor.Trim();
        }

        public static PieceType GetPieceTypeByString(string pieceName)
        {
            switch(pieceName)
            {
                case "queen":
                    return PieceType.Queen;
                case "bishop":
                    return PieceType.Bishop;
                case "rook":
                    return PieceType.Rook;
                case "knight":
                    return PieceType.Knight;
                default:
                    return PieceType.Queen;
            }
        }
    }
}
