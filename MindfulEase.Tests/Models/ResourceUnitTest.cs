using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class ResourceUnitTest
    {
        [Fact]
        public void Resource_Properties_InitializedCorrectly()
        {
            // Arrange
            var resource = new Resource
            {
                Id = 1,
                Title = "Sample Resource",
                Link = "https://example.com",
                ResourceType = "Podcast"
            };

            // Act & Assert
            Assert.Equal(1, resource.Id);
            Assert.Equal("Sample Resource", resource.Title);
            Assert.Equal("https://example.com", resource.Link);
            Assert.Equal("Podcast", resource.ResourceType);
        }
    }
}
