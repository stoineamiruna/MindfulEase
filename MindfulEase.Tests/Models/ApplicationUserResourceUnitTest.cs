using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserResourceUnitTest
    {
        [Fact]
        public void ApplicationUserResource_Properties_InitializedCorrectly()
        {
            // Arrange
            var appUserResource = new ApplicationUserResource
            {
                UserId = "user123",
                ResourceId = 1,
                IsLiked = true
            };

            // Act & Assert
            Assert.Equal("user123", appUserResource.UserId);
            Assert.Equal(1, appUserResource.ResourceId);
            Assert.True(appUserResource.IsLiked);
        }

        [Fact]
        public void ApplicationUserResource_ShouldHaveUser()
        {
            // Arrange
            var appUserResource = new ApplicationUserResource
            {
                User = new ApplicationUser { Id = "user123" }
            };

            // Act & Assert
            Assert.NotNull(appUserResource.User);
        }

        [Fact]
        public void ApplicationUserResource_ShouldHaveResource()
        {
            // Arrange
            var appUserResource = new ApplicationUserResource
            {
                Resource = new Resource { Id = 1, Title = "Mindfulness Article" }
            };

            // Act & Assert
            Assert.NotNull(appUserResource.Resource);
        }
    }
}
