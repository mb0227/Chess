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
        string selectedColor, selectedTimeControl, selectedDifficulty;
        public Homepage()
        {
            InitializeComponent();
        }

        private void MultiplayerClick(object sender, RoutedEventArgs e)
        {
            SelectOptions options = new SelectOptions(true);
            if (options.ShowDialog() == true)
            {
                selectedColor = options.SelectedColor;
                selectedTimeControl = options.SelectedTimeControl;
            }
            if (selectedColor != null && selectedTimeControl != null)
            {
                GL.PlayerColor playerColor = selectedColor == "White" ? GL.PlayerColor.White : GL.PlayerColor.Black;
                int timeControl = int.Parse(selectedTimeControl.Replace("m", ""));
                GamePage gamePage = new GamePage(playerColor, timeControl);
                NavigationService.Navigate(gamePage);
            }
        }

        private void VsComputerClick(object sender, RoutedEventArgs e)
        {
            SelectOptions options = new SelectOptions(false);
            if (options.ShowDialog() == true)
            {
                selectedColor = options.SelectedColor;
                selectedTimeControl = options.SelectedTimeControl;
                selectedDifficulty = options.SelectedDifficulty;
            }
            if (selectedColor != null && selectedTimeControl != null && selectedDifficulty != null)
            {
                GL.PlayerColor playerColor = selectedColor == "White" ? GL.PlayerColor.White : GL.PlayerColor.Black;
                int timeControl = int.Parse(selectedTimeControl.Replace("m", ""));
                int difficulty = GetDifficulty();
                GamePage gamePage = new GamePage(playerColor, timeControl, difficulty);
                NavigationService.Navigate(gamePage);
            }
        }

        private int GetDifficulty()
        {
            switch (selectedDifficulty)
            {
                case "Easy":
                    return 1;
                case "Medium":
                    return 2;
                case "Hard":
                    return 3;
                default:
                    return 2;
            }
        }
    }
}
