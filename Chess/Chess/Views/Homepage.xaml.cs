using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess.Views
{
    public partial class Homepage : Page
    {
        public Homepage()
        {
            InitializeComponent();
        }

        private void MultiplayerClick(object sender, RoutedEventArgs e)
        {
            GamePage gamePage = new GamePage();
            NavigationService.Navigate(gamePage);
        }
    }
}
