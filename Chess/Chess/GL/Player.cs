namespace Chess.GL
{
    public enum PlayerColor
    {
        White,
        Black
    }

    public class Player
    {
        public PlayerColor Color;

        public Player(PlayerColor color)
        {
            Color = color;
        }
    }
}
