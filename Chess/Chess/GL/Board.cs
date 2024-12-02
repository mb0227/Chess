using System;

namespace Chess.GL
{
    public class Board
    {
        private Block[,] Blocks;

        public Board()
        {
            Blocks = new Block[8, 8];
            // Creating White Pieces
            Blocks[0, 0] = new Block(0, 0, new Rook(PieceColor.White, PieceType.Rook, true));
            Blocks[0, 1] = new Block(0, 1, new Knight(PieceColor.White, PieceType.Knight, true));
            Blocks[0, 2] = new Block(0, 2, new Bishop(PieceColor.White, PieceType.Bishop, true));
            Blocks[0, 3] = new Block(0, 3, new Queen(PieceColor.White, PieceType.Queen, true));
            Blocks[0, 4] = new Block(0, 4, new King(PieceColor.White, PieceType.King, true));
            Blocks[0, 5] = new Block(0, 5, new Bishop(PieceColor.White, PieceType.Bishop, true));
            Blocks[0, 6] = new Block(0, 6, new Knight(PieceColor.White, PieceType.Knight, true));
            Blocks[0, 7] = new Block(0, 7, new Rook(PieceColor.White, PieceType.Rook, true));
            for (int file = 0; file < 8; file++)
            {
                Blocks[1, file] = new Block(1, file, new Pawn(PieceColor.White, PieceType.Pawn, true));
            }

            // Creating Black Pieces
            Blocks[7, 0] = new Block(7, 0, new Rook(PieceColor.Black, PieceType.Rook, true));
            Blocks[7, 1] = new Block(7, 1, new Knight(PieceColor.Black, PieceType.Knight, true));
            Blocks[7, 2] = new Block(7, 2, new Bishop(PieceColor.Black, PieceType.Bishop, true));
            Blocks[7, 3] = new Block(7, 3, new Queen(PieceColor.Black, PieceType.Queen, true));
            Blocks[7, 4] = new Block(7, 4, new King(PieceColor.Black, PieceType.King, true));
            Blocks[7, 5] = new Block(7, 5, new Bishop(PieceColor.Black, PieceType.Bishop, true));
            Blocks[7, 6] = new Block(7, 6, new Knight(PieceColor.Black, PieceType.Knight, true));
            Blocks[7, 7] = new Block(7, 7, new Rook(PieceColor.Black, PieceType.Rook, true));
            for (int file = 0; file < 8; file++)
            {
                Blocks[6, file] = new Block(6, file, new Pawn(PieceColor.Black, PieceType.Pawn, true));
            }

            // Creating Empty Blocks
            for (int rank = 2; rank < 6; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Blocks[rank, file] = new Block(rank, file);
                }
            }

        }

        public Block GetBlock(int rank, int file)
        {
            return Blocks[rank, file];
        }

        public Block GetBlock(Piece piece)
        {
            // search the board and return the block that contains the piece
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (Blocks[rank, file].GetPiece() == piece)
                    {
                        return Blocks[rank, file];
                    }
                }
            }
            return null;
        }

        public void DisplayBoard()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (Blocks[rank, file].IsEmpty())
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Piece piece = Blocks[rank, file].GetPiece();
                        if (piece.GetColor() == PieceColor.White)
                        {
                            Console.Write("W ");
                        }
                        else
                        {
                            Console.Write("B ");
                        }
                        Console.Write(piece.GetPieceType().ToString() + " ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
