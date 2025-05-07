using System;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserEmotionUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            // Arrange & Act
            var emotionEntry = new ApplicationUserEmotion();

            // Assert
            Assert.NotNull(emotionEntry);
            Assert.IsType<ApplicationUserEmotion>(emotionEntry);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user123",
                FirstName = "Ion",
                LastName = "Pop"
            };

            var emotion = new Emotion
            {
                Id = 5,
                Label = "Joy",
                ColorCode = "yellow"
            };

            var entry = new ApplicationUserEmotion
            {
                Id = 1,
                UserId = user.Id,
                User = user,
                EmotionId = emotion.Id,
                Emotion = emotion,
                Date = new DateTime(2025, 5, 7),
                MoodValue = 8
            };

            // Act & Assert
            Assert.Equal(1, entry.Id);
            Assert.Equal("user123", entry.UserId);
            Assert.Equal(user, entry.User);
            Assert.Equal(5, entry.EmotionId);
            Assert.Equal(emotion, entry.Emotion);
            Assert.Equal(new DateTime(2025, 5, 7), entry.Date);
            Assert.Equal(8, entry.MoodValue);
        }

        [Fact]
        public void NullProperties_Allowed()
        {
            // Arrange
            var entry = new ApplicationUserEmotion
            {
                Id = 2,
                UserId = null,
                User = null,
                EmotionId = null,
                Emotion = null,
                Date = DateTime.MinValue,
                MoodValue = 0
            };

            // Act & Assert
            Assert.Equal(2, entry.Id);
            Assert.Null(entry.UserId);
            Assert.Null(entry.User);
            Assert.Null(entry.EmotionId);
            Assert.Null(entry.Emotion);
            Assert.Equal(DateTime.MinValue, entry.Date);
            Assert.Equal(0, entry.MoodValue);
        }
    }
}
