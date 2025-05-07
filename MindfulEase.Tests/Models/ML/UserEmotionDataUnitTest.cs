using MindfulEase.Models.ML;
using Xunit;

namespace MindfulEase.Tests.Models.ML
{
    public class UserEmotionDataUnitTest
    {
        [Fact]
        public void UserEmotionData_Properties_InitializedCorrectly()
        {
            // Arrange
            var userEmotionData = new UserEmotionData
            {
                Date = DateTime.Now,
                EmotionLabel = "Joy",
                MoodValue = 8.5
            };

            // Act & Assert
            Assert.Equal("Joy", userEmotionData.EmotionLabel);
            Assert.Equal(8.5, userEmotionData.MoodValue);
        }
    }
}
