using Chess.GL;
using System.Collections.Generic;

namespace Chess.Interfaces
{
    interface IMove
    {
        List<Move> GetPossibleMoves(Board board);
    }
}
