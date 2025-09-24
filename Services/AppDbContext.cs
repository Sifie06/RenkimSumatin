using Microsoft.EntityFrameworkCore;
using renkimsumatin.Models;
using System.Collections.Generic;
using System;
using System.IO;

namespace renkimsumatin.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "renkimsumatin.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}