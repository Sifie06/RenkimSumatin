using renkimsumatin.Models;
using renkimsumatin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace renkimsumatin.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;

        public ReportsViewModel(DataService dataService)
        {
            _dataService = dataService;
            ExportCSVCommand = new RelayCommand(ExportCSV);
            ExportPDFCommand = new RelayCommand(ExportPDF);
            ExportRankingCommand = new RelayCommand(ExportRanking);
            ExportRanksCSVCommand = new RelayCommand(ExportRanksCSV);
            ExportRanksPDFCommand = new RelayCommand(ExportRanksPDF);
            RefreshCommand = new RelayCommand(Refresh);

            RankingScope = RankingScopes[0]; // default All Subjects

            // Initialize default report data view
            _reportData = BuildDefaultReportData();
        }

        // Backing collection to allow swapping content for previewing ranking
        private ObservableCollection<ReportData> _reportData;
        public ObservableCollection<ReportData> ReportData
        {
            get => _reportData;
            private set { _reportData = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ReportData> BuildDefaultReportData()
        {
            var data = new List<ReportData>();
            var students = _dataService.GetStudents();
            var subjects = _dataService.GetSubjects();
            var assessments = _dataService.GetAssessments();
            var scores = _dataService.GetScores();

            foreach (var score in scores)
            {
                var student = students.FirstOrDefault(s => s.Id == score.StudentId);
                var assessment = assessments.FirstOrDefault(a => a.Id == score.AssessmentId);

                if (student != null && assessment != null)
                {
                    var subject = subjects.FirstOrDefault(s => s.Id == assessment.SubjectId);
                    if (subject != null)
                    {
                        data.Add(new ReportData
                        {
                            StudentName = student.Name,
                            StudentId = student.StudentId,
                            SubjectName = subject.Name,
                            AssessmentName = assessment.Name,
                            TotalMarks = assessment.TotalMarks,
                            MarksObtained = score.MarksObtained,
                            Grade = GradingService.CalculateGrade(score.MarksObtained, assessment.TotalMarks)
                        });
                    }
                }
            }

            return new ObservableCollection<ReportData>(data.OrderBy(d => d.StudentName));
        }

        // Ranking selection
        public List<string> RankingScopes { get; } = new() { "All Subjects", "By Subject", "By Assessment" };
        private string _rankingScope = "All Subjects";
        public string RankingScope
        {
            get => _rankingScope;
            set { _rankingScope = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Subject> RankingSubjects => _dataService.GetSubjects();
        private string? _selectedRankingSubjectId;
        public string? SelectedRankingSubjectId
        {
            get => _selectedRankingSubjectId;
            set
            {
                _selectedRankingSubjectId = value;
                // Clear assessment selection if it no longer matches the subject filter
                if (!string.IsNullOrEmpty(SelectedRankingAssessmentId))
                {
                    var currentAssessment = _dataService.GetAssessments().FirstOrDefault(a => a.Id == SelectedRankingAssessmentId);
                    if (currentAssessment == null || currentAssessment.SubjectId != _selectedRankingSubjectId)
                    {
                        SelectedRankingAssessmentId = null;
                        OnPropertyChanged(nameof(SelectedRankingAssessmentId));
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredRankingAssessments));
            }
        }

        // Filtered assessments based on selected subject
        public ObservableCollection<Assessment> FilteredRankingAssessments
        {
            get
            {
                var all = _dataService.GetAssessments();
                if (string.IsNullOrEmpty(SelectedRankingSubjectId)) return all;
                return new ObservableCollection<Assessment>(all.Where(a => a.SubjectId == SelectedRankingSubjectId));
            }
        }

        private string? _selectedRankingAssessmentId;
        public string? SelectedRankingAssessmentId
        {
            get => _selectedRankingAssessmentId;
            set { _selectedRankingAssessmentId = value; OnPropertyChanged(); }
        }

        public RelayCommand ExportCSVCommand { get; }
        public RelayCommand ExportPDFCommand { get; }
        public RelayCommand ExportRankingCommand { get; }
        public RelayCommand ExportRanksCSVCommand { get; }
        public RelayCommand ExportRanksPDFCommand { get; }
        public RelayCommand RefreshCommand { get; }

        private void Refresh()
        {
            // Reset to default listing and refresh pickers
            ReportData = BuildDefaultReportData();
            OnPropertyChanged(nameof(RankingSubjects));
            OnPropertyChanged(nameof(FilteredRankingAssessments));
        }

        private void ExportCSV()
        {
            ExportService.ExportToCSV(ReportData.ToList(), "student_grades_report");
        }

        private void ExportPDF()
        {
            ExportService.ExportToPDF(ReportData.ToList(), "student_grades_report");
        }

        private (List<ReportData> ranking, string filenameSuffix) BuildRankingData()
        {
            var students = _dataService.GetStudents();
            var subjects = _dataService.GetSubjects();
            var assessments = _dataService.GetAssessments();
            var scores = _dataService.GetScores();

            IEnumerable<string> targetAssessmentIds = assessments.Select(a => a.Id);
            string subjectNameForDisplay = "All Subjects";
            string assessmentNameForDisplay = "Total";
            string filenameSuffix = "all";

            if (RankingScope == "By Subject" && !string.IsNullOrEmpty(SelectedRankingSubjectId))
            {
                targetAssessmentIds = assessments.Where(a => a.SubjectId == SelectedRankingSubjectId).Select(a => a.Id);
                subjectNameForDisplay = subjects.FirstOrDefault(s => s.Id == SelectedRankingSubjectId)?.Name ?? "Subject";
                assessmentNameForDisplay = "Total";
                filenameSuffix = $"subject_{subjectNameForDisplay}";
            }
            else if (RankingScope == "By Assessment" && !string.IsNullOrEmpty(SelectedRankingAssessmentId))
            {
                targetAssessmentIds = assessments.Where(a => a.Id == SelectedRankingAssessmentId).Select(a => a.Id);
                var assess = assessments.FirstOrDefault(a => a.Id == SelectedRankingAssessmentId);
                subjectNameForDisplay = subjects.FirstOrDefault(s => s.Id == assess?.SubjectId)?.Name ?? "Subject";
                assessmentNameForDisplay = assess?.Name ?? "Assessment";
                filenameSuffix = $"assessment_{assess?.Name ?? "assessment"}";
            }

            var includedAssessments = assessments.Where(a => targetAssessmentIds.Contains(a.Id)).ToList();

            var totals = scores
                .Where(s => targetAssessmentIds.Contains(s.AssessmentId))
                .GroupBy(s => s.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    TotalObtained = g.Sum(s => s.MarksObtained ?? 0),
                    TotalPossible = includedAssessments.Where(a => g.Any(sc => sc.AssessmentId == a.Id)).Sum(a => a.TotalMarks)
                })
                .ToList();

            var ordered = totals.OrderByDescending(r => r.TotalObtained).ToList();

            var ranking = ordered
                .Select((r, idx) => new { r, Rank = idx + 1 })
                .Join(students, x => x.r.StudentId, st => st.Id, (x, st) => new ReportData
                {
                    StudentName = st.Name,
                    StudentId = st.StudentId,
                    SubjectName = subjectNameForDisplay,
                    AssessmentName = assessmentNameForDisplay,
                    TotalMarks = x.r.TotalPossible,
                    MarksObtained = x.r.TotalObtained,
                    Grade = GradingService.CalculateGrade(x.r.TotalObtained, x.r.TotalPossible),
                    Rank = x.Rank
                })
                .ToList();

            return (ranking, filenameSuffix);
        }

        private void ExportRanking()
        {
            // Populate grid with preview of ranking; user can export via CSV/PDF buttons
            var (ranking, _) = BuildRankingData();
            ReportData = new ObservableCollection<ReportData>(ranking);
        }

        private void ExportRanksCSV()
        {
            var (ranking, filenameSuffix) = BuildRankingData();
            ExportService.ExportRanksToCSV(ranking, $"student_ranking_{filenameSuffix}");
        }

        private void ExportRanksPDF()
        {
            var (ranking, filenameSuffix) = BuildRankingData();
            ExportService.ExportRanksToPDF(ranking, $"student_ranking_{filenameSuffix}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}