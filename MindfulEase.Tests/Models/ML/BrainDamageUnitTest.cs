using MindfulEase.Models.ML;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MindfulEase.Tests.Models.ML
{
    public class BrainDamageUnitTest
    {
        [Fact]
        public void BrainDamage_Properties_InitializedCorrectly()
        {
            // Arrange
            var brainDamage = new BrainDamage
            {
                Age = 30,
                Sex = "Male",
                Joy = 5,
                Sadness = 3,
                Anger = 2,
                Love = 4,
                Fear = 6,
                Surprise = 3,
                Disgust = 2,
                YearsSinceStart = 5,
                PredictedDamage = new Dictionary<string, float>(),
                Emotions = new List<EmotionData>(),
                BrainRegions = new List<BrainRegionData>(),
                EmotionBrainRegions = new List<EmotionBrainRegionData>()
            };

            // Act & Assert
            Assert.Equal(30, brainDamage.Age);
            Assert.Equal("Male", brainDamage.Sex);
            Assert.Equal(5, brainDamage.Joy);
            Assert.Equal(3, brainDamage.Sadness);
            Assert.Equal(2, brainDamage.Anger);
            Assert.Equal(4, brainDamage.Love);
            Assert.Equal(6, brainDamage.Fear);
            Assert.Equal(3, brainDamage.Surprise);
            Assert.Equal(2, brainDamage.Disgust);
            Assert.Equal(5, brainDamage.YearsSinceStart);
        }

        [Fact]
        public void BrainDamage_Validation_ThrowsErrorForInvalidAge()
        {
            // Arrange
            var brainDamage = new BrainDamage
            {
                Age = 120, // Invalid Age
                Sex = "Male",
                Joy = 5,
                Sadness = 3,
                Anger = 2,
                Love = 4,
                Fear = 6,
                Surprise = 3,
                Disgust = 2,
                YearsSinceStart = 5
            };

            // Act & Assert
            var validationContext = new ValidationContext(brainDamage, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(brainDamage, validationContext, validationResults, true);
            Assert.False(isValid);  // Validarea trebuie să eșueze pentru vârsta invalidă
        }
    }
}
