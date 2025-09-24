using System;
using System.ComponentModel.DataAnnotations;

namespace renkimsumatin.Models
{
    public class Score
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string StudentId { get; set; } = string.Empty;
        [Required]
        public string AssessmentId { get; set; } = string.Empty;
        public int? MarksObtained { get; set; }

        public Score() { }
    }
}