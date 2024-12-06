namespace Chess.GL
{
    public class Block
    {
        private Piece Piece;
        private int Rank;
        private int File;

        public Block(int rank, int file)
        {
            Piece = null;
            this.Rank = rank;
            this.File = file;
        }

        public Block(int rank, int file, Piece piece)
        {
            Piece = piece;
            this.Rank = rank;
            this.File = file;
        }

        public override string ToString()
        {
            return "Rank " + Rank + " File " + File;

        }

        public Piece GetPiece()
        {
            return Piece;
        }

        public void SetPiece(Piece piece)
        {
            Piece = piece;
        }

        public int GetRank()
        {
            return Rank;
        }

        public int GetFile()
        {
            return File;
        }

        public bool IsEmpty()
        {
            return Piece == null;
        }

        public void SetRank(int rank)
        {
            Rank = rank;
        }
    }
}
