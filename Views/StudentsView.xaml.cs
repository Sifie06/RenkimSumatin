using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace renkimsumatin.Views
{
    public partial class StudentsView : UserControl
    {
        public StudentsView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Name == "NameTextBox" && textBox.Text == "Student Name")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
                else if (textBox.Name == "StudentIdTextBox" && textBox.Text == "Student ID")
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
                    textBox.Text = "Student Name";
                    textBox.Foreground = Brushes.Gray;
                }
                else if (textBox.Name == "StudentIdTextBox" && string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "Student ID";
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
    }
}