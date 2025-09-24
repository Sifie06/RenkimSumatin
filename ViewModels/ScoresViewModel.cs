using renkimsumatin.Models;
using renkimsumatin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace renkimsumatin.ViewModels
{
    public class ScoresViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;

        private string? _selectedStudentId;
        private string? _selectedSubjectId;
        private string? _selectedAssessmentId;
        private int? _marksObtained;

        public ScoresViewModel(DataService dataService)
        {
            _dataService = dataService;
            Refresh();
            AddOrUpdateScoreCommand = new RelayCommand(AddOrUpdateScore);
            DeleteScoreCommand = new RelayCommand<ScoreDisplayModel>(DeleteScore);
            RefreshCommand = new RelayCommand(Refresh);
        }

        public ObservableCollection<ScoreDisplayModel> Scores { get; set; } = new();
        public ObservableCollection<Student> Students { get; set; } = new();
        public ObservableCollection<Subject> Subjects { get; set; } = new();
        public ObservableCollection<Assessment> Assessments { get; set; } = new();

        public string? SelectedStudentId
        {
            get => _selectedStudentId;
            set { _selectedStudentId = value; OnPropertyChanged(); }
        }

        public string? SelectedSubjectId
        {
            get => _selectedSubjectId;
            set { _selectedSubjectId = value; OnPropertyChanged(); FilterAssessmentsBySubject(); }
        }

        public string? SelectedAssessmentId
        {
            get => _selectedAssessmentId;
            set { _selectedAssessmentId = value; OnPropertyChanged(); }
        }

        public int? MarksObtained
        {
            get => _marksObtained;
            set { _marksObtained = value; OnPropertyChanged(); }
        }

        public RelayCommand AddOrUpdateScoreCommand { get; }
        public RelayCommand<ScoreDisplayModel> DeleteScoreCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public void Refresh()
        {
            var students = _dataService.GetStudents();
            var subjects = _dataService.GetSubjects();
            var assessments = _dataService.GetAssessments();
            var scores = _dataService.GetScores();

            Students = students;
            Subjects = subjects;
            _allAssessments = assessments.ToList();
            FilterAssessmentsBySubject();
            OnPropertyChanged(nameof(Students));
            OnPropertyChanged(nameof(Subjects));
            OnPropertyChanged(nameof(Assessments));

            Scores.Clear();
            foreach (var score in scores)
            {
                var student = students.FirstOrDefault(s => s.Id == score.StudentId);
                var assessment = assessments.FirstOrDefault(a => a.Id == score.AssessmentId);
                if (student != null && assessment != null)
                {
                    Scores.Add(new ScoreDisplayModel
                    {
                        Id = score.Id,
                        StudentId = student.Id,
                        StudentName = student.Name,
                        AssessmentId = assessment.Id,
                        AssessmentName = assessment.Name,
                        MarksObtained = score.MarksObtained,
                        TotalMarks = assessment.TotalMarks,
                        Grade = GradingService.CalculateGrade(score.MarksObtained, assessment.TotalMarks).ToString()
                    });
                }
            }
        }

        private List<Assessment> _allAssessments = new();
        private void FilterAssessmentsBySubject()
        {
            if (string.IsNullOrEmpty(SelectedSubjectId))
            {
                Assessments = new ObservableCollection<Assessment>(_allAssessments);
            }
            else
            {
                Assessments = new ObservableCollection<Assessment>(_allAssessments.Where(a => a.SubjectId == SelectedSubjectId));
            }
            OnPropertyChanged(nameof(Assessments));
        }

        private void AddOrUpdateScore()
        {
            if (!string.IsNullOrEmpty(SelectedStudentId) && !string.IsNullOrEmpty(SelectedAssessmentId))
            {
                var assessment = Assessments.FirstOrDefault(a => a.Id == SelectedAssessmentId);
                if (assessment != null && MarksObtained.HasValue && MarksObtained.Value > assessment.TotalMarks)
                {
                    MessageBox.Show($"Entered marks ({MarksObtained}) exceed Total Marks ({assessment.TotalMarks}). It will be set to {assessment.TotalMarks}.",
                        "Score exceeds Total Marks", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MarksObtained = assessment.TotalMarks;
                }

                _dataService.UpdateScore(SelectedStudentId!, SelectedAssessmentId!, MarksObtained);
                Refresh();
                MarksObtained = null;
            }
        }

        private void DeleteScore(ScoreDisplayModel score)
        {
            _dataService.UpdateScore(score.StudentId, score.AssessmentId, null);
            Refresh();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}