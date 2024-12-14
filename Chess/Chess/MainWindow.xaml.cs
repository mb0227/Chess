using Chess.Views;
using System.Windows;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new Homepage();
            //Main.Content = new GamePage(GL.PlayerColor.White, 5, 3);
        }
    }
}
