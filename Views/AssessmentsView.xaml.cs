using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace renkimsumatin.Views
{
    public partial class AssessmentsView : UserControl
    {
        public AssessmentsView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Name == "NameTextBox" && textBox.Text == "Assessment Name")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
                else if (textBox.Name == "TotalMarksTextBox" && textBox.Text == "Total Marks")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Name == "NameTextBox" && string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "Assessment Name";
                    textBox.Foreground = Brushes.Gray;
                }
                else if (textBox.Name == "TotalMarksTextBox" && string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "Total Marks";
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
    }
}