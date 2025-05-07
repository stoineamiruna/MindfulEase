using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class DiaryEmotionUnitTest
    {
        [Fact]
        public void CanCreate_WithNullValues()
        {
            var de = new DiaryEmotion
            {
                DiaryId = null,
                EmotionId = null,
                Diary = null,
                Emotion = null,
                Score = null
            };

            Assert.Null(de.DiaryId);
            Assert.Null(de.EmotionId);
            Assert.Null(de.Diary);
            Assert.Null(de.Emotion);
            Assert.Null(de.Score);
        }

        [Fact]
        public void CanAssign_ValidValues()
        {
            var diary = new Diary { Id = 1, Content = "Example" };
            var emotion = new Emotion { Id = 2, Label = "Joy" };

            var de = new DiaryEmotion
            {
                DiaryId = 1,
                EmotionId = 2,
                Diary = diary,
                Emotion = emotion,
                Score = 0.8
            };

            Assert.Equal(1, de.DiaryId);
            Assert.Equal(2, de.EmotionId);
            Assert.Equal(diary, de.Diary);
            Assert.Equal(emotion, de.Emotion);
            Assert.Equal(0.8, de.Score);
        }
    }
}
