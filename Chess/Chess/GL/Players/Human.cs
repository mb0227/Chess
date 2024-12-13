namespace Chess.GL
{
    public class Human : Player
    {
        public Human(PlayerColor color) : base(color)
        {
            PlayerType = PlayerType.Human;
        }
    }
}
