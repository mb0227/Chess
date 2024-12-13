namespace Chess.GL
{
    public class Computer : Player
    {
        public Computer(PlayerColor color) : base(color)
        {
            PlayerType = PlayerType.Computer;
        }
    }
}
