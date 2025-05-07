using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class TherapeuticGameTagUnitTest
    {
        [Fact]
        public void TherapeuticGameTag_Properties_InitializedCorrectly()
        {
            // Arrange
            var therapeuticGameTag = new TherapeuticGameTag
            {
                TagId = 1,
                TherapeuticGameId = 1
            };

            // Act & Assert
            Assert.Equal(1, therapeuticGameTag.TagId);
            Assert.Equal(1, therapeuticGameTag.TherapeuticGameId);
        }
    }
}
