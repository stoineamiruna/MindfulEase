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
        public DbSet<ApplicationUserEmotion> ApplicationUserEmotions { get; set; }
        public DbSet<WeeklyChallenge> WeeklyChallenges { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Resource> Resources { get; set; }

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

            // Many-to-Many Relationship: ApplicationUser <-> Emotion
            modelBuilder.Entity<ApplicationUserEmotion>()
                .HasKey(ue => new { ue.UserId, ue.EmotionId });

            modelBuilder.Entity<ApplicationUserEmotion>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.Emotions)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<ApplicationUserEmotion>()
                .HasOne(ue => ue.Emotion)
                .WithMany(e => e.Users)
                .HasForeignKey(ue => ue.EmotionId);
        }
    }
}
