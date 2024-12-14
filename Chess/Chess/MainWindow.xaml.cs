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
        }
    }
}
