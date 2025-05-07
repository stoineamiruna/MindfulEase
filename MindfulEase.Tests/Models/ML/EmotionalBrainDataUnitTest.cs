using MindfulEase.Models.ML;
using Xunit;

namespace MindfulEase.Tests.Models.ML
{
    public class EmotionalBrainDataUnitTest
    {
        [Fact]
        public void EmotionalBrainData_Properties_InitializedCorrectly()
        {
            // Arrange
            var emotionalBrainData = new EmotionalBrainData
            {
                ParticipantID = 1,
                Age = 30,
                Sex = "Male",
                YearsSinceStart = 5,
                Joy = 8,
                Sadness = 4,
                Anger = 3,
                Love = 6,
                Fear = 7,
                Surprise = 2,
                Disgust = 1,
                VentromedialPrefrontalCortex = 0.5f,
                NucleusAccumbens = 0.6f,
                Amygdala = 0.7f,
                AnteriorCingulateCortex = 0.3f,
                Insula = 0.4f,
                Hypothalamus = 0.5f,
                DorsolateralPrefrontalCortex = 0.2f,
                OrbitofrontalCortex = 0.3f,
                Striatum = 0.4f,
                Hippocampus = 0.5f,
                SuperiorParietalCortex = 0.6f,
                BasalGanglia = 0.7f
            };

            // Act & Assert
            Assert.Equal(1, emotionalBrainData.ParticipantID);
            Assert.Equal(30, emotionalBrainData.Age);
            Assert.Equal("Male", emotionalBrainData.Sex);
            Assert.Equal(5, emotionalBrainData.YearsSinceStart);
        }
    }
}
