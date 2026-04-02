using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Data
{
    public class OldDbContext : DbContext
    {
        public OldDbContext(DbContextOptions<OldDbContext> options)
            : base(options)
        {
        }

        // ------------------------
        //  DB SETS (TABLES)
        // ------------------------

        public DbSet<Major> Majors { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<UniversityMajor> UniversityMajors { get; set; }

        public DbSet<MajorTrait> MajorTraits { get; set; }
        public DbSet<MajorSubField> MajorSubFields { get; set; }

        public DbSet<QuestionGroup> QuestionGroups { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerImpact> AnswerImpacts { get; set; }

        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestAnswer> TestAnswers { get; set; }
        public DbSet<TestAnswerImpact> TestAnswerImpacts { get; set; }
        public DbSet<TestResultMajorScore> TestResultMajorScores { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Counselor> Counselors { get; set; }

        // *** NEW TABLES Roadmap ***
        public DbSet<Roadmap> Roadmaps { get; set; }
        public DbSet<RoadmapStage> RoadmapStages { get; set; }
        public DbSet<RoadmapItem> RoadmapItems { get; set; }
        public DbSet<RoadmapSkill> RoadmapSkills { get; set; }

        public DbSet<CounselingSchedule> CounselingSchedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Admin> Admins { get; set; }

        





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------
            // EXISTING RELATIONSHIPS
            // ------------------------------

            // Major -> MajorTraits
            modelBuilder.Entity<Major>()
                .HasMany(m => m.MajorTraits)
                .WithOne(t => t.Major)
                .HasForeignKey(t => t.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            // AnswerImpact -> Answer
            modelBuilder.Entity<AnswerImpact>()
                .HasOne(ai => ai.Answer)
                .WithMany(a => a.Impacts)
                .HasForeignKey(ai => ai.AnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // AnswerImpact -> Major
            modelBuilder.Entity<AnswerImpact>()
                .HasOne(ai => ai.Major)
                .WithMany(m => m.AnswerImpacts)
                .HasForeignKey(ai => ai.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            // AnswerImpact -> MajorTrait
            modelBuilder.Entity<AnswerImpact>()
                .HasOne(ai => ai.MajorTrait)
                .WithMany()
                .HasForeignKey(ai => ai.MajorTraitId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswer -> TestResult
            modelBuilder.Entity<TestAnswer>()
                .HasOne(ta => ta.TestResult)
                .WithMany(tr => tr.TestAnswers)
                .HasForeignKey(ta => ta.TestResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswer -> Question
            modelBuilder.Entity<TestAnswer>()
                .HasOne(ta => ta.Question)
                .WithMany()
                .HasForeignKey(ta => ta.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswer -> Answer
            modelBuilder.Entity<TestAnswer>()
                .HasOne(ta => ta.Answer)
                .WithMany()
                .HasForeignKey(ta => ta.AnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswerImpact -> TestAnswer
            modelBuilder.Entity<TestAnswerImpact>()
                .HasOne(ti => ti.TestAnswer)
                .WithMany(ta => ta.Impacts)
                .HasForeignKey(ti => ti.TestAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswerImpact -> Major
            modelBuilder.Entity<TestAnswerImpact>()
                .HasOne(ti => ti.Major)
                .WithMany()
                .HasForeignKey(ti => ti.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestAnswerImpact -> MajorTrait
            modelBuilder.Entity<TestAnswerImpact>()
                .HasOne(ti => ti.MajorTrait)
                .WithMany()
                .HasForeignKey(ti => ti.MajorTraitId)
                .OnDelete(DeleteBehavior.Restrict);

            // TestResult -> MajorScore
            modelBuilder.Entity<TestResultMajorScore>()
                .HasOne(ms => ms.TestResult)
                .WithMany(tr => tr.MajorScores)
                .HasForeignKey(ms => ms.TestResultId)
                .OnDelete(DeleteBehavior.Restrict);

                // Counselor -> CounselingSchedule (1 - nhiều)
    modelBuilder.Entity<CounselingSchedule>()
    .HasOne(cs => cs.Counselor)
    .WithMany(c => c.Schedules)
    .HasForeignKey(cs => cs.CounselorId)
    .OnDelete(DeleteBehavior.Cascade);


            // ---------------------------------------------------
            //             NEW RELATIONSHIPS: ROADMAP
            // ---------------------------------------------------

            // Major -> Roadmaps
            modelBuilder.Entity<Roadmap>()
                .HasOne(r => r.Major)
                .WithMany() 
                .HasForeignKey(r => r.MajorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roadmap -> Stages
            modelBuilder.Entity<Roadmap>()
                .HasMany(r => r.Stages)
                .WithOne(s => s.Roadmap)
                .HasForeignKey(s => s.RoadmapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stage -> Items
            modelBuilder.Entity<RoadmapStage>()
                .HasMany(s => s.Items)
                .WithOne(i => i.Stage)
                .HasForeignKey(i => i.StageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Item -> Skills
            modelBuilder.Entity<RoadmapItem>()
                .HasMany(i => i.Skills)
                .WithOne(s => s.Item)
                .HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
                // ---------------------------------------------------
//        NEW RELATIONSHIPS: UNIVERSITY - MAJOR
// ---------------------------------------------------

// Composite Key cho bảng trung gian
modelBuilder.Entity<UniversityMajor>()
    .HasKey(um => new { um.UniversityId, um.MajorId });

// UniversityMajor -> University
modelBuilder.Entity<UniversityMajor>()
    .HasOne(um => um.University)
    .WithMany(u => u.UniversityMajors)
    .HasForeignKey(um => um.UniversityId)
    .OnDelete(DeleteBehavior.Cascade);

// UniversityMajor -> Major
modelBuilder.Entity<UniversityMajor>()
    .HasOne(um => um.Major)
    .WithMany(m => m.UniversityMajors)
    .HasForeignKey(um => um.MajorId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
