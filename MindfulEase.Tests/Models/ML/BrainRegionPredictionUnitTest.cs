using MindfulEase.Models.ML;
using Xunit;

namespace MindfulEase.Tests.Models.ML
{
    public class BrainRegionPredictionUnitTest
    {
        [Fact]
        public void BrainRegionPrediction_Properties_InitializedCorrectly()
        {
            // Arrange
            var brainRegionPrediction = new BrainRegionPrediction
            {
                VentromedialPrefrontalCortex = 0.5f,
                NucleusAccumbens = 0.3f,
                Amygdala = 0.8f,
                AnteriorCingulateCortex = 0.7f,
                Insula = 0.6f,
                Hypothalamus = 0.9f,
                DorsolateralPrefrontalCortex = 0.4f,
                OrbitofrontalCortex = 0.6f,
                Striatum = 0.2f,
                Hippocampus = 0.7f,
                SuperiorParietalCortex = 0.3f,
                BasalGanglia = 0.5f
            };

            // Act & Assert
            Assert.Equal(0.5f, brainRegionPrediction.VentromedialPrefrontalCortex);
            Assert.Equal(0.3f, brainRegionPrediction.NucleusAccumbens);
            Assert.Equal(0.8f, brainRegionPrediction.Amygdala);
            Assert.Equal(0.7f, brainRegionPrediction.AnteriorCingulateCortex);
            Assert.Equal(0.6f, brainRegionPrediction.Insula);
            Assert.Equal(0.9f, brainRegionPrediction.Hypothalamus);
        }
    }
}
