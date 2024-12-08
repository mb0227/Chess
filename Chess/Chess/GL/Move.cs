using System;

namespace Chess.GL
{
    public class Move
    {
        private static Board Board;
        private Player PlayerMoved;
        private Block StartBlock;
        private Block EndBlock;
        private Piece PieceMoved;
        private Piece PieceKilled;
        private string Notation;

        private MoveType MoveType;
        private PieceType PromotedPieceType;
        // bool members

        //private bool IsCastling;
        //private bool IsEnPassant;
        //private bool IsPromotion;
        //private bool IsCheck;
        //private bool IsCheckMate;

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled)
        {
            StartBlock = startBlock;
            EndBlock = endBlock;
            PieceMoved = pieceMoved;
            PieceKilled = pieceKilled;
            MoveType = MoveType.Normal;
            MakeNotation();
        }
        
        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, MoveType moveType) : this(startBlock, endBlock, pieceMoved, pieceKilled)
        {
            MoveType = moveType;
        }
        
        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, MoveType moveType, PieceType promotedPieceType) : this(startBlock, endBlock, pieceMoved, pieceKilled, moveType)
        {
            PromotedPieceType = promotedPieceType;
            Notation = "";
            MakeNotation();
        }

        public override string ToString()
        {
           string details = "Start: " + StartBlock.ToString() + " End: " + EndBlock.ToString() + " Piece Moved Details: " + PieceMoved.ToString();
           if (PieceKilled != null) details += " Piece Killed: " + PieceKilled.ToString();
           details += "\nNotation: " + Notation; 
           return details;
        }

        public void MakeNotation()
        {
            if (PieceMoved.GetPieceType() != PieceType.Pawn)
            {
                if (PieceKilled == null)
                    Notation = GetPieceMovedString(PieceMoved.GetPieceType()) + GetFileString(EndBlock.GetFile());
                else
                    Notation = GetPieceMovedString(PieceMoved.GetPieceType());
            }
            else
            {
                Notation = GetPieceMovedString(PieceMoved.GetPieceType()) + GetFileString(StartBlock.GetFile());
            }

            if (PieceKilled != null) Notation += "x" + GetFileString(EndBlock.GetFile());
            Notation += Board.TranslateRank(EndBlock.GetRank());
            if (MoveType == MoveType.Promotion)
            {
                Notation += "=" + GetPieceMovedString(PromotedPieceType);
            }
            // Additional rules like castling, promotion, or checkmate
        }

        private string GetPieceMovedString(PieceType p)
        {
            if (p == PieceType.King) return "K";
            else if (p == PieceType.Queen) return "Q";
            else if (p == PieceType.Knight) return "N";
            else if (p == PieceType.Rook) return "R";
            else if (p == PieceType.Bishop) return "B";
            return "";
        }

        private string GetFileString(int file)
        {
            return Board.TranslateFile(file).ToString();
        }

        public Player GetPlayerMoved()
        {
            return PlayerMoved;
        }

        public Block GetStartBlock()
        {
            return StartBlock;
        }

        public Block GetEndBlock()
        {
            return EndBlock;
        }

        public Piece GetPieceMoved()
        {
            return PieceMoved;
        }

        public Piece GetPieceKilled()
        {
            return PieceKilled;
        }

        public MoveType GetMoveType()
        {
            return MoveType;
        }

        public string GetNotation()
        {
            return Notation;
        }

        public void SetPlayerMoved(Player player)
        {
            PlayerMoved = player;
        }

        // setters for bool methods

        public bool GetIsPromotion()
        {
            return MoveType == MoveType.Promotion;
        }

        public void SetIsPromotion(bool isPromotion)
        {
            if(isPromotion) MoveType = MoveType.Promotion;
        }

        public static void SetBoard(Board board)
        {
            Board = board;
        }
    }
}
