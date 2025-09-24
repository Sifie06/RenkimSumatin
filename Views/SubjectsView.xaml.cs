using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace renkimsumatin.Views
{
    public partial class SubjectsView : UserControl
    {
        public SubjectsView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Name == "NameTextBox" && textBox.Text == "Subject Name")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
                else if (textBox.Name == "CodeTextBox" && textBox.Text == "Subject Code")
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
                    textBox.Text = "Subject Name";
                    textBox.Foreground = Brushes.Gray;
                }
                else if (textBox.Name == "CodeTextBox" && string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "Subject Code";
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
    }
}