using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.GL
{
    public class UtilityFunctions
    {
        public static PieceType GetPieceTypeByString(string pieceName)
        {
            switch (pieceName)
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
