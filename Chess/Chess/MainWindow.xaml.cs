using System;
using System.Windows;
using Chess.Views;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new GamePage(GL.PlayerColor.White, 5);
            //Main.Content = new Homepage();
        }
    }
}
