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

            SelectOptions options = new SelectOptions();
            if (options.ShowDialog() == true)
            {
                string selectedColor = options.SelectedColor;
                string selectedTimeControl = options.SelectedTimeControl;
                string selectedDifficulty = options.SelectedDifficulty;

                MessageBox.Show($"Color: {selectedColor}, Time: {selectedTimeControl}, Difficulty: {selectedDifficulty}", "Selection Summary");
            }
        }
    }
}
