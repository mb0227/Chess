using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess.Views
{
    public partial class SelectOptions : Window
    {
        public string SelectedColor { get; private set; }
        public string SelectedTimeControl { get; private set; }
        public string SelectedDifficulty { get; private set; }

        public SelectOptions()
        {
            InitializeComponent();
            LoadImages();
            SelectedDifficulty = "Medium"; // Default
        }

        private void LoadImages()
        {
            try
            {
                string whiteKingImagePath = Path.Combine("..\\..\\Images", "white-king.png");
                whiteKingImagePath = Path.GetFullPath(whiteKingImagePath);
                string blackKingImagePath = Path.Combine("..\\..\\Images", "black-king.png");
                blackKingImagePath = Path.GetFullPath(blackKingImagePath);

                WhiteKingImage.Source = new BitmapImage(new Uri(whiteKingImagePath, UriKind.Absolute));
                BlackKingImage.Source = new BitmapImage(new Uri(blackKingImagePath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = (FindName("White") as RadioButton)?.IsChecked == true ? "White" : "Black";
            SelectedTimeControl = GetCheckedRadioButton("TimeControl");
            SelectedDifficulty = (DifficultyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public void HideDifficultySection()
        {
            DifficultyPanel.Visibility = Visibility.Collapsed;
        }

        private string GetCheckedRadioButton(string groupName)
        {
            RadioButton checkedRadioButton = FindCheckedRadioButton(this, groupName);
            return checkedRadioButton?.Content.ToString();
        }

        private RadioButton FindCheckedRadioButton(DependencyObject parent, string groupName)
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is RadioButton rb && rb.GroupName == groupName && rb.IsChecked == true)
                {
                    return rb;
                }

                var result = FindCheckedRadioButton(child, groupName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
