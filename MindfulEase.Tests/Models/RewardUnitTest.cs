using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class RewardUnitTest
    {
        [Fact]
        public void Reward_Properties_InitializedCorrectly()
        {
            // Arrange
            var reward = new Reward
            {
                Id = 1,
                UserId = "user123",
                Activity = "Completed Quiz",
                Points = 100,
                DateEarned = DateTime.Now
            };

            // Act & Assert
            Assert.Equal(1, reward.Id);
            Assert.Equal("user123", reward.UserId);
            Assert.Equal("Completed Quiz", reward.Activity);
            Assert.Equal(100, reward.Points);
            Assert.True(reward.DateEarned <= DateTime.Now);  // Checking if the date is not in the future
        }
    }
}
