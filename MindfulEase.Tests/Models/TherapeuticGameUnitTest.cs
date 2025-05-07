using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class TherapeuticGameUnitTest
    {
        [Fact]
        public void TherapeuticGame_Properties_InitializedCorrectly()
        {
            // Arrange
            var therapeuticGame = new TherapeuticGame
            {
                Id = 1,
                Name = "Memory Game",
                Type = "Memory Game",
                Instructions = "Match the pairs.",
                GameUrl = "http://example.com",
                Background = "background_image_url"
            };

            // Act & Assert
            Assert.Equal(1, therapeuticGame.Id);
            Assert.Equal("Memory Game", therapeuticGame.Name);
            Assert.Equal("Memory Game", therapeuticGame.Type);
            Assert.Equal("Match the pairs.", therapeuticGame.Instructions);
            Assert.Equal("http://example.com", therapeuticGame.GameUrl);
            Assert.Equal("background_image_url", therapeuticGame.Background);
        }

        [Fact]
        public void TherapeuticGame_ShouldHaveTagsAndUsers()
        {
            // Arrange
            var therapeuticGame = new TherapeuticGame
            {
                Id = 1,
                Name = "Puzzle Game",
                Users = new List<ApplicationUserTherapeuticGame>
                {
                    new ApplicationUserTherapeuticGame { UserId = "user123" }
                },
                Tags = new List<TherapeuticGameTag>
                {
                    new TherapeuticGameTag { TagId = 1 }
                }
            };

            // Act & Assert
            Assert.NotNull(therapeuticGame.Users);
            Assert.Single(therapeuticGame.Users);

            Assert.NotNull(therapeuticGame.Tags);
            Assert.Single(therapeuticGame.Tags);
        }
    }
}
