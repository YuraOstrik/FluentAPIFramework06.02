using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Fluent_API.Framework03._02
{
    class FluentContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<DoctorExamination> DoctorsExaminations { get; set; }

        public FluentContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-N63S67G\SQLEXPRESS;Database=FluentNew;TrustServerCertificate=True;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasIndex(d => d.Name).IsUnique();
            modelBuilder.Entity<Department>().ToTable(t => t.HasCheckConstraint("Building", "Building > 0 AND Building < 6"));
            modelBuilder.Entity<Examination>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<Ward>().HasIndex(w => w.Name).IsUnique();
            
        }
    }

    public class Department
    {
        public int Id { get; set; }
        public int Building { get; set; }
        public string Name { get; set; }
    }

    public class Ward
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Places { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }

    public class Doctor
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Premium { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string Surname { get; set; }
    }

    public class Examination
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class DoctorExamination
    {
        public int Id { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int ExaminationId { get; set; }
        public Examination Examination { get; set; }
        public int WardId { get; set; }
        public Ward Ward { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (FluentContext db = new FluentContext())
                {
                    // Добавляем отделы
                    if(!db.Departments.Any())
                    {
                        db.Departments.AddRange(
                        new Department { Building = 1, Name = "Cardiology" },
                        new Department { Building = 2, Name = "Neurology" });

                        db.Doctors.AddRange(
                        new Doctor { Name = "John", Surname = "Doe", Salary = 5000 },
                        new Doctor { Name = "Jane", Surname = "Smith", Salary = 5500 });

                        db.Examinations.AddRange(
                            new Examination { Name = "MRI" },
                            new Examination { Name = "X-Ray" });
                    }

                    if(!db.Wards.Any())
                    {
                        db.Wards.AddRange(
                            new Ward { Name = "Ward A", Places = 3, DepartmentId = 1 },
                            new Ward { Name = "Ward B", Places = 5, DepartmentId = 2 });
                    }


                    db.DoctorsExaminations.AddRange(
                        new DoctorExamination { DoctorId = 1, ExaminationId = 1, WardId = 1, StartTime = TimeSpan.Parse("09:00:00"), EndTime = TimeSpan.Parse("10:00:00") },
                        new DoctorExamination { DoctorId = 2, ExaminationId = 2, WardId = 2, StartTime = TimeSpan.Parse("11:00:00"), EndTime = TimeSpan.Parse("12:00:00") });
                    db.SaveChanges();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
            }
        }
    }
}
