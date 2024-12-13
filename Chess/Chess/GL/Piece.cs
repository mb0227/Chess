using Chess.Interfaces;
using System;
using System.Collections.Generic;

namespace Chess.GL
{

    public class Piece : IMove
    {
        protected PieceColor Color;
        protected PieceType PieceType;
        protected bool Alive;
        
        public Piece(PieceColor color, PieceType type, bool alive)
        {
            Color = color;
            PieceType = type;
            Alive = alive;
        }

        public PieceColor GetColor()
        {
            return Color;
        }

        public PieceType GetPieceType()
        {
            return PieceType;
        }

        public bool IsAlive()
        {
            return Alive;
        }

        public void SetPieceType(PieceType type)
        {
            PieceType = type;
        }

        public void Revive()
        {
            if (!Alive)
            {
                Alive = true;
            }
        }

        public void Kill()
        {
            if (Alive)
            {
                Alive = false;
            }
        }

        public override string ToString()
        {
            return $"Color: {Color.ToString()} PieceType: {PieceType.ToString()} Alive: {Alive}";
        }

        public virtual bool IsAttackingKing(Board board, Block kingBlock)
        {
            return false;
        }

        public virtual List<Move> GetPossibleMoves(Board board)
        {
            return null;
        }

        public virtual bool CanAttack(Block targetBlock, Board board)
        {
            return false;
        }
    }
}
