using renkimsumatin.Models;
using renkimsumatin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace renkimsumatin.ViewModels
{
    public class AssessmentsViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private Assessment? _editingAssessment;
        private string _name = string.Empty;
        private int _totalMarks;
        private string _selectedSubjectId = string.Empty;

        public AssessmentsViewModel(DataService dataService)
        {
            _dataService = dataService;
            AddAssessmentCommand = new RelayCommand(AddAssessment);
            EditAssessmentCommand = new RelayCommand<AssessmentDisplayModel>(EditAssessment);
            UpdateAssessmentCommand = new RelayCommand(UpdateAssessment);
            DeleteAssessmentCommand = new RelayCommand<AssessmentDisplayModel>(DeleteAssessment);
            CancelEditCommand = new RelayCommand(CancelEdit);
            RefreshCommand = new RelayCommand(Refresh);

            Subjects = _dataService.GetSubjects();
            RefreshAssessments();
        }

        public ObservableCollection<Subject> Subjects { get; }
        public ObservableCollection<AssessmentDisplayModel> Assessments { get; private set; } = new();

        private void RefreshAssessments()
        {
            var subjects = Subjects.ToDictionary(s => s.Id, s => s.Name);
            Assessments.Clear();
            foreach (var a in _dataService.GetAssessments())
            {
                Assessments.Add(new AssessmentDisplayModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    TotalMarks = a.TotalMarks,
                    SubjectId = a.SubjectId,
                    SubjectName = subjects.TryGetValue(a.SubjectId, out var n) ? n : a.SubjectId
                });
            }
            OnPropertyChanged(nameof(Assessments));
        }

        private void Refresh()
        {
            // Reload subjects and assessments to ensure subject names are current too
            var freshSubjects = _dataService.GetSubjects();
            Subjects.Clear();
            foreach (var s in freshSubjects) Subjects.Add(s);
            OnPropertyChanged(nameof(Subjects));
            RefreshAssessments();
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public int TotalMarks
        {
            get => _totalMarks;
            set { _totalMarks = value; OnPropertyChanged(); }
        }

        public string SelectedSubjectId
        {
            get => _selectedSubjectId;
            set { _selectedSubjectId = value; OnPropertyChanged(); }
        }

        public Assessment? EditingAssessment
        {
            get => _editingAssessment;
            set { _editingAssessment = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEditing)); }
        }

        public bool IsEditing => EditingAssessment != null;

        public RelayCommand AddAssessmentCommand { get; }
        public RelayCommand UpdateAssessmentCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand<AssessmentDisplayModel> EditAssessmentCommand { get; }
        public RelayCommand<AssessmentDisplayModel> DeleteAssessmentCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void AddAssessment()
        {
            if (!string.IsNullOrEmpty(Name) && TotalMarks > 0 && !string.IsNullOrEmpty(SelectedSubjectId))
            {
                var a = new Assessment { Name = Name, TotalMarks = TotalMarks, SubjectId = SelectedSubjectId };
                _dataService.AddAssessment(a);
                RefreshAssessments();
                Name = string.Empty;
                TotalMarks = 0;
                SelectedSubjectId = string.Empty;
            }
        }

        private void EditAssessment(AssessmentDisplayModel assessment)
        {
            EditingAssessment = new Assessment
            {
                Id = assessment.Id,
                Name = assessment.Name,
                TotalMarks = assessment.TotalMarks,
                SubjectId = assessment.SubjectId
            };
        }

        private void UpdateAssessment()
        {
            if (EditingAssessment != null && !string.IsNullOrEmpty(EditingAssessment.Name) && EditingAssessment.TotalMarks > 0 && !string.IsNullOrEmpty(EditingAssessment.SubjectId))
            {
                _dataService.UpdateAssessment(EditingAssessment);
                RefreshAssessments();
                EditingAssessment = null;
            }
        }

        private void DeleteAssessment(AssessmentDisplayModel assessment)
        {
            if (MessageBox.Show($"Are you sure you want to delete {assessment.Name}? This will also delete all scores entered for it.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _dataService.DeleteAssessment(assessment.Id);
                RefreshAssessments();
            }
        }

        private void CancelEdit()
        {
            EditingAssessment = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}