using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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

        private void Chess960Click(object sender, RoutedEventArgs e)
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
                GamePage gamePage = new GamePage(playerColor, timeControl, -1, true); // true means 960
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


        private void AboutClick(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage();
            NavigationService.Navigate(aboutPage);
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
