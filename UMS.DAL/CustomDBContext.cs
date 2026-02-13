using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UMS.DAL.Entities;

namespace UMS.DAL
{
    public class CustomDBContext : DbContext
    {
        public CustomDBContext(DbContextOptions<CustomDBContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Section)
                .WithMany(sec => sec.Students)
                .HasForeignKey(s => s.SectionId);

            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Teachers)
                .WithMany(t => t.Lessons)
                .UsingEntity(j => j.ToTable("TeacherLesson"));

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Lesson)
                .WithMany() 
                .HasForeignKey(e => e.LessonId);

            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Sections)
                .WithMany(s => s.Lessons)
                .UsingEntity(j => j.ToTable("LessonSection"));

            modelBuilder.Entity<Schedule>()
                .HasOne(sc => sc.Lesson)
                .WithMany()
                .HasForeignKey(sc => sc.LessonId);

            modelBuilder.Entity<Schedule>()
                .HasOne(sc => sc.Section)
                .WithMany()
                .HasForeignKey(sc => sc.SectionId);

            modelBuilder.Entity<Schedule>()
                .HasMany(sc => sc.Teachers)
                .WithMany()
                .UsingEntity(j => j.ToTable("ScheduleTeacher"));

            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne()
                .HasForeignKey<User>(u => u.StudentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Teacher)
                .WithOne()
                .HasForeignKey<User>(u => u.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public override int SaveChanges()
        {
            HandleSoftDeletes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDeletes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDeletes()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            if (!entries.Any()) return;

            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedTime = utcNow;
                entry.Entity.LastModifiedTime = utcNow;
            }
        }
    }
}
