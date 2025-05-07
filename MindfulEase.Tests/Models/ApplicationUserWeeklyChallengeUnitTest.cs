using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserWeeklyChallengeUnitTest
    {
        [Fact]
        public void ApplicationUserWeeklyChallenge_Properties_InitializedCorrectly()
        {
            // Arrange
            var appUserWeeklyChallenge = new ApplicationUserWeeklyChallenge
            {
                UserId = "user123",
                WeeklyChallengeId = 1,
                IsCompleted = true,
                CompletedDate = DateTime.Now
            };

            // Act & Assert
            Assert.Equal("user123", appUserWeeklyChallenge.UserId);
            Assert.Equal(1, appUserWeeklyChallenge.WeeklyChallengeId);
            Assert.True(appUserWeeklyChallenge.IsCompleted);
            Assert.NotNull(appUserWeeklyChallenge.CompletedDate);
        }

        [Fact]
        public void ApplicationUserWeeklyChallenge_ShouldHaveUser()
        {
            // Arrange
            var appUserWeeklyChallenge = new ApplicationUserWeeklyChallenge
            {
                User = new ApplicationUser { Id = "user123" }
            };

            // Act & Assert
            Assert.NotNull(appUserWeeklyChallenge.User);
        }

        [Fact]
        public void ApplicationUserWeeklyChallenge_ShouldHaveWeeklyChallenge()
        {
            // Arrange
            var appUserWeeklyChallenge = new ApplicationUserWeeklyChallenge
            {
                WeeklyChallenge = new WeeklyChallenge { Id = 1, Title = "Weekly Mindfulness Challenge" }
            };

            // Act & Assert
            Assert.NotNull(appUserWeeklyChallenge.WeeklyChallenge);
        }
    }
}
