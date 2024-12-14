using Chess.GL;
using System;

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

        public Piece RemoveFirstPiece()
        {
            if (Head == null)
                return null;
            Node temp = Head;
            Head = Head.NextNode;
            return temp.Piece;
        }

        public string GetFirstPiece()
        {
            if (Head == null)
                return null;
            return Head.Piece.GetColor().ToString().ToLower() + "-" + Head.Piece.GetPieceType().ToString().ToLower();
        }

        public int GetSize()
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

        public void Display()
        {
            Node temp = Head;
            while (temp != null)
            {
                Console.Write($"{temp.Piece.GetColor().ToString()}'s Dead Pieces: {temp.Piece.GetPieceType()}, ");
                temp = temp.NextNode;
            }
            Console.WriteLine();
        }
    }
}
