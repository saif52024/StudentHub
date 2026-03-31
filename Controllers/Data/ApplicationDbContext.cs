using Microsoft.EntityFrameworkCore;
using StudentHub.Models;

namespace StudentHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Table mappings
            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Department>().ToTable("departments");
            modelBuilder.Entity<Mark>().ToTable("marks");
            modelBuilder.Entity<Attendance>().ToTable("attendances");

            // ✅ Department ↔ Student (real FK, no shadow)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students) // ✅ FIXED: link to collection
                .HasForeignKey(s => s.DepartmentId)
                .HasConstraintName("FK_Students_Departments_DepartmentId")
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Marks cascade delete
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Attendance cascade delete
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


    }
}
