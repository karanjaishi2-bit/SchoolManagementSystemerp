using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<BillingMaster> BillingMasters { get; set; }
        public DbSet<BillingItem> BillingItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BillingMaster -> BillingItems relationship
            modelBuilder.Entity<BillingItem>()
                .HasOne(bi => bi.BillingMaster)
                .WithMany(bm => bm.BillingItems)
                .HasForeignKey(bi => bi.BillingMasterId)
                .OnDelete(DeleteBehavior.Cascade);

            // BillingMaster -> Student relationship
            modelBuilder.Entity<BillingMaster>()
                .HasOne(bm => bm.Student)
                .WithMany(s => s.BillingMasters) // Student model must have ICollection<BillingMaster>
                .HasForeignKey(bm => bm.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Composite key for Enrollment
            // modelBuilder.Entity<Enrollment>().HasKey(e => new { e.StudentId, e.CourseId });
        }
    }
}