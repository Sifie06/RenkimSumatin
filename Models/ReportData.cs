namespace renkimsumatin.Models
{
    public class ReportData
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string AssessmentName { get; set; } = string.Empty;
        public int TotalMarks { get; set; }
        public int? MarksObtained { get; set; }
        public Grade Grade { get; set; }
        public int Rank { get; set; }
    }
}