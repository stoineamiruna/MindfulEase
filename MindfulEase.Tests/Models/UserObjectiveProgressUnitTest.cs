using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class UserObjectiveProgressUnitTest
    {
        [Fact]
        public void UserObjectiveProgress_Properties_InitializedCorrectly()
        {
            // Arrange
            var userObjectiveProgress = new UserObjectiveProgress
            {
                Id = 1,
                UserObjectiveId = 1,
                Date = DateTime.Now,
                IsCompleted = true
            };

            // Act & Assert
            Assert.Equal(1, userObjectiveProgress.Id);
            Assert.Equal(1, userObjectiveProgress.UserObjectiveId);
            Assert.Equal(DateTime.Now.Date, userObjectiveProgress.Date.Date);
            Assert.True(userObjectiveProgress.IsCompleted);
        }

        [Fact]
        public void UserObjectiveProgress_ShouldHaveUserObjective()
        {
            // Arrange
            var userObjectiveProgress = new UserObjectiveProgress
            {
                UserObjective = new UserObjective { Id = 1, UserId = "user123" }
            };

            // Act & Assert
            Assert.NotNull(userObjectiveProgress.UserObjective);
        }
    }
}
