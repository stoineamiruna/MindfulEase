using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class WeeklyChallengeUnitTest
    {
        [Fact]
        public void WeeklyChallenge_Properties_InitializedCorrectly()
        {
            // Arrange
            var weeklyChallenge = new WeeklyChallenge
            {
                Id = 1,
                Title = "Weekly Stress Challenge",
                Description = "Complete the stress-relief activities this week.",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                RequiredPoints = 50,
                RequiredJournalEntries = 5,
                RequiredQuizzes = 3,
                RequiredResources = 2,
                URLBackground = "background_image_url"
            };

            // Act & Assert
            Assert.Equal(1, weeklyChallenge.Id);
            Assert.Equal("Weekly Stress Challenge", weeklyChallenge.Title);
            Assert.Equal("Complete the stress-relief activities this week.", weeklyChallenge.Description);
            Assert.True(weeklyChallenge.StartDate <= DateTime.Now && weeklyChallenge.EndDate >= DateTime.Now);
            Assert.Equal(50, weeklyChallenge.RequiredPoints);
            Assert.Equal(5, weeklyChallenge.RequiredJournalEntries);
            Assert.Equal(3, weeklyChallenge.RequiredQuizzes);
            Assert.Equal(2, weeklyChallenge.RequiredResources);
            Assert.Equal("background_image_url", weeklyChallenge.URLBackground);
        }

        [Fact]
        public void WeeklyChallenge_ShouldHaveUsers()
        {
            // Arrange
            var weeklyChallenge = new WeeklyChallenge
            {
                Id = 1,
                Users = new List<ApplicationUserWeeklyChallenge>
                {
                    new ApplicationUserWeeklyChallenge { UserId = "user123" }
                }
            };

            // Act & Assert
            Assert.NotNull(weeklyChallenge.Users);
            Assert.Single(weeklyChallenge.Users);
        }
    }
}
