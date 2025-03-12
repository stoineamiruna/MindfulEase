using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Models;

namespace MindfulEase.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets pentru fiecare model
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<WeeklyReport> WeeklyReports { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<TherapeuticGame> TherapeuticGames { get; set; }
        public DbSet<ApplicationUserTherapeuticGame> ApplicationUserTherapeuticGames { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<ApplicationUserQuiz> ApplicationUserQuizzes { get; set; }
        public DbSet<Emotion> Emotions { get; set; }
        public DbSet<DiaryEmotion> DiaryEmotions { get; set; }
        public DbSet<WeeklyChallenge> WeeklyChallenges { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ApplicationUserResource> ApplicationUserResources { get; set; }
        public DbSet<ApplicationUserQuestionQuiz> ApplicationUserQuestionQuizzes { get; set; }
        public DbSet<QuestionQuiz> QuestionQuizzes { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<UserObjective> UserObjectives { get; set; }
        public DbSet<UserObjectiveProgress> UserObjectiveProgresses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<ApplicationUserWeeklyChallenge> ApplicationUserWeeklyChallenges { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many Relationship: ApplicationUser <-> TherapeuticGame
            modelBuilder.Entity<ApplicationUserTherapeuticGame>()
                .HasKey(ut => new { ut.UserId, ut.GameId });

            modelBuilder.Entity<ApplicationUserTherapeuticGame>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.TherapeuticGames)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<ApplicationUserTherapeuticGame>()
                .HasOne(ut => ut.Game)
                .WithMany(g => g.Users)
                .HasForeignKey(ut => ut.GameId);

            // Many-to-Many Relationship: ApplicationUser <-> Quiz
            modelBuilder.Entity<ApplicationUserQuiz>()
                .HasKey(uq => new { uq.UserId, uq.QuizId });

            modelBuilder.Entity<ApplicationUserQuiz>()
                .HasOne(uq => uq.User)
                .WithMany(u => u.Quizzes)
                .HasForeignKey(uq => uq.UserId);

            modelBuilder.Entity<ApplicationUserQuiz>()
                .HasOne(uq => uq.Quiz)
                .WithMany(q => q.Users)
                .HasForeignKey(uq => uq.QuizId);

            // Many-to-Many Relationship: Diary <-> Emotion
            modelBuilder.Entity<DiaryEmotion>()
                .HasKey(ue => new { ue.DiaryId, ue.EmotionId });

            modelBuilder.Entity<DiaryEmotion>()
                .HasOne(ue => ue.Diary)
                .WithMany(u => u.Emotions)
                .HasForeignKey(ue => ue.DiaryId);

            modelBuilder.Entity<DiaryEmotion>()
                .HasOne(ue => ue.Emotion)
                .WithMany(e => e.Diaries)
                .HasForeignKey(ue => ue.EmotionId);

            // Many-to-Many Relationship: ApplicationUser <-> Resource
            modelBuilder.Entity<ApplicationUserResource>()
                .HasKey(ue => new { ue.UserId, ue.ResourceId });

            modelBuilder.Entity<ApplicationUserResource>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.Resources)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<ApplicationUserResource>()
                .HasOne(ue => ue.Resource)
                .WithMany(e => e.Users)
                .HasForeignKey(ue => ue.ResourceId);

            // Many-to-Many Relationship: ApplicationUser <-> QuestionQuiz
            modelBuilder.Entity<ApplicationUserQuestionQuiz>()
                .HasKey(uq => new { uq.UserId, uq.QuestionId });

            modelBuilder.Entity<ApplicationUserQuestionQuiz>()
                .HasOne(uq => uq.User)
                .WithMany(u => u.UserQuestionQuizzes)
                .HasForeignKey(uq => uq.UserId);

            modelBuilder.Entity<ApplicationUserQuestionQuiz>()
                .HasOne(uq => uq.Question)
                .WithMany(q => q.UserQuestionQuizzes)
                .HasForeignKey(uq => uq.QuestionId);

            // Many-to-Many Relationship: ApplicationUser <-> Objective
            modelBuilder.Entity<UserObjective>()
                .HasKey(ut => new { ut.UserId, ut.ObjectiveId });

            modelBuilder.Entity<UserObjective>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Objectives)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserObjective>()
                .HasOne(ut => ut.Objective)
                .WithMany(g => g.Users)
                .HasForeignKey(ut => ut.ObjectiveId);

            // Many-to-Many Relationship: ApplicationUser <-> Badge
            modelBuilder.Entity<UserBadge>()
                .HasKey(ut => new { ut.UserId, ut.BadgeId });

            modelBuilder.Entity<UserBadge>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserBadge>()
                .HasOne(ut => ut.Badge)
                .WithMany(g => g.Users)
                .HasForeignKey(ut => ut.BadgeId);
            // Many-to-Many Relationship: ApplicationUser <-> WeeklyChallenge
            modelBuilder.Entity< ApplicationUserWeeklyChallenge > ()
                .HasKey(ut => new { ut.UserId, ut.WeeklyChallengeId });

            modelBuilder.Entity<ApplicationUserWeeklyChallenge>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.WeeklyChallenges)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<ApplicationUserWeeklyChallenge>()
                .HasOne(ut => ut.WeeklyChallenge)
                .WithMany(g => g.Users)
                .HasForeignKey(ut => ut.WeeklyChallengeId);
        }
    }
}
