using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class RoutineUnitTest
    {
        [Fact]
        public void Routine_Properties_InitializedCorrectly()
        {
            // Arrange
            var routine = new Routine
            {
                Id = 1,
                ClusterId = 1,
                RoutineDescription = "Morning routine"
            };

            // Act & Assert
            Assert.Equal(1, routine.Id);
            Assert.Equal(1, routine.ClusterId);
            Assert.Equal("Morning routine", routine.RoutineDescription);
        }

        [Fact]
        public void Routine_ShouldHaveResources()
        {
            // Arrange
            var routine = new Routine
            {
                Id = 1,
                ClusterId = 1,
                RoutineDescription = "Evening routine",
                Resources = new List<Resource> { new Resource { Id = 1, Title = "Meditation Guide" } }
            };

            // Act & Assert
            Assert.NotNull(routine.Resources);
            Assert.Single(routine.Resources);
        }
    }
}
