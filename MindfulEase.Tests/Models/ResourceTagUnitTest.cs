using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class ResourceTagUnitTest
    {
        [Fact]
        public void ResourceTag_Properties_InitializedCorrectly()
        {
            // Arrange
            var resourceTag = new ResourceTag
            {
                TagId = 1,
                ResourceId = 1
            };

            // Act & Assert
            Assert.Equal(1, resourceTag.TagId);
            Assert.Equal(1, resourceTag.ResourceId);
        }
    }
}
