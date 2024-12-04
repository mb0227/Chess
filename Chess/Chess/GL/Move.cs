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

        // bool members
        private bool IsCastling;
        private bool IsEnPassant;
        private bool IsPromotion;
        private bool IsCheck;
        private bool IsCheckMate;

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled)
        {
            StartBlock = startBlock;
            EndBlock = endBlock;
            PieceMoved = pieceMoved;
            PieceKilled = pieceKilled;
            MakeNotation();
            IsCastling = false;
            IsEnPassant = false;
            IsPromotion = false;
            IsCheck = false;
            IsCheckMate = false;
        }

        public override string ToString()
        {
           string details = "Start: " + StartBlock.ToString() + " End: " + EndBlock.ToString() + " Piece Moved Details: " + PieceMoved.ToString();
           if (PieceKilled != null) details += "Piece Killed: " + PieceKilled.ToString();
           details += "\n Notation: " + Notation; 
           return details;
        }

        public void MakeNotation()
        {
            Notation = GetPieceMovedString() + GetFileString(StartBlock.GetFile());
            if (PieceKilled != null) Notation += "x";
            Notation += Board.TranslateRank(EndBlock.GetRank());
            // Additional rules like castling, promotion, or checkmate
        }

        private string GetPieceMovedString()
        {
            if (PieceMoved.GetPieceType() == PieceType.King) return "K";
            else if (PieceMoved.GetPieceType() == PieceType.Queen) return "Q";
            else if (PieceMoved.GetPieceType() == PieceType.Knight) return "N";
            else if (PieceMoved.GetPieceType() == PieceType.Rook) return "R";
            else if (PieceMoved.GetPieceType() == PieceType.Bishop) return "B";
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

        public bool GetIsCastling()
        {
            return IsCastling;
        }

        public bool GetIsEnPassant()
        {
            return IsEnPassant;
        }

        public bool GetIsPromotion()
        {
            return IsPromotion;
        }

        public bool GetIsCheck()
        {
            return IsCheck;
        }

        public bool GetIsCheckMate()
        {
            return IsCheckMate;
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
        public void SetIsCastling(bool isCastling)
        {
            IsCastling = isCastling;
        }

        public void SetIsEnPassant(bool isEnPassant)
        {
            IsEnPassant = isEnPassant;
        }

        public void SetIsPromotion(bool isPromotion)
        {
            IsPromotion = isPromotion;
        }

        public void SetIsCheck(bool isCheck)
        {
            IsCheck = isCheck;
        }

        public void SetIsCheckMate(bool isCheckMate)
        {
            IsCheckMate = isCheckMate;
        }

        public static void SetBoard(Board board)
        {
            Board = board;
        }
    }
}
