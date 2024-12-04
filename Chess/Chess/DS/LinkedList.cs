using System.Collections.Generic;
using System;
using Chess.GL;

namespace Chess.DS
{
    public class Node
    {
        public Piece Piece;
        public Node NextNode;

        public Node(Piece piece)
        {
            Piece = piece;
            NextNode = null;
        }
    }

    public class LinkedList
    {
        public Node Head;

        public LinkedList()
        {
            Head = null;
        }

        public void InsertAtHead(Piece piece)
        {
            Node newNode = new Node(piece);
            newNode.NextNode = Head;
            Head = newNode;
        }

        public void InsertAtTail(Piece piece)
        {
            if (Head == null)
            {
                InsertAtHead(piece);
                return;
            }

            Node temp = Head;
            while (temp.NextNode != null)
            {
                temp = temp.NextNode;
            }

            Node newNode = new Node(piece);
            temp.NextNode = newNode;
        }

        public void Display()
        {
            Node temp = Head;
            while (temp != null)
            {
                Console.Write($"{temp.Piece.ToString()} -> ");
                temp = temp.NextNode;
            }
            Console.WriteLine("NULL");
        }

        public Piece GetLastPiece()
        {
            Node temp = Head;
            if (temp == null)
                return null;
            while (temp.NextNode != null)
            {
                temp = temp.NextNode;
            }
            return temp.Piece;
        }

        public bool Contains(Piece piece)
        {
            Node temp = Head;
            while (temp != null)
            {
                if (temp.Piece.Equals(piece))
                {
                    return true;
                }
                temp = temp.NextNode;
            }
            return false;
        }

        public int Size()
        {
            Node temp = Head;
            int size = 0;
            while (temp != null)
            {
                size++;
                temp = temp.NextNode;
            }
            return size;
        }

        public void RemovePiece(Piece piece)
        {
            Node temp = Head;
            Node prev = null;
            while (temp != null)
            {
                if (temp.Piece.Equals(piece))
                {
                    if (prev == null)
                    {
                        Head = temp.NextNode;
                    }
                    else
                    {
                        prev.NextNode = temp.NextNode;
                    }
                    break;
                }
                prev = temp;
                temp = temp.NextNode;
            }
        }

        public void RemoveFirstPiece()
        {
            if (Head == null)
                return;
            Head = Head.NextNode;
        }

        public Piece RemoveLastPiece()
        {
            Piece lastPiece = null;

            if (Head == null)
                return null;

            if (Head.NextNode == null)
            {
                lastPiece = Head.Piece;
                Head = null;
                return lastPiece;
            }

            Node temp = Head;
            while (temp.NextNode.NextNode != null)
            {
                temp = temp.NextNode;
            }

            lastPiece = temp.NextNode.Piece;
            temp.NextNode = null;
            return lastPiece;
        }
    }
}
