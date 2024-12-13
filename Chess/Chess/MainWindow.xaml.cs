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
            Main.Content = new Homepage();
        }
    }
}
