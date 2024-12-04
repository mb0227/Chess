using Chess.GL;
using System;
using System.Collections.Generic;

namespace Chess.DS
{
    public class Stack
    {
        public List<Move> MovesStack;

        public Stack()
        {
            MovesStack = new List <Move>();
        }

        public void Push(Move move)
        {
           MovesStack.Add(move);
        }

        public Move Pop()
        {
            if (!IsEmpty())
            {
                int top = MovesStack.Count - 1;
                Move move =  MovesStack[top];
                MovesStack.RemoveAt(top);
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
            for (int i = 0; i < MovesStack.Count; i++)
            {
                Console.WriteLine(MovesStack[i].GetNotation());
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

        public bool Contains(Move move)
        {
            for (int i = 0; i < MovesStack.Count; i++)
            {
                if (MovesStack[i].Equals(move))
                {
                    return true;
                }
            }
            return false;
        }
    }
}