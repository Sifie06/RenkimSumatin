using System;
using System.ComponentModel.DataAnnotations;

namespace renkimsumatin.Models
{
    public class Assessment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int TotalMarks { get; set; }
        [Required]
        public string SubjectId { get; set; } = string.Empty;

        public Assessment() { }
    }
}