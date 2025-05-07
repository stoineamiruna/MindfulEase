using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class EmotionClusterPredictionUnitTest
    {
        [Fact]
        public void EmotionClusterPrediction_Properties_InitializedCorrectly()
        {
            // Arrange
            var emotionClusterPrediction = new EmotionClusterPrediction
            {
                ClusterId = 2,  // Asigură-te că folosești un uint pentru ClusterId
                Distances = new float[] { 0.5f, 0.3f, 0.7f }
            };

            // Act & Assert
            Assert.Equal(2u, emotionClusterPrediction.ClusterId);  // Folosește 2u pentru a specifica un uint
            Assert.Equal(3, emotionClusterPrediction.Distances.Length);
            Assert.Equal(0.5f, emotionClusterPrediction.Distances[0]);
            Assert.Equal(0.3f, emotionClusterPrediction.Distances[1]);
            Assert.Equal(0.7f, emotionClusterPrediction.Distances[2]);
        }

        [Fact]
        public void EmotionClusterPrediction_ShouldHaveValidDistances()
        {
            // Arrange
            var emotionClusterPrediction = new EmotionClusterPrediction
            {
                ClusterId = 1u,  // Folosește uint pentru ClusterId
                Distances = new float[] { 0.1f, 0.2f, 0.3f }
            };

            // Act & Assert
            Assert.NotNull(emotionClusterPrediction.Distances);
            Assert.All(emotionClusterPrediction.Distances, distance => Assert.InRange(distance, 0f, 1f));
        }
    }
}
