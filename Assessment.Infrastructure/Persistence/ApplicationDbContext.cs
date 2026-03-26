using Assessment.Domain.Entities;
using Assessment.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Question> Questions => Set<Question>();
        public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
        public DbSet<TechStack> TechStacks => Set<TechStack>();

        public DbSet<HrTestQuestion> HrTestQuestions => Set<HrTestQuestion>();

        public DbSet<HrApplicant> HrApplicants => Set<HrApplicant>();
        public DbSet<HrTest> HrTests => Set<HrTest>();
        public DbSet<HrTestTechStack> HrTestTechStacks => Set<HrTestTechStack>();

        public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();

        public DbSet<TestSnapshot> TestSnapshots => Set<TestSnapshot>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<HrTestTechStack>()
                .HasKey(x => new { x.TestId, x.TechStackId });

            b.Entity<Question>()
                .HasOne(q => q.TechStack)
                .WithMany()
                .HasForeignKey(q => q.TechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<UserAnswer>()
                .HasOne(x => x.Test)
                .WithMany()
                .HasForeignKey(x => x.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<UserAnswer>()
                .HasOne(x => x.Applicant)
                .WithMany()
                .HasForeignKey(x => x.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<UserAnswer>()
                .HasOne(x => x.Question)
                .WithMany()
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<UserAnswer>()
                .HasOne(x => x.SelectedOption)
                .WithMany()
                .HasForeignKey(x => x.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<HrTestQuestion>()
                .HasKey(x => new { x.TestId, x.QuestionId });

            b.Entity<HrTestQuestion>()
                .HasOne(x => x.Test)
                .WithMany()
                .HasForeignKey(x => x.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<HrTestQuestion>()
                .HasOne(x => x.Question)
                .WithMany()
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<HrTestQuestion>()
                .HasIndex(x => new { x.TestId, x.Order })
                .IsUnique();

            b.Entity<HrTest>()
                .Property(x => x.ScorePercentage)
                .HasColumnType("decimal(5,2)");

           
            b.Entity<TestSnapshot>()
                .HasOne(s => s.Test)
                .WithMany()
                .HasForeignKey(s => s.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<TestSnapshot>()
                .HasOne(s => s.Applicant)
                .WithMany()
                .HasForeignKey(s => s.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<TestSnapshot>()
                .Property(s => s.ImageData)
                .HasColumnType("nvarchar(max)");
        }
    }
}