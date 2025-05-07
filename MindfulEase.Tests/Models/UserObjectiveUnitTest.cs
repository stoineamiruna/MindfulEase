using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class UserObjectiveUnitTest
    {
        [Fact]
        public void UserObjective_Properties_InitializedCorrectly()
        {
            // Arrange
            var userObjective = new UserObjective
            {
                Id = 1,
                UserId = "user123",
                ObjectiveId = 1,
                TargetValue = 50,
                TargetTime = TimeSpan.FromDays(30)
            };

            // Act & Assert
            Assert.Equal(1, userObjective.Id);
            Assert.Equal("user123", userObjective.UserId);
            Assert.Equal(1, userObjective.ObjectiveId);
            Assert.Equal(50, userObjective.TargetValue);
            Assert.Equal(TimeSpan.FromDays(30), userObjective.TargetTime);
        }

        [Fact]
        public void UserObjective_ShouldHaveProgress()
        {
            // Arrange
            var userObjective = new UserObjective
            {
                Id = 1,
                UserId = "user123",
                UserObjectiveProgresses = new List<UserObjectiveProgress>
                {
                    new UserObjectiveProgress { Id = 1, IsCompleted = true, Date = DateTime.Now }
                }
            };

            // Act & Assert
            Assert.NotNull(userObjective.UserObjectiveProgresses);
            Assert.Single(userObjective.UserObjectiveProgresses);
        }
    }
}
