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
        private CastlingType CastlingType;

        public Move(MoveType moveType)
        {
            MoveType = moveType;
        }

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled)
        {
            StartBlock = startBlock;
            EndBlock = endBlock;
            PieceMoved = pieceMoved;
            PieceKilled = pieceKilled;
            MoveType = MoveType.Normal;
            MakeNotation();
        }

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, CastlingType castlingType) : this(startBlock, endBlock, pieceMoved, pieceKilled)
        {
            MoveType = MoveType.Castling;
            CastlingType = castlingType;
            Notation = "";
            MakeNotation();
        }

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, MoveType moveType) : this(startBlock, endBlock, pieceMoved, pieceKilled)
        {
            MoveType = moveType;
            Notation = "";
            MakeNotation();
        }

        public Move(Block startBlock, Block endBlock, Piece pieceMoved, Piece pieceKilled, MoveType moveType, PieceType promotedPieceType) : this(startBlock, endBlock, pieceMoved, pieceKilled, moveType)
        {
            PromotedPieceType = promotedPieceType;
            Notation = "";
            MakeNotation();
        }

        // special constructor for castling in 960
        // I will pass castling type as none while getting moves from king.
        // usually I assign move type after making the move but in this case i need to assign it before making the move 
        // so I can access this particular move in the front-end
        public Block RookStartBlock;
        public Block RookEndBlock;
        public Move(Block kingStartBlock, Block kingEndBlock, Block rookStartBlock, Block rookEndBlock, CastlingType castlingType)
        {
            StartBlock = kingStartBlock;
            EndBlock = kingEndBlock;
            RookStartBlock = rookStartBlock;
            RookEndBlock = rookEndBlock;
            PieceMoved = Board.GetBlock(kingStartBlock.GetRank(), kingStartBlock.GetFile()).GetPiece();
            PieceKilled = null;
            MoveType = MoveType.Castling;
            CastlingType = castlingType;
            if(castlingType == CastlingType.KingSideCastle) Notation = "O-O";
            else Notation = "O-O-O";
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

        public bool GetIsPromotion()
        {
            return MoveType == MoveType.Promotion;
        }

        public void SetMoveType(MoveType moveType)
        {
            MoveType = moveType;
        }

        public void SetIsPromotion(bool isPromotion)
        {
            if (isPromotion) MoveType = MoveType.Promotion;
        }

        public static void SetBoard(Board board)
        {
            Board = board;
        }

        public void SetNotation(string notation)
        {
            Notation = notation;
        }

        public CastlingType GetCastlingType()
        {
            return CastlingType;
        }

        public void MakeNotation()
        {
            if (MoveType == MoveType.Draw)
                return;

            if (MoveType == MoveType.Castling && CastlingType == CastlingType.KingSideCastle)
            {
                Notation = "O-O";
                return;
            }
            if (MoveType == MoveType.Castling && CastlingType == CastlingType.QueenSideCastle)
            {
                Notation = "O-O-O";
                return;
            }

            if (PieceMoved.GetPieceType() != PieceType.Pawn)
            {
                Notation = GetPieceMovedString(PieceMoved.GetPieceType());
                // Now we check if any any piece can move to same square
                Piece otherPiece = Board.OtherPieceCanMove(PieceMoved, EndBlock);
                if (otherPiece != null)
                {
                    if (Board.GetBlock(otherPiece).GetFile() != StartBlock.GetFile()) // if files are different write just file
                    {
                        Notation += GetFileString(StartBlock.GetFile());
                    }
                    else if (Board.GetBlock(otherPiece).GetFile() == StartBlock.GetFile() // if files are same but ranks are different write just rank
                          && Board.GetBlock(otherPiece).GetRank() != StartBlock.GetRank())
                    {
                        Notation += Board.TranslateRank(StartBlock.GetRank());
                    }
                    else // if both are same write rank and file both
                    {
                        Notation += GetFileString(StartBlock.GetFile()) + Board.TranslateRank(StartBlock.GetRank());
                    }
                }
                if (PieceKilled == null)
                {
                    Notation += GetFileString(EndBlock.GetFile());
                }
            }
            else
            {
                Notation = GetPieceMovedString(PieceMoved.GetPieceType()) + GetFileString(StartBlock.GetFile());
            }

            if (PieceKilled != null)
                Notation += "x" + GetFileString(EndBlock.GetFile());

            Notation += Board.TranslateRank(EndBlock.GetRank());

            if (MoveType == MoveType.Promotion)
                Notation += "=" + GetPieceMovedString(PromotedPieceType);

            if (MoveType == MoveType.Checkmate)
                Notation += "#";
            else if (MoveType == MoveType.Check)
                Notation += "+";

            if (MoveType == MoveType.PromotionCheck)
                Notation += "=" + GetPieceMovedString(PromotedPieceType) + "+";

        }

        public override string ToString()
        {
            string details = "Start: " + StartBlock.ToString() + " End: " + EndBlock.ToString() + " Piece Moved Details: " + PieceMoved.ToString();
            if (PieceKilled != null) details += " Piece Killed: " + PieceKilled.ToString();
            details += " Notation: " + Notation;
            return details;
        }
    }
}
