using System.Collections.Generic;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class EmotionUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            var emotion = new Emotion();
            Assert.NotNull(emotion);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            var emotion = new Emotion
            {
                Id = 1,
                Label = "Happiness",
                ColorCode = "#FFFF00",
                Diaries = new List<DiaryEmotion>(),
                Users = new List<ApplicationUserEmotion>(),
                BrainRegions = new List<EmotionBrainRegion>()
            };

            Assert.Equal(1, emotion.Id);
            Assert.Equal("Happiness", emotion.Label);
            Assert.Equal("#FFFF00", emotion.ColorCode);
            Assert.NotNull(emotion.Diaries);
            Assert.NotNull(emotion.Users);
            Assert.NotNull(emotion.BrainRegions);
        }

        [Fact]
        public void NullCollections_Allowed()
        {
            var emotion = new Emotion
            {
                Id = 2,
                Label = "Fear",
                ColorCode = null,
                Diaries = null,
                Users = null,
                BrainRegions = null
            };

            Assert.Equal(2, emotion.Id);
            Assert.Equal("Fear", emotion.Label);
            Assert.Null(emotion.ColorCode);
            Assert.Null(emotion.Diaries);
            Assert.Null(emotion.Users);
            Assert.Null(emotion.BrainRegions);
        }
    }
}
