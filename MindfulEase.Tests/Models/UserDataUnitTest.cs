using MindfulEase.Models;
using Microsoft.ML.Data;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class UserDataUnitTest
    {
        [Fact]
        public void UserData_Properties_InitializedCorrectly()
        {
            // Arrange
            var userData = new UserData
            {
                UserId = "user123",
                Features = new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f }
            };

            // Act & Assert
            Assert.Equal("user123", userData.UserId);
            Assert.Equal(7, userData.Features.Length);
            Assert.Equal(1.0f, userData.Features[0]);
            Assert.Equal(7.0f, userData.Features[6]);
        }
    }
}
