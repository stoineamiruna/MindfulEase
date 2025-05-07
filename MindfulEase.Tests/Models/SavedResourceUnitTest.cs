using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class SavedResourceUnitTest
    {
        [Fact]
        public void SavedResource_Properties_InitializedCorrectly()
        {
            // Arrange
            var savedResource = new SavedResource
            {
                UserId = "user123",
                ResourceId = 1,
                Date = DateTime.Now,
                IsSaved = true
            };

            // Act & Assert
            Assert.Equal("user123", savedResource.UserId);
            Assert.Equal(1, savedResource.ResourceId);
            Assert.True(savedResource.Date <= DateTime.Now);  // Checking if the date is not in the future
            Assert.True(savedResource.IsSaved == true);
        }

        [Fact]
        public void SavedResource_ShouldHaveUserAndResource()
        {
            // Arrange
            var savedResource = new SavedResource
            {
                UserId = "user123",
                ResourceId = 1,
                Date = DateTime.Now,
                IsSaved = true,
                User = new ApplicationUser { Id = "user123" },
                Resource = new Resource { Id = 1, Title = "Resource 1" }
            };

            // Act & Assert
            Assert.NotNull(savedResource.User);
            Assert.NotNull(savedResource.Resource);
        }
    }
}
