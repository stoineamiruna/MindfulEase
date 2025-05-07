using MindfulEase.Models.ML;
using Xunit;

namespace MindfulEase.Tests.Models.ML
{
    public class EmotionalInputDataUnitTest
    {
        [Fact]
        public void EmotionalInputData_Properties_InitializedCorrectly()
        {
            // Arrange
            var emotionalInputData = new EmotionalInputData
            {
                Age = 30,
                Sex = "Female",
                Joy = 7,
                Sadness = 4,
                Anger = 3,
                Love = 8,
                Fear = 5,
                Surprise = 2,
                Disgust = 1,
                YearsSinceStart = 6
            };

            // Act & Assert
            Assert.Equal(30, emotionalInputData.Age);
            Assert.Equal("Female", emotionalInputData.Sex);
            Assert.Equal(7, emotionalInputData.Joy);
            Assert.Equal(4, emotionalInputData.Sadness);
        }
    }
}
