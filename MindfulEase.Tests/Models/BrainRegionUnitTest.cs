using System.Collections.Generic;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class BrainRegionUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            var region = new BrainRegion();
            Assert.NotNull(region);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            var region = new BrainRegion
            {
                Id = 1,
                Name = "Amygdala",
                Emotions = new List<EmotionBrainRegion>
                {
                    new EmotionBrainRegion { EmotionId = 2, BrainRegionId = 1 }
                }
            };

            Assert.Equal(1, region.Id);
            Assert.Equal("Amygdala", region.Name);
            Assert.Single(region.Emotions);
        }

        [Fact]
        public void NullEmotions_Allowed()
        {
            var region = new BrainRegion
            {
                Id = 2,
                Name = "Hippocampus",
                Emotions = null
            };

            Assert.Equal(2, region.Id);
            Assert.Equal("Hippocampus", region.Name);
            Assert.Null(region.Emotions);
        }
    }
}
