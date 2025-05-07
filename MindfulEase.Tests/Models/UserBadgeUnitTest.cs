using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class UserBadgeUnitTest
    {
        [Fact]
        public void UserBadge_Properties_InitializedCorrectly()
        {
            // Arrange
            var userBadge = new UserBadge
            {
                Id = 1,
                UserId = "user123",
                BadgeId = 1,
                DateUnlocked = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, userBadge.Id);
            Assert.Equal("user123", userBadge.UserId);
            Assert.Equal(1, userBadge.BadgeId);
            Assert.True(userBadge.DateUnlocked <= DateTime.Now);  // Checking if the date is not in the future
        }

        [Fact]
        public void UserBadge_ShouldHaveBadgeAndUser()
        {
            // Arrange
            var userBadge = new UserBadge
            {
                UserId = "user123",
                BadgeId = 1,
                DateUnlocked = DateTime.Now,
                User = new ApplicationUser { Id = "user123" },
                Badge = new Badge { Id = 1, Title = "Achievement" }
            };

            // Act & Assert
            Assert.NotNull(userBadge.User);
            Assert.NotNull(userBadge.Badge);
        }
    }
}
