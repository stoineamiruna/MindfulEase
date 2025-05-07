using System;
using System.Collections.Generic;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class DiaryUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            var diary = new Diary();
            Assert.NotNull(diary);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            var diary = new Diary
            {
                Id = 1,
                UserId = "user123",
                EntryDate = new DateTime(2025, 5, 7),
                Content = "Today I felt calm and focused.",
                User = new ApplicationUser { Id = "user123", FirstName = "Ana" },
                Emotions = new List<DiaryEmotion>
                {
                    new DiaryEmotion { DiaryId = 1, EmotionId = 3 }
                }
            };

            Assert.Equal(1, diary.Id);
            Assert.Equal("user123", diary.UserId);
            Assert.Equal(new DateTime(2025, 5, 7), diary.EntryDate);
            Assert.Equal("Today I felt calm and focused.", diary.Content);
            Assert.NotNull(diary.User);
            Assert.Single(diary.Emotions);
        }

        [Fact]
        public void NullNavigationProperties_Allowed()
        {
            var diary = new Diary
            {
                Id = 2,
                UserId = null,
                Content = "No user linked.",
                EntryDate = DateTime.Now,
                User = null,
                Emotions = null
            };

            Assert.Equal(2, diary.Id);
            Assert.Null(diary.UserId);
            Assert.Null(diary.User);
            Assert.Null(diary.Emotions);
        }
    }
}
