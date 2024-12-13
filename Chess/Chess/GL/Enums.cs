using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.GL
{
    public enum GameStatus
    {
        ACTIVE,
        BLACK_WIN,
        WHITE_WIN,
        FORFEIT,
        STALEMATE,
        RESIGNATION,
        DRAW,
        THREEFOLD_REPETITION,
        FIFTY_MOVE_RULE
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
        Castling,
        PromotionCheck,
        Draw
    }

    public enum CastlingType
    {
        KingSideCastle,
        QueenSideCastle,
        None
    }

    public enum PlayerColor
    {
        White,
        Black
    }

    public enum PlayerType
    {
        Human,
        Computer
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public enum PieceColor
    {
        White,
        Black
    }

    public class Enums
    {
    }
}
