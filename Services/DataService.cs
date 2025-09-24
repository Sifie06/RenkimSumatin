using renkimsumatin.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace renkimsumatin.Services
{
    public class DataService
    {
        private readonly AppDbContext _context;

        public DataService()
        {
            _context = new AppDbContext();
            var hasMigrations = _context.Database.GetMigrations().Any();
            if (hasMigrations)
            {
                _context.Database.Migrate(); // Apply existing migrations
            }
            else
            {
                _context.Database.EnsureCreated(); // Create tables from model when no migrations exist
            }

#if DEBUG
            // Seed minimal data on first run in DEBUG to verify schema and persistence
            if (!_context.Students.Any() && !_context.Subjects.Any() && !_context.Assessments.Any() && !_context.Scores.Any())
            {
                var student = new Student { Name = "Test Student", StudentId = "S001" };
                var subject = new Subject { Name = "Mathematics", Code = "MATH101" };
                var assessment = new Assessment { Name = "Midterm", TotalMarks = 100, SubjectId = subject.Id };

                _context.Students.Add(student);
                _context.Subjects.Add(subject);
                _context.Assessments.Add(assessment);
                _context.SaveChanges();

                var score = new Score { StudentId = student.Id, AssessmentId = assessment.Id, MarksObtained = 85 };
                _context.Scores.Add(score);
                _context.SaveChanges();
            }
#endif
        }

        // Student methods
        public ObservableCollection<Student> GetStudents()
            => new ObservableCollection<Student>(_context.Students.ToList());

        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void DeleteStudent(string studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
            {
                // Remove related scores first
                var relatedScores = _context.Scores.Where(s => s.StudentId == studentId);
                _context.Scores.RemoveRange(relatedScores);

                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        // Subject methods
        public ObservableCollection<Subject> GetSubjects()
            => new ObservableCollection<Subject>(_context.Subjects.ToList());

        public void AddSubject(Subject subject)
        {
            _context.Subjects.Add(subject);
            _context.SaveChanges();
        }

        public void UpdateSubject(Subject subject)
        {
            _context.Subjects.Update(subject);
            _context.SaveChanges();
        }

        public void DeleteSubject(string subjectId)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == subjectId);
            if (subject != null)
            {
                // Remove related assessments and their scores
                var relatedAssessments = _context.Assessments.Where(a => a.SubjectId == subjectId).Select(a => a.Id).ToList();
                var relatedScores = _context.Scores.Where(s => relatedAssessments.Contains(s.AssessmentId));
                _context.Scores.RemoveRange(relatedScores);

                _context.Assessments.RemoveRange(_context.Assessments.Where(a => a.SubjectId == subjectId));

                _context.Subjects.Remove(subject);
                _context.SaveChanges();
            }
        }

        // Assessment methods
        public ObservableCollection<Assessment> GetAssessments()
            => new ObservableCollection<Assessment>(_context.Assessments.ToList());

        public void AddAssessment(Assessment assessment)
        {
            _context.Assessments.Add(assessment);
            _context.SaveChanges();
        }

        public void UpdateAssessment(Assessment assessment)
        {
            // Attach and mark modified to avoid replacing tracked instances unexpectedly
            var tracked = _context.Assessments.FirstOrDefault(a => a.Id == assessment.Id);
            if (tracked != null)
            {
                tracked.Name = assessment.Name;
                tracked.TotalMarks = assessment.TotalMarks;
                tracked.SubjectId = assessment.SubjectId;
            }
            else
            {
                _context.Assessments.Update(assessment);
            }
            _context.SaveChanges();
        }

        public void DeleteAssessment(string assessmentId)
        {
            var assessment = _context.Assessments.FirstOrDefault(a => a.Id == assessmentId);
            if (assessment != null)
            {
                // Remove related scores
                _context.Scores.RemoveRange(_context.Scores.Where(s => s.AssessmentId == assessmentId));

                _context.Assessments.Remove(assessment);
                _context.SaveChanges();
            }
        }

        // Score methods
        public ObservableCollection<Score> GetScores()
            => new ObservableCollection<Score>(_context.Scores.ToList());

        public void UpdateScore(string studentId, string assessmentId, int? marks)
        {
            var existingScore = _context.Scores.FirstOrDefault(s => s.StudentId == studentId && s.AssessmentId == assessmentId);

            if (existingScore != null)
            {
                if (marks.HasValue)
                {
                    // Ensure marks do not exceed total marks for the assessment
                    var total = _context.Assessments.Where(a => a.Id == assessmentId).Select(a => a.TotalMarks).FirstOrDefault();
                    if (marks.Value > total) marks = total;
                    existingScore.MarksObtained = marks;
                }
                else
                {
                    _context.Scores.Remove(existingScore);
                }
            }
            else if (marks.HasValue)
            {
                var total = _context.Assessments.Where(a => a.Id == assessmentId).Select(a => a.TotalMarks).FirstOrDefault();
                if (marks.Value > total) marks = total;
                _context.Scores.Add(new Score
                {
                    StudentId = studentId,
                    AssessmentId = assessmentId,
                    MarksObtained = marks
                });
            }

            _context.SaveChanges();
        }
    }
}