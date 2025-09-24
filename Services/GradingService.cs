using renkimsumatin.Models;

namespace renkimsumatin.Services
{
    public static class GradingService
    {
        public static Grade CalculateGrade(int? score, int totalMarks)
        {
            if (score == null || totalMarks <= 0)
            {
                return Grade.NA;
            }

            double percentage = (double)score.Value / totalMarks * 100;

            if (percentage >= 90) return Grade.A;
            if (percentage >= 75) return Grade.B;
            if (percentage >= 50) return Grade.C;
            if (percentage >= 25) return Grade.D;
            return Grade.F;
        }

        public static string GetGradeColor(Grade grade)
        {
            return grade switch
            {
                Grade.A => "#10B981", // Green
                Grade.B => "#0EA5E9", // Sky blue
                Grade.C => "#F59E0B", // Yellow
                Grade.D => "#F97316", // Orange
                Grade.F => "#EF4444", // Red
                _ => "#6B7280"        // Gray
            };
        }
    }
}