using Chess.GL;
using System;
using System.Collections.Generic;

namespace Chess.DS
{
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

        public void Display()
        {
            if (MovesStack.Count == 0)
            {
                Console.WriteLine("Stack is empty.");
                return;
            }
            foreach (string move in MovesStack)
            {
                Console.WriteLine(move);
            }
            Console.WriteLine();
        }

        public bool IsEmpty()
        {
            return MovesStack.Count == 0;
        }

        public int GetSize()
        {
            return MovesStack.Count;
        }

        public bool Contains(string move)
        {
            return MovesStack.Contains(move);
        }
    }
}
