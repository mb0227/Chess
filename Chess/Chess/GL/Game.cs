﻿using Chess.DS;
using System;
using System.Collections.Generic;

namespace Chess.GL
{
    public class Game
    {
        // Class Members
        private Player PlayerOne;
        private Player PlayerTwo;
        private Player CurrentMove;
        private Board Board;
        private Stack Moves;
        private Move FirstPlayerMove;
        private Move SecondPlayerMove;
        private bool IsGameOver;
        private GameStatus Status;
        private MovesStack MovesStack;

        private int Difficulty;

        // bool: true for add, false for remove
        public event Action<string, bool> MoveMade;
        public event Action<string, bool> PlayerOneDeadPiecesChanged;
        public event Action<string, bool> PlayerTwoDeadPiecesChanged;

        private static Game GameInstance;

        // Constructors
        private Game(Player playerOne, Player playerTwo, bool is960)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            Board = new Board(playerOne.GetColor(), is960);
            Moves = new Stack();
            MovesStack = new MovesStack();
            IsGameOver = false;
            Status = GameStatus.ACTIVE;
            CurrentMove = PlayerOne.GetColor().ToString().Trim() == "White" ? PlayerOne : PlayerTwo;
            Move.SetBoard(Board);
            Computer.SetBoard(Board);
        }

        private Game(Player playerOne, Player playerTwo, int difficulty) : this(playerOne, playerTwo, false)
        {
            Difficulty = difficulty;
        }

        // Game Initiazlize Methods
        public static Game MakeGame(Player playerOne, Player playerTwo, bool is960) //Singleton Pattern
        {
            if (GameInstance == null)
            {
                GameInstance = new Game(playerOne, playerTwo, is960);
            }
            else
            {
                RestartGame();
                GameInstance = new Game(playerOne, playerTwo, is960);
            }
            return GameInstance;
        }

        public static Game MakeGame(Player playerOne, Player playerTwo, int difficulty)
        {
            if (GameInstance == null)
            {
                GameInstance = new Game(playerOne, playerTwo, difficulty);
            }
            else
            {
                RestartGame();
                GameInstance = new Game(playerOne, playerTwo, difficulty);
            }
            return GameInstance;
        }

        private static void RestartGame()
        {
            GameInstance = null;
        }

        // Move Functionality
        public Move MakeComputerMove()
        {
            Move computerMove = null;
            if (CurrentMove.GetPlayerType() == PlayerType.Computer && !IsGameOver)
            {
                Computer computer = (Computer)CurrentMove;
                computerMove = computer.MakeAIMove(CurrentMove.GetColor(), Difficulty); // first argument is player's color and second argument is the depth of the minimax algorithm
            }
            return computerMove;
        }

        public void MakeMove(int prevRank, int prevFile, int newRank, int newFile, MoveType moveType, PieceType pieceType = PieceType.Queen, int enPassantTargetRow = -1, CastlingType castlingType = CastlingType.None)
        {
            if (moveType == MoveType.Draw)
            {
                Move move = new Move(moveType);
                move.SetNotation("¹/₂");
                Moves.Push(move, move);
                MoveMade?.Invoke(Moves.Peek(), true);
                MovesStack.Push(move);
                return;
            }
            if (!IsGameOver && Status == GameStatus.ACTIVE && Board.WithinBounds(prevRank, prevFile) && Board.WithinBounds(newRank, newFile))
            {
                if (Board.GetBlock(prevRank, prevFile).IsEmpty() && moveType != MoveType.Castling)
                {
                    return;
                }
                
                Block prevBlock = Board.GetBlock(prevRank, prevFile);
                Piece pieceAtPrev = prevBlock.GetPiece();
                Block newBlock = Board.GetBlock(newRank, newFile);

                if(moveType == MoveType.Castling && GetBoard().is960)
                {
                    int rank = (CurrentMove.GetColor() == PlayerColor.White) ? 7 : 0;
                    if (PlayerOne.GetColor() == PlayerColor.Black)
                    {
                        rank = (CurrentMove.GetColor() == PlayerColor.White) ? 0 : 7;
                    }
                    prevBlock = Board.GetBlock(rank, prevRank);
                    pieceAtPrev = prevBlock.GetPiece();
                }

                if (moveType == MoveType.Promotion && pieceAtPrev.GetPieceType() == PieceType.Pawn)
                {
                    CheckForPromotionCheck(prevBlock, newBlock, pieceAtPrev, ref moveType, ref pieceType);
                }

                // update move movement
                if (pieceAtPrev.GetPieceType() == PieceType.Pawn)
                {
                    Pawn pawn = (Pawn)prevBlock.GetPiece();
                    pawn.SetPawnMoved();
                    if (prevRank == 1 || prevRank == 6)
                        pawn.PawnMoved(Math.Abs(prevRank - newRank));
                }
                else
                {
                    if (pieceAtPrev.GetPieceType() == PieceType.Rook)
                    {
                        Rook rook = (Rook)prevBlock.GetPiece();
                        rook.SetHasMoved();
                    }
                    else if (pieceAtPrev.GetPieceType() == PieceType.King)
                    {
                        King king = (King)prevBlock.GetPiece();
                        king.SetHasMoved();
                    }
                }

                if (Board.IsCheck(pieceAtPrev, newBlock))
                {
                    moveType = MoveType.Check;
                    King king = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.White ? PieceColor.Black : PieceColor.White).GetPiece();
                    king.SetCheck(true);
                }
                else
                {
                    King king = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.Black : PieceColor.White).GetPiece();
                    king.SetCheck(false);
                }

                if (moveType == MoveType.EnPassant && Board.GetBlock(enPassantTargetRow, newFile) != null)
                {
                    Block targetBlock = Board.GetBlock(enPassantTargetRow, newFile);
                    Piece pieceAtTarget = targetBlock.GetPiece();
                    pieceAtTarget?.Kill();

                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtTarget);

                    if (CurrentMove == PlayerOne)
                    {
                        PlayerTwo.KillPiece(pieceAtTarget);
                        PlayerTwoDeadPiecesChanged?.Invoke(PlayerTwo.GetFirstDeadPiece(), true);
                    }
                    else
                    {
                        PlayerOne.KillPiece(pieceAtTarget);
                        PlayerOneDeadPiecesChanged?.Invoke(PlayerOne.GetFirstDeadPiece(), true);
                    }
                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                    Board.GetBlock(enPassantTargetRow, newFile).SetPiece(null);
                }
                else if (moveType == MoveType.Castling)
                {
                    int rank, kingStartFile, kingEndFile, rookStartFile, rookEndFile;
                    if (!GetBoard().is960)
                    {
                        AddMove(prevBlock, newBlock, moveType, pieceAtPrev);
                        rank = (CurrentMove.GetColor() == PlayerColor.White) ? 7 : 0;
                        kingStartFile = prevFile;
                        rookStartFile = (castlingType == CastlingType.KingSideCastle) ? 7 : 0;
                        kingEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile + 2 : kingStartFile - 2;
                        rookEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile + 1 : kingStartFile - 1;

                        if (PlayerOne.GetColor() == PlayerColor.Black)
                        {
                            rank = (CurrentMove.GetColor() == PlayerColor.White) ? 0 : 7;
                            rookStartFile = (castlingType == CastlingType.KingSideCastle) ? 0 : 7;
                            kingEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile - 2 : kingStartFile + 2;
                            rookEndFile = (castlingType == CastlingType.KingSideCastle) ? kingStartFile - 1 : kingStartFile + 1;
                        }

                        if (!Board.is960 && !Board.WithinBounds(kingStartFile, kingEndFile) || !Board.WithinBounds(rookStartFile, rookEndFile))
                        {
                            return;
                        }
                    }
                    else
                    {
                        // now there is a slight change in logic and variables used here
                        // if the board is 960 instead of passing the rank before and after move
                        // I instead passed king's before and after file and rook's before and after file
                        // prevRank is king's current file, prevFile is king's target file, targetRank is rook's current file and targetFile is rook's target file
                        rank = (CurrentMove.GetColor() == PlayerColor.White) ? 7 : 0;
                        if (PlayerOne.GetColor() == PlayerColor.Black)
                        {
                            rank = (CurrentMove.GetColor() == PlayerColor.White) ? 0 : 7;
                        }
                        kingStartFile = prevRank;
                        kingEndFile = prevFile;
                        rookStartFile = newRank;
                        rookEndFile = newFile;
                        pieceAtPrev = Board.GetBlock(rank, prevRank).GetPiece();
                    }

                    Block rookStartBlock = Board.GetBlock(rank, rookStartFile);
                    Block rookEndBlock = Board.GetBlock(rank, rookEndFile);
                    Block kingStartBlock = Board.GetBlock(rank, kingStartFile);
                    Block kingEndBlock = Board.GetBlock(rank, kingEndFile);

                    Add960CastleMove(kingStartBlock, kingEndBlock, rookStartBlock, rookEndBlock, castlingType);

                    Piece rook = Board.GetBlock(rank, rookStartFile).GetPiece();
                    Piece king = Board.GetBlock(rank, kingStartFile).GetPiece();
                    Board.GetBlock(rank, rookStartFile).SetPiece(null);
                    Board.GetBlock(rank, kingStartFile).SetPiece(null);
                    Board.GetBlock(rank, rookEndFile).SetPiece(rook);
                    Board.GetBlock(rank, kingEndFile).SetPiece(king);
                }
                else if (moveType == MoveType.PromotionCheck)
                {
                    Piece pieceAtNew = newBlock.GetPiece();

                    if (pieceAtNew?.GetPieceType() != PieceType.King)
                        pieceAtNew?.Kill();

                    AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    if (pieceType == PieceType.Queen)
                        pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                    else if (pieceType == PieceType.Bishop)
                        pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                    else if (pieceType == PieceType.Rook)
                        pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                    else if (pieceType == PieceType.Knight)
                        pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);

                    if (CurrentMove == PlayerOne && pieceAtNew != null)
                    {
                        PlayerTwo.KillPiece(pieceAtNew);
                        string deadPiece = PlayerTwo.GetFirstDeadPiece();  // Get the dead piece as a string
                        PlayerTwoDeadPiecesChanged?.Invoke(deadPiece, true);
                    }
                    else if (CurrentMove == PlayerTwo && pieceAtNew != null)
                    {
                        PlayerOne.KillPiece(pieceAtNew);
                        PlayerOneDeadPiecesChanged?.Invoke(PlayerOne.GetFirstDeadPiece(), true);
                    }

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else if (newBlock.GetPiece() == null && moveType != MoveType.EnPassant) // if the target block is empty
                {
                    if(moveType != MoveType.Promotion)AddMove(prevBlock, newBlock, moveType, pieceAtPrev);

                    if (moveType == MoveType.Promotion
                        && pieceAtPrev.GetPieceType() == PieceType.Pawn
                        && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White && pieceAtPrev.GetColor() == PieceColor.White
                        || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.White && pieceAtPrev.GetColor() == PieceColor.Black
                        || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black && pieceAtPrev.GetColor() == PieceColor.White
                        || prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.Black && pieceAtPrev.GetColor() == PieceColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        if (pieceType == PieceType.Queen)
                        {
                            pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                        }
                        else if (pieceType == PieceType.Bishop)
                        {
                            pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                        }
                        else if (pieceType == PieceType.Rook)
                        {
                            pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                        }
                        else if (pieceType == PieceType.Knight)
                        {
                            pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);
                        }
                    }

                    if(moveType == MoveType.Promotion) AddMove(prevBlock, newBlock, moveType, pieceAtPrev, null, pieceType);

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                else // if the target block is not empty (kill piece)
                {
                    Piece pieceAtNew = newBlock.GetPiece();

                    if (pieceAtNew?.GetPieceType() != PieceType.King)
                        pieceAtNew.Kill();

                    if (moveType != MoveType.Promotion)
                        AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew);


                    if (moveType == MoveType.Promotion && prevBlock?.GetPiece().GetPieceType() == PieceType.Pawn
                        && ((prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White
                        || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.White
                        || prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.Black
                        || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)))
                    {
                        Pawn pawn = (Pawn)prevBlock.GetPiece();
                        if (pieceType == PieceType.Queen)
                        {
                            pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                        }
                        else if (pieceType == PieceType.Bishop)
                        {
                            pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                        }
                        else if (pieceType == PieceType.Rook)
                        {
                            pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                        }
                        else if (pieceType == PieceType.Knight)
                        {
                            pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);
                        }
                    };

                    if (CurrentMove == PlayerOne)
                    {
                        PlayerTwo.KillPiece(pieceAtNew);
                        PlayerTwoDeadPiecesChanged?.Invoke(PlayerTwo.GetFirstDeadPiece(), true);
                    }
                    else
                    {
                        PlayerOne.KillPiece(pieceAtNew);
                        PlayerOneDeadPiecesChanged?.Invoke(PlayerOne.GetFirstDeadPiece(), true);
                    }

                    if(moveType == MoveType.Promotion)
                        AddMove(prevBlock, newBlock, moveType, pieceAtPrev, pieceAtNew, pieceType);

                    Board.GetBlock(newRank, newFile).SetPiece(pieceAtPrev);
                    Board.GetBlock(prevRank, prevFile).SetPiece(null);
                }
                if (CurrentMove.GetColor() == PlayerColor.White)
                {
                    if ((moveType == MoveType.Check || moveType == MoveType.PromotionCheck) && Board.GetFinalStatus(PieceColor.Black)) // Scan for Checkmate
                    {
                        moveType = MoveType.Checkmate;
                        Status = GameStatus.WHITE_WIN;
                        IsGameOver = true;
                    }
                    else if (Board.GetFinalStatus(PieceColor.Black) && !Board.IsKingInCheck(PieceColor.Black)) // Scan for Stalemate
                    {
                        moveType = MoveType.Stalemate;
                        Status = GameStatus.STALEMATE;
                        IsGameOver = true;
                    }
                }
                else
                {
                    if ((moveType == MoveType.Check || moveType == MoveType.PromotionCheck) && Board.GetFinalStatus(PieceColor.White)) // Scan for Checkmate
                    {
                        moveType = MoveType.Checkmate;
                        Status = GameStatus.BLACK_WIN;
                        IsGameOver = true;
                    }
                    else if (Board.GetFinalStatus(PieceColor.White) && !Board.IsKingInCheck(PieceColor.White)) // Scan for Stalemate
                    {
                        moveType = MoveType.Stalemate;
                        Status = GameStatus.STALEMATE;
                        IsGameOver = true;
                    }
                }

                if (CurrentMove.GetColor() == PlayerColor.Black)
                {
                    if (PlayerOne.GetColor() == PlayerColor.White)
                    {
                        if (moveType == MoveType.Checkmate)
                        {
                            SecondPlayerMove.SetMoveType(MoveType.Checkmate);
                            SecondPlayerMove.SetNotation(SecondPlayerMove.GetNotation().Replace("+", "#"));
                        }
                        Moves.Push(FirstPlayerMove, SecondPlayerMove);
                        OnMoveMade(Moves.Peek());
                    }
                    else
                    {
                        if (moveType == MoveType.Checkmate)
                        {
                            FirstPlayerMove.SetMoveType(MoveType.Checkmate);
                            FirstPlayerMove.SetNotation(FirstPlayerMove.GetNotation().Replace("+", "#"));
                        }
                        Moves.Push(SecondPlayerMove, FirstPlayerMove);
                        OnMoveMade(Moves.Peek());
                    }
                }

                if (CurrentMove.GetColor() == PlayerColor.White && PlayerOne.GetColor() == PlayerColor.White && moveType == MoveType.Checkmate)
                {
                    FirstPlayerMove.SetMoveType(MoveType.Checkmate);
                    FirstPlayerMove.SetNotation(FirstPlayerMove.GetNotation().Replace("+", "#"));
                    Moves.Push(FirstPlayerMove);
                    MoveMade.Invoke(Moves.Peek(), true);
                }

                if (CurrentMove == PlayerTwo)
                {
                    CurrentMove = PlayerOne;
                    MovesStack.Push(SecondPlayerMove);
                }
                else
                {
                    CurrentMove = PlayerTwo;
                    MovesStack.Push(FirstPlayerMove);
                }
                //DisplayMoves();
                //MovesStack.Display();
            }
        }

        public void CheckForPromotionCheck(Block prevBlock, Block newBlock, Piece pieceAtPrev, ref MoveType moveType, ref PieceType pieceType)
        {
            Piece pieceAtNew = newBlock.GetPiece();
            Pawn pawn = (Pawn)prevBlock.GetPiece();
            if (prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.White
                || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.White
                || prevBlock.GetRank() == 1 && PlayerOne.GetColor() == PlayerColor.Black
                || prevBlock.GetRank() == 6 && PlayerOne.GetColor() == PlayerColor.Black)
            {
                if (pieceType == PieceType.Queen)
                    pieceAtPrev = new Queen(pieceAtPrev.GetColor(), PieceType.Queen, true);
                else if (pieceType == PieceType.Bishop)
                    pieceAtPrev = new Bishop(pieceAtPrev.GetColor(), PieceType.Bishop, true);
                else if (pieceType == PieceType.Rook)
                    pieceAtPrev = new Rook(pieceAtPrev.GetColor(), PieceType.Rook, true);
                else if (pieceType == PieceType.Knight)
                    pieceAtPrev = new Knight(pieceAtPrev.GetColor(), PieceType.Knight, true);
                else
                    return;
            }

            Board.GetBlock(newBlock.GetRank(), newBlock.GetFile()).SetPiece(pieceAtPrev);
            Board.GetBlock(prevBlock.GetRank(), prevBlock.GetFile()).SetPiece(null);

            PieceColor opponentKingColor = CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.White : PieceColor.Black;
            King king = (King)Board.FindKing(opponentKingColor).GetPiece();
            if (Board.IsKingInCheck(opponentKingColor))
            {
                moveType = MoveType.PromotionCheck;
                king.SetCheck(true);
            }
            else
            {
                king.SetCheck(false);
            }

            // reset board
            Board.GetBlock(newBlock.GetRank(), newBlock.GetFile()).SetPiece(pieceAtNew);
            Board.GetBlock(prevBlock.GetRank(), prevBlock.GetFile()).SetPiece(pawn);
        }

        public void AddMove(Block prevBlock, Block newBlock, MoveType moveType, Piece prevBlockPiece, Piece capturedPiece = null, PieceType promotedPieceType = PieceType.Queen)
        {
            // first check if the previous move was of pawn and it moved 2 blocks and player didn't kill it by en passant
            SetPawnUnEnPassantable();
            Move move;
            if (moveType == MoveType.Normal || moveType == MoveType.Check)
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType);
            }
            else if (moveType == MoveType.Castling)
            {
                CastlingType castlingType = CastlingType.None;

                if (newBlock.GetFile() == 6 || newBlock.GetFile() == 1)
                    castlingType = CastlingType.KingSideCastle;
                else if (newBlock.GetFile() == 2 || newBlock.GetFile() == 5)
                    castlingType = CastlingType.QueenSideCastle;

                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, castlingType);
            }
            else if (moveType == MoveType.EnPassant)
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType);
            }
            else
            {
                move = new Move(prevBlock, newBlock, prevBlockPiece, capturedPiece, moveType, promotedPieceType);
            }

            if (CurrentMove == PlayerOne)
                FirstPlayerMove = move;
            else
                SecondPlayerMove = move;
        }

        public void Add960CastleMove(Block kingPrevBlock, Block kingTargetBlock, Block rookPrevBlock, Block rookEndBlock, CastlingType castlingType)
        {
            Move move = new Move(kingPrevBlock, kingTargetBlock, rookPrevBlock, rookEndBlock, castlingType);
            if (CurrentMove == PlayerOne)
                FirstPlayerMove = move;
            else
                SecondPlayerMove = move;
        }

        public void UndoMove(Move move)
        {
            Block prevBlock = move.GetStartBlock();
            Block newBlock = move.GetEndBlock();
            Piece prevBlockPiece = move.GetPieceMoved();
            Piece capturedPiece = move.GetPieceKilled();
            MoveType moveType = move.GetMoveType();

            if (moveType == MoveType.Normal || moveType == MoveType.Kill)
            {
                if (prevBlockPiece.GetPieceType() == PieceType.Rook || prevBlockPiece.GetPieceType() == PieceType.King || prevBlockPiece.GetPieceType() == PieceType.Pawn)
                {
                    if (prevBlockPiece.GetPieceType() == PieceType.Rook)
                    {
                        Rook rook = (Rook)prevBlockPiece;
                        rook.UndoMove();
                    }
                    else if (prevBlockPiece.GetPieceType() == PieceType.Pawn
                        && ((prevBlockPiece.GetColor() == PieceColor.White && PlayerOne.GetColor() == PlayerColor.White && prevBlock.GetRank() == 6)
                        || (prevBlockPiece.GetColor() == PieceColor.Black && PlayerOne.GetColor() == PlayerColor.White && prevBlock.GetRank() == 1)
                        || (prevBlockPiece.GetColor() == PieceColor.Black && PlayerOne.GetColor() == PlayerColor.Black && prevBlock.GetRank() == 6)
                        || (prevBlockPiece.GetColor() == PieceColor.White && PlayerOne.GetColor() == PlayerColor.Black && prevBlock.GetRank() == 1)))
                    {
                        Pawn pawn = (Pawn)prevBlockPiece;
                        pawn.ResetPawn();
                    }
                    else if (prevBlockPiece.GetPieceType() == PieceType.King)
                    {
                        King king = (King)prevBlockPiece;
                        king.UndoMove();
                    }
                }
            }

            if (moveType == MoveType.Normal)
            {
                if (capturedPiece == null)
                    newBlock.SetPiece(null);
                else
                {
                    newBlock.SetPiece(capturedPiece);
                    capturedPiece.Revive();
                }

                prevBlock.SetPiece(prevBlockPiece);
            }
            else if (moveType == MoveType.Kill)
            {
                newBlock.SetPiece(capturedPiece);
                prevBlock.SetPiece(prevBlockPiece);
                capturedPiece.Revive();
            }
            else if (moveType == MoveType.Check || moveType == MoveType.Checkmate || moveType == MoveType.Stalemate)
            {
                if (capturedPiece == null)
                    newBlock.SetPiece(null);
                else
                {
                    newBlock.SetPiece(capturedPiece);
                    capturedPiece.Revive();
                }
                prevBlock.SetPiece(prevBlockPiece);
                King king = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.White : PieceColor.Black).GetPiece();
                king.SetCheck(false);
                king.UndoMove();
            }
            else if (moveType == MoveType.Promotion || moveType == MoveType.PromotionCheck)
            {
                if (capturedPiece == null)
                    newBlock.SetPiece(null);
                else
                {
                    newBlock.SetPiece(capturedPiece);
                    capturedPiece.Revive();
                }

                if (moveType == MoveType.PromotionCheck)
                {
                    King opponentKing = (King)Board.FindKing(CurrentMove.GetColor() == PlayerColor.Black ? PieceColor.Black : PieceColor.White).GetPiece();
                    opponentKing.SetCheck(false);
                }

                Piece piece = prevBlockPiece;
                prevBlockPiece.SetPieceType(PieceType.Pawn);
                prevBlock.SetPiece((Pawn)prevBlockPiece);
            }
            else if (moveType == MoveType.EnPassant)
            {
                newBlock.SetPiece(null);
                prevBlock.SetPiece(prevBlockPiece);
                capturedPiece.Revive();
                Pawn pawn = (Pawn)capturedPiece;
                pawn.UndoMove();
                Board.GetBlock(prevBlock.GetRank(), newBlock.GetFile()).SetPiece(capturedPiece);
            }
            else if (moveType == MoveType.Castling && !GetBoard().is960)
            {
                int rookTargetFile = -1;
                int rookCurrentFile = -1;
                CastlingType castlingType = move.GetCastlingType();
                bool FirstPlayerSelectedColorWhite = PlayerOne.GetColor() == PlayerColor.White;

                if (castlingType == CastlingType.KingSideCastle && FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 5;
                    rookTargetFile = 7;
                }
                else if (castlingType == CastlingType.KingSideCastle && !FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 2;
                    rookTargetFile = 0;
                }
                else if (castlingType == CastlingType.QueenSideCastle && FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 3;
                    rookTargetFile = 0;
                }
                else if (castlingType == CastlingType.QueenSideCastle && !FirstPlayerSelectedColorWhite)
                {
                    rookCurrentFile = 4;
                    rookTargetFile = 7;
                }

                Block rookCurrentBlock = Board.GetBlock(newBlock.GetRank(), rookCurrentFile);
                Block kingBlock = Board.GetBlock(newBlock.GetRank(), newBlock.GetFile());
                Block rookTargetBlock = Board.GetBlock(newBlock.GetRank(), rookTargetFile);
                King king = (King)kingBlock.GetPiece();
                Rook rook = (Rook)rookCurrentBlock.GetPiece();

                rook.UndoMove();
                king.UndoMove();

                rookTargetBlock.SetPiece(rookCurrentBlock.GetPiece());
                rookCurrentBlock.SetPiece(null);
                kingBlock.SetPiece(null);
                prevBlock.SetPiece(prevBlockPiece);
            }
            else if(moveType == MoveType.Castling && GetBoard().is960)
            {
                Block kingPrevBlock = move.GetStartBlock();
                Block kingEndBlock = move.GetEndBlock();
                Block rookPrevBlock = move.RookStartBlock;
                Block rookEndBlock = move.RookEndBlock;

                King king = (King)kingEndBlock.GetPiece();
                Rook rook = (Rook)rookEndBlock.GetPiece();

                king.UndoMove();
                rook.UndoMove();

                kingEndBlock.SetPiece(null);
                rookEndBlock.SetPiece(null);
                kingPrevBlock.SetPiece(king);
                rookPrevBlock.SetPiece(rook);
            }

            if (SecondPlayerMove != null)
            {
                MoveMade?.Invoke(move.GetNotation(), false);
            }
            if (CurrentMove == PlayerOne)
            {
                CurrentMove = PlayerTwo;
                if (capturedPiece != null)
                {
                    Piece deadPiece = PlayerOne.GetLatestDeadPiece();
                    if (deadPiece != null)
                    {
                        string piece = deadPiece.GetColor().ToString().ToLower() + "_" + deadPiece.GetPieceType().ToString().ToLower();
                        PlayerOneDeadPiecesChanged?.Invoke(piece, false);
                    }
                }
            }
            else
            {
                CurrentMove = PlayerOne;

                if (capturedPiece != null)
                {
                    Piece deadPiece = PlayerTwo.GetLatestDeadPiece();
                    if (deadPiece != null)
                    {
                        string piece = deadPiece.GetColor().ToString().ToLower() + "_" + deadPiece.GetPieceType().ToString().ToLower();
                        PlayerTwoDeadPiecesChanged?.Invoke(piece, false);
                    }
                }
            }

            MovesStack.Pop();

            if ((CurrentMove == PlayerTwo && PlayerOne.GetColor() == PlayerColor.White)
               || CurrentMove == PlayerOne && PlayerTwo.GetColor() == PlayerColor.White)
            {
                Moves.Pop();
            }
            else
            {
                if (PlayerOne.GetColor() == PlayerColor.White)
                    FirstPlayerMove.SetNotation(Moves.PeekWhite());
                if (PlayerTwo.GetColor() == PlayerColor.White)
                    SecondPlayerMove.SetNotation(Moves.PeekWhite());
            }
        }

        // Control Functions
        private void SetPawnUnEnPassantable()
        {
            if (Moves.GetSize() == 0) return;
            string lastMove = Moves.Peek();
            string[] tokens = lastMove.Split(' ');
            string prevNotation;
            int tokenForPlayer = -1;
            if (tokens.Length == 3)
            {
                if (PlayerOne.GetColor() == PlayerColor.White)
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.Black ? 2 : 1;
                else
                    tokenForPlayer = CurrentMove.GetColor() == PlayerColor.White ? 1 : 2;

                prevNotation = tokens[tokenForPlayer];
                if (prevNotation.Length != 2) return;
                int prevFile = Board.GetFileInInt(prevNotation[0].ToString());
                int prevRank = Board.TranslateRank(int.Parse(prevNotation[1].ToString()));
                if (PlayerOne.GetColor() == PlayerColor.Black)
                {
                    prevRank = int.Parse(prevNotation[1].ToString()) - 1;
                    prevFile = ReverseBlockValue(prevFile);
                }
                if (!Board.WithinBounds(prevRank, prevFile)) return;
                Block block = Board.GetBlock(prevRank, prevFile);
                if (!block.IsEmpty() && block.GetPiece().GetPieceType() == PieceType.Pawn)
                {
                    Pawn piece = (Pawn)block.GetPiece();
                    if (piece.GetEnPassantable())
                    {
                        piece.SetUnEnPassantable();
                    }
                }
            }
        }

        // Display Functions
        public void DisplayMoves()
        {
            Moves.Display();
        }

        // Getters 
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

        public MovesStack GetMovesStack()
        {
            return MovesStack;
        }

        public bool GetIsGameOver()
        {
            return IsGameOver;
        }

        public GameStatus GetStatus()
        {
            return Status;
        }

        public bool IsTurn(string pieceColor)
        {
            return CurrentMove.GetColor().ToString().Trim() == pieceColor.Trim();
        }

        public Player GetCurrentPlayer()
        {
            return CurrentMove;
        }

        // Setters
        public void SetIsGameOver(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }

        public void UpdateStatus(GameStatus status)
        {
            Status = status;
        }

        // Utility Functions
        public int ReverseBlockValue(int value)
        {
            switch (value)
            {
                case 0:
                    return 7;
                case 1:
                    return 6;
                case 2:
                    return 5;
                case 3:
                    return 4;
                case 4:
                    return 3;
                case 5:
                    return 2;
                case 6:
                    return 1;
                case 7:
                    return 0;
                default:
                    return -1;
            }
        }

        private void OnMoveMade(string move)
        {
            MoveMade?.Invoke(move, true);
        }

        // Check Status Functions
        public bool CheckThreeFoldRepeitetion()
        {
            if (Moves.GetSize() < 3 || MovesStack.GetSize() < 6)
                return false;

            // Checking for three fold repeitetion
            Move lastMove = MovesStack.Pop();
            Move secondLastMove = MovesStack.Pop();
            Move thirdLastMove = MovesStack.Pop();
            Move fourthLastMove = MovesStack.Pop();
            Move fifthLastMove = MovesStack.Pop();
            Move sixthLastMove = MovesStack.Pop();

            MovesStack.Push(sixthLastMove);
            MovesStack.Push(fifthLastMove);
            MovesStack.Push(fourthLastMove);
            MovesStack.Push(thirdLastMove);
            MovesStack.Push(secondLastMove);
            MovesStack.Push(lastMove);

            if (lastMove.GetNotation() == fifthLastMove.GetNotation()
            && thirdLastMove.GetEndBlock().GetRank() == fifthLastMove.GetStartBlock().GetRank()
            && thirdLastMove.GetEndBlock().GetFile() == fifthLastMove.GetStartBlock().GetFile()
            && lastMove.GetPieceMoved() == thirdLastMove.GetPieceMoved()
            && thirdLastMove.GetPieceMoved() == fifthLastMove.GetPieceMoved()
            && secondLastMove.GetNotation() == sixthLastMove.GetNotation()
            && fourthLastMove.GetEndBlock().GetRank() == sixthLastMove.GetStartBlock().GetRank()
            && fourthLastMove.GetEndBlock().GetFile() == sixthLastMove.GetStartBlock().GetFile()
            && secondLastMove.GetPieceMoved() == fourthLastMove.GetPieceMoved()
            && fourthLastMove.GetPieceMoved() == sixthLastMove.GetPieceMoved())
            {
                return true;
            }
            return false;
        }

        public bool CheckFiftyMoveRule()
        {
            // Checking for 50 move rule
            if (Moves.GetSize() < 50) return false;
            List<Move> moves = MovesStack.GetMoves();
            for (int i = 0; i < 100; i++)
            {
                if (moves[i].GetPieceMoved().GetPieceType() == PieceType.Pawn)
                    return false;
            }
            return true;
        }

        public bool CheckDraw()
        {
            return CheckThreeFoldRepeitetion() || CheckFiftyMoveRule();
        }
    }
}
