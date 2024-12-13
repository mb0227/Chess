using Chess.GL;
using System;
using System.Collections.Generic;

namespace Chess.DS
{
    public class MovesStack
    {
        public List<Move> Moves;

        public MovesStack()
        {
            Moves = new List<Move>();
        }

        public void Push(Move move)
        {
            Moves.Add(move);
        }

        public Move Pop()
        {
            if (!IsEmpty())
            {
                int top = Moves.Count - 1;
                Move move = Moves[top];
                Moves.RemoveAt(top);
                return move;
            }
            else
            {
                Console.WriteLine("Stack is empty, cannot pop.");
                return null;
            }
        }

        public Move Peek()
        {
            if (Moves.Count > 0)
            {
                return Moves[Moves.Count - 1];
            }
            else
            {
                Console.WriteLine("Stack is empty, cannot peek.");
                return null;
            }
        }

        public Move PeekLast(int count)
        {
            if (count > Moves.Count)
            {
                Console.WriteLine("Count is greater than the number of moves in the stack.");
                return null;
            }
            return Moves[Moves.Count - count];
        }

        public bool IsEmpty()
        {
            return Moves.Count == 0;
        }

        public int GetSize()
        {
            return Moves.Count;
        }

        public List<Move> GetMoves()
        {
            return Moves;
        }
        public void Display()
        {
            if (Moves.Count == 0)
            {
                Console.WriteLine("No moves.");
                return;
            }
            foreach (Move move in Moves)
            {
                Console.WriteLine(move.GetNotation());
            }
            Console.WriteLine();
        }
    }

    public class Stack
    {
        public List<string> MovesStack; 

        public Stack()
        {
            MovesStack = new List<string>();
        }

        public void Push(Move whiteMove, Move blackMove = null)
        { 
            int moveNumber = MovesStack.Count + 1;
            string formattedMove = blackMove == null
                ? $"{moveNumber}. {whiteMove.GetNotation()}" // Only White's move
                : $"{moveNumber}. {whiteMove.GetNotation()} {blackMove.GetNotation()}"; // White and Black's moves
            MovesStack.Add(formattedMove);
        }

        public string Pop()
        {
            if (!IsEmpty())
            {
                int top = MovesStack.Count - 1;
                string move = MovesStack[top];
                MovesStack.RemoveAt(top);
                return move;
            }
            else
            {
                Console.WriteLine("Stack is empty, cannot pop.");
                return null;
            }
        }

        public string Peek()
        {
            if (MovesStack.Count > 0)
            {
                return MovesStack[MovesStack.Count - 1];
            } 
            else
            {
                Console.WriteLine("Stack is empty, cannot peek.");
                return null;
            }
        }

        public bool IsEmpty()
        {
            return MovesStack.Count == 0;
        }

        public int GetSize()
        {
            return MovesStack.Count;
        }

        public void Display()
        {
            if (MovesStack.Count == 0)
            {
                Console.WriteLine("No moves.");
                return;
            }
            foreach (string move in MovesStack)
            {
                Console.WriteLine(move);
            }
            Console.WriteLine();
        }

    }
}

