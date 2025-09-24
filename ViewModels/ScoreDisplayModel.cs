namespace renkimsumatin.ViewModels
{
    public class ScoreDisplayModel
    {
        public string Id { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string AssessmentId { get; set; } = string.Empty;
        public string AssessmentName { get; set; } = string.Empty;
        public int? MarksObtained { get; set; }
        public int TotalMarks { get; set; }
        public string Grade { get; set; } = string.Empty;
    }
}