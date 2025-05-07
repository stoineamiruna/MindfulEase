using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserTherapeuticGameUnitTest
    {
        [Fact]
        public void ApplicationUserTherapeuticGame_Properties_InitializedCorrectly()
        {
            // Arrange
            var appUserTherapeuticGame = new ApplicationUserTherapeuticGame
            {
                UserId = "user123",
                GameId = 1
            };

            // Act & Assert
            Assert.Equal("user123", appUserTherapeuticGame.UserId);
            Assert.Equal(1, appUserTherapeuticGame.GameId);
        }

        [Fact]
        public void ApplicationUserTherapeuticGame_ShouldHaveUser()
        {
            // Arrange
            var appUserTherapeuticGame = new ApplicationUserTherapeuticGame
            {
                User = new ApplicationUser { Id = "user123" }
            };

            // Act & Assert
            Assert.NotNull(appUserTherapeuticGame.User);
        }

        [Fact]
        public void ApplicationUserTherapeuticGame_ShouldHaveGame()
        {
            // Arrange
            var appUserTherapeuticGame = new ApplicationUserTherapeuticGame
            {
                Game = new TherapeuticGame { Id = 1, Name = "Memory Puzzle" }
            };

            // Act & Assert
            Assert.NotNull(appUserTherapeuticGame.Game);
        }
    }
}
