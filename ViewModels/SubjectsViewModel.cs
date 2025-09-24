using renkimsumatin.Models;
using renkimsumatin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;

namespace renkimsumatin.ViewModels
{
    public class SubjectsViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private string _name = string.Empty;
        private string _code = string.Empty;

        public SubjectsViewModel(DataService dataService)
        {
            _dataService = dataService;
            AddSubjectCommand = new RelayCommand(AddSubject);
            DeleteSubjectCommand = new RelayCommand<Subject>(DeleteSubject);
            RefreshCommand = new RelayCommand(Refresh);

            Subjects = new ObservableCollection<Subject>(_dataService.GetSubjects());
        }

        public ObservableCollection<Subject> Subjects { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddSubjectCommand { get; }
        public RelayCommand<Subject> DeleteSubjectCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void Refresh()
        {
            var latest = _dataService.GetSubjects();
            Subjects.Clear();
            foreach (var s in latest)
                Subjects.Add(s);
            OnPropertyChanged(nameof(Subjects));
        }

        private void AddSubject()
        {
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Code))
            {
                var subject = new Subject { Name = Name, Code = Code };
                _dataService.AddSubject(subject);
                Subjects.Add(subject);
                Name = string.Empty;
                Code = string.Empty;
            }
        }

        private void DeleteSubject(Subject subject)
        {
            if (MessageBox.Show($"Are you sure you want to delete {subject.Name}? This will also delete all its assessments and scores.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _dataService.DeleteSubject(subject.Id);
                Subjects.Remove(subject);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}