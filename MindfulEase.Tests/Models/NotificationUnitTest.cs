using System;
using System.ComponentModel.DataAnnotations;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class NotificationUnitTest
    {
        [Fact]
        public void CanCreate_WithRequiredFields()
        {
            var notification = new Notification
            {
                Id = 1,
                Message = "You have a new badge!",
                Link = "/badges/1"
            };

            Assert.Equal(1, notification.Id);
            Assert.Equal("You have a new badge!", notification.Message);
            Assert.Equal("/badges/1", notification.Link);
            Assert.False(notification.IsRead);
            Assert.True((DateTime.Now - notification.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void CanAssign_UserAndMarkAsRead()
        {
            var user = new ApplicationUser { Id = "abc123", FirstName = "Alex" };

            var notification = new Notification
            {
                UserId = "abc123",
                User = user,
                Message = "Reminder: Mood entry pending.",
                Link = "/diary",
                IsRead = true
            };

            Assert.Equal("abc123", notification.UserId);
            Assert.Equal(user, notification.User);
            Assert.True(notification.IsRead);
        }

        [Fact]
        public void RequiredMessage_Validation()
        {
            var notification = new Notification();
            var context = new ValidationContext(notification);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(notification, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Message"));
        }
    }
}
