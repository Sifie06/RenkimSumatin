using renkimsumatin.Models;
using renkimsumatin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace renkimsumatin.ViewModels
{
    public class StudentsViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private Student? _editingStudent;
        private string _name = string.Empty;
        private string _studentId = string.Empty;
        private string? _originalEditingStudentId; // keep original to prevent changes

        public StudentsViewModel(DataService dataService)
        {
            _dataService = dataService;
            AddStudentCommand = new RelayCommand(AddStudent);
            EditStudentCommand = new RelayCommand<Student>(EditStudent);
            UpdateStudentCommand = new RelayCommand(UpdateStudent);
            DeleteStudentCommand = new RelayCommand<Student>(DeleteStudent);
            CancelEditCommand = new RelayCommand(CancelEdit);
            RefreshCommand = new RelayCommand(Refresh);

            // Initialize collection once and keep it bound
            Students = new ObservableCollection<Student>(_dataService.GetStudents());
        }

        public ObservableCollection<Student> Students { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged();
            }
        }

        public Student? EditingStudent
        {
            get => _editingStudent;
            set
            {
                _editingStudent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        public bool IsEditing => EditingStudent != null;

        public RelayCommand AddStudentCommand { get; }
        public RelayCommand UpdateStudentCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand<Student> EditStudentCommand { get; }
        public RelayCommand<Student> DeleteStudentCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void Refresh()
        {
            var latest = _dataService.GetStudents();
            Students.Clear();
            foreach (var s in latest)
                Students.Add(s);
            OnPropertyChanged(nameof(Students));
        }

        private void AddStudent()
        {
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(StudentId))
            {
                var student = new Student { Name = Name, StudentId = StudentId };
                _dataService.AddStudent(student);
                Students.Add(student);
                Name = string.Empty;
                StudentId = string.Empty;
            }
        }

        private void EditStudent(Student student)
        {
            // clone to avoid editing the live instance until Save
            EditingStudent = new Student { Id = student.Id, Name = student.Name, StudentId = student.StudentId };
            _originalEditingStudentId = student.StudentId;
        }

        private void UpdateStudent()
        {
            if (EditingStudent != null && !string.IsNullOrWhiteSpace(EditingStudent.Name) && !string.IsNullOrWhiteSpace(EditingStudent.StudentId))
            {
                // Prevent changing Student ID
                if (!string.Equals(EditingStudent.StudentId, _originalEditingStudentId))
                {
                    MessageBox.Show("Student ID cannot be edited. Please edit only the Student Name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _dataService.UpdateStudent(EditingStudent);

                    // Update local collection
                    var existing = Students.FirstOrDefault(s => s.Id == EditingStudent.Id);
                    if (existing != null)
                    {
                        existing.Name = EditingStudent.Name;
                        // existing.StudentId unchanged on purpose
                    }
                    else
                    {
                        // In case the item wasn't found, refresh the list
                        Refresh();
                    }

                    EditingStudent = null;
                    _originalEditingStudentId = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteStudent(Student student)
        {
            if (MessageBox.Show($"Are you sure you want to delete {student.Name}? This will also delete all their scores.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _dataService.DeleteStudent(student.Id);
                Students.Remove(student);
            }
        }

        private void CancelEdit()
        {
            EditingStudent = null;
            _originalEditingStudentId = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}