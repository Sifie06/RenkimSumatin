using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace renkimsumatin.Models
{
    public class Subject : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = string.Empty;
        private string _code = string.Empty;

        [Key]
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        [Required]
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        [Required]
        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(); }
        }

        public Subject() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}