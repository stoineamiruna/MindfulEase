using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ErrorViewModelUnitTest
    {
        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsTrue_WhenRequestIdIsNotNullOrEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = "12345"
            };

            // Act & Assert
            Assert.True(errorViewModel.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = null
            };

            // Act & Assert
            Assert.False(errorViewModel.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = string.Empty
            };

            // Act & Assert
            Assert.False(errorViewModel.ShowRequestId);
        }
    }
}
