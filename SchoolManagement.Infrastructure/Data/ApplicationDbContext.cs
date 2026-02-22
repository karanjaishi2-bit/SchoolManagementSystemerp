using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Entities.Students;
using SchoolManagement.Core.Entities.Teachers;
using SchoolManagement.Core.Entities.Classes;
using SchoolManagement.Core.Entities.Subjects;
using SchoolManagement.Core.Entities.Attendance;
using SchoolManagement.Core.Entities.Fees;
using SchoolManagement.Core.Entities.Results;
using SchoolManagement.Core.Entities.Settings;
using SchoolManagement.Core.Entities.HR;
using SchoolManagement.Core.Entities.Staff;
using SchoolManagement.Core.Entities.Auth;


namespace SchoolManagement.Infrastructure.Data
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
        public DbSet<ClassData> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<FeeItem> FeeItems { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<FeeBill> FeeBills { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<SchoolSettings> SchoolSettings { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Core.Entities.Staff.Staff> Staff { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Gender).HasMaxLength(20);     
                entity.Property(e => e.Photo).HasMaxLength(500);     
                entity.Property(e => e.StudentId).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.StudentId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Teacher Configuration
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TeacherId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Gender).HasMaxLength(20);     // ADD THIS
                entity.Property(e => e.Photo).HasMaxLength(500);     // ADD THIS
                entity.HasIndex(e => e.TeacherId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Grade Configuration
            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Grades)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Result Configuration
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Percentage).HasColumnType("decimal(5,2)");
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Enrollment Configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        
            // Attendance Configuration
            modelBuilder.Entity<Core.Entities.Attendance.Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClassId).HasMaxLength(50);
                entity.Property(e => e.Date).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            });

            // Fee Configuration
            modelBuilder.Entity<Fee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StudentName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            // FeeItem Configuration
            modelBuilder.Entity<FeeItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FeeHead).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FeeType).IsRequired().HasMaxLength(50);
            });

            // FeeStructure Configuration
            modelBuilder.Entity<FeeStructure>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClassId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClassName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.FeeItems)
                    .WithOne()
                    .HasForeignKey(f => f.FeeStructureId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // FeeBill Configuration
            modelBuilder.Entity<FeeBill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StudentName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BalanceAmount).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.FeeItems)
                    .WithOne()
                    .HasForeignKey(f => f.FeeBillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Staff Configuration (REPLACE old configuration)
            modelBuilder.Entity<Core.Entities.Staff.Staff>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.StaffId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.Photo).HasMaxLength(500);
                entity.HasIndex(e => e.StaffId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

           

            // ClassData Configuration
            modelBuilder.Entity<ClassData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Subject Configuration
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TeacherName).HasMaxLength(200);
                entity.Property(e => e.ClassId).HasMaxLength(50);
                entity.Property(e => e.ClassName).HasMaxLength(100);
                entity.HasIndex(e => e.Code).IsUnique();
            });
            // Payroll Configuration
            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Allowances).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Deductions).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetSalary).HasColumnType("decimal(18,2)");
            });
            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Set default values for CreatedAt
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var createdAtProperty = entityType.FindProperty("CreatedAt");
                if (createdAtProperty != null)
                {
                    createdAtProperty.SetDefaultValueSql("GETUTCDATE()");
                }
            }
        }
    }
}