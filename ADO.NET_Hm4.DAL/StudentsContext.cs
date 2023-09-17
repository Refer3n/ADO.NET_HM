using Microsoft.EntityFrameworkCore;
using Pract4.DAL.Entities;

namespace Pract4.DAL
{
    public class StudentsContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<StudentCard> StudentCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conString = "Data Source=DESKTOP-VI7HLAA\\SQLSERVER;Initial Catalog=Students;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(conString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentCard)
                .WithOne(s => s.Student)
                .HasForeignKey<StudentCard>(sc => sc.Id);
        }
    }
}