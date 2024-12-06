using System.Windows;
using System.Windows.Controls;

namespace Chess
{
    public partial class PromotionWindow : Window
    {
        public string SelectedPiece { get; private set; }

        public PromotionWindow()
        {
            InitializeComponent();
        }

        private void OnPromotionSelected(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SelectedPiece = button.Content.ToString(); // Get the piece name (Queen, Rook, etc.)
                this.DialogResult = true; // Close the dialog
            }
        }
    }
}
