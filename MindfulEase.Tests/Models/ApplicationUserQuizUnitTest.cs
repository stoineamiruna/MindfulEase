using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserQuizUnitTest
    {
        [Fact]
        public void ApplicationUserQuiz_Properties_InitializedCorrectly()
        {
            // Arrange
            var appUserQuiz = new ApplicationUserQuiz
            {
                UserId = "user123",
                QuizId = 1,
                TotalScore = 85,
                IsCompleted = true,
                Date = DateTime.Now
            };

            // Act & Assert
            Assert.Equal("user123", appUserQuiz.UserId);
            Assert.Equal(1, appUserQuiz.QuizId);
            Assert.Equal(85, appUserQuiz.TotalScore);
            Assert.True(appUserQuiz.IsCompleted);
            Assert.NotNull(appUserQuiz.Date);
        }

        [Fact]
        public void ApplicationUserQuiz_ShouldHaveUser()
        {
            // Arrange
            var appUserQuiz = new ApplicationUserQuiz
            {
                User = new ApplicationUser { Id = "user123" }
            };

            // Act & Assert
            Assert.NotNull(appUserQuiz.User);
        }

        [Fact]
        public void ApplicationUserQuiz_ShouldHaveQuiz()
        {
            // Arrange
            var appUserQuiz = new ApplicationUserQuiz
            {
                Quiz = new Quiz { Id = 1, Title = "Anxiety Test" }
            };

            // Act & Assert
            Assert.NotNull(appUserQuiz.Quiz);
        }
    }
}
