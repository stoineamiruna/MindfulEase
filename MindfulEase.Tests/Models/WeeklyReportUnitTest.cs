using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class WeeklyReportUnitTest
    {
        [Fact]
        public void WeeklyReport_Properties_InitializedCorrectly()
        {
            // Arrange
            var weeklyReport = new WeeklyReport
            {
                Id = 1,
                UserId = "user123",
                WeekStartDate = DateTime.Now.AddDays(-7),
                NrWeeklyChallenges = 3,
                NrObjectives = 5,
                NrDiaries = 4,
                AverageEmotions = "2.3;0;2.1"
            };

            // Act & Assert
            Assert.Equal(1, weeklyReport.Id);
            Assert.Equal("user123", weeklyReport.UserId);
            Assert.True(weeklyReport.WeekStartDate <= DateTime.Now);
            Assert.Equal(3, weeklyReport.NrWeeklyChallenges);
            Assert.Equal(5, weeklyReport.NrObjectives);
            Assert.Equal(4, weeklyReport.NrDiaries);
            Assert.Equal("2.3;0;2.1", weeklyReport.AverageEmotions);
        }

        [Fact]
        public void WeeklyReport_ShouldHaveUser()
        {
            // Arrange
            var weeklyReport = new WeeklyReport
            {
                User = new ApplicationUser { Id = "user123" }
            };

            // Act & Assert
            Assert.NotNull(weeklyReport.User);
        }
    }
}
