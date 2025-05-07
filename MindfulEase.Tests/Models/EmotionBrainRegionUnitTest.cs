using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class EmotionBrainRegionUnitTest
    {
        [Fact]
        public void CanCreate_WithNullReferences()
        {
            var eb = new EmotionBrainRegion
            {
                EmotionId = null,
                BrainRegionId = null,
                Emotion = null,
                BrainRegion = null
            };

            Assert.Null(eb.EmotionId);
            Assert.Null(eb.BrainRegionId);
            Assert.Null(eb.Emotion);
            Assert.Null(eb.BrainRegion);
        }

        [Fact]
        public void CanAssign_ValidReferences()
        {
            var emotion = new Emotion { Id = 3, Label = "Anger" };
            var region = new BrainRegion { Id = 5, Name = "Amygdala" };

            var eb = new EmotionBrainRegion
            {
                EmotionId = 3,
                BrainRegionId = 5,
                Emotion = emotion,
                BrainRegion = region
            };

            Assert.Equal(3, eb.EmotionId);
            Assert.Equal(5, eb.BrainRegionId);
            Assert.Equal(emotion, eb.Emotion);
            Assert.Equal(region, eb.BrainRegion);
        }
    }
}
