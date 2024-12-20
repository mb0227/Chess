using Chess.GL;
using System;
using System.IO;
using System.Resources;
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

        public SelectOptions(bool hideDifficultySection)
        {
            InitializeComponent();
            LoadImages();
            SelectedDifficulty = "Medium"; // Default
            if (hideDifficultySection)
                HideDifficultySection();
        }

        private void LoadImages()
        {
            try
            {
                var whiteKingImagePath = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("white_king");
                var blackKingImagePath = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("black_king");

                WhiteKingImage.Source = UtilityFunctions.BitmapToBitmapImage(whiteKingImagePath);
                BlackKingImage.Source = UtilityFunctions.BitmapToBitmapImage(blackKingImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = GetCheckedRadioButton("Color");
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
