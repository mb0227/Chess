namespace Chess.GL
{
    public class Move
    {
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

        public Move(Player player, Player playerTwo, Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, string notation)
        {
            PlayerMoved = player;
            StartBlock = startBlock;
            EndBlock = endBlock;
            PieceMoved = pieceMoved;
            PieceKilled = pieceKilled;
            Notation = notation;
            IsCastling = false;
            IsEnPassant = false;
            IsPromotion = false;
            IsCheck = false;
            IsCheckMate = false;
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
    }
}
