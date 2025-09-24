using System;
using System.Globalization;
using System.Windows.Data;
using renkimsumatin.Models;
using renkimsumatin.Services;

namespace renkimsumatin.Converters
{
    public class ScoreToGradeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Expecting values[0] = MarksObtained (int?), values[1] = TotalMarks (int)
            if (values == null || values.Length < 2)
                return Grade.NA.ToString();

            int? score = null;
            if (values[0] is int i0)
                score = i0;
            else if (values[0] is int?)
                score = (int?)values[0];

            int totalMarks;
            if (values[1] is int i1)
                totalMarks = i1;
            else if (values[1] is int?)
                totalMarks = ((int?)values[1]) ?? 0;
            else
                return Grade.NA.ToString();

            if (score.HasValue)
                return GradingService.CalculateGrade(score.Value, totalMarks).ToString();

            return Grade.NA.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}