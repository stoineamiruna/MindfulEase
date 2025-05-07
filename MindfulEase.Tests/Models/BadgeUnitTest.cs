using System;
using System.Collections.Generic;
using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class BadgeUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            // Arrange & Act
            var badge = new Badge();

            // Assert
            Assert.NotNull(badge);
            Assert.IsType<Badge>(badge);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var badge = new Badge
            {
                Id = 1,
                Title = "Early Bird",
                Description = "Logged in before 7 AM",
                ImageUrl = "/images/badges/earlybird.png",
                Activity = "LoginBefore7AM",
                Users = new List<UserBadge>
                {
                    new UserBadge { Id = 1, UserId = "user1", BadgeId = 1 },
                    new UserBadge { Id = 2, UserId = "user2", BadgeId = 1 }
                }
            };

            // Act & Assert
            Assert.Equal(1, badge.Id);
            Assert.Equal("Early Bird", badge.Title);
            Assert.Equal("Logged in before 7 AM", badge.Description);
            Assert.Equal("/images/badges/earlybird.png", badge.ImageUrl);
            Assert.Equal("LoginBefore7AM", badge.Activity);
            Assert.NotNull(badge.Users);
            Assert.Equal(2, badge.Users.Count);
        }

        [Fact]
        public void NullUsersList_Allowed()
        {
            // Arrange
            var badge = new Badge
            {
                Id = 2,
                Title = "Night Owl",
                Description = "Logged in after midnight",
                ImageUrl = "/images/badges/nightowl.png",
                Activity = "LoginAfterMidnight",
                Users = null
            };

            // Act & Assert
            Assert.Equal(2, badge.Id);
            Assert.Equal("Night Owl", badge.Title);
            Assert.Equal("Logged in after midnight", badge.Description);
            Assert.Equal("/images/badges/nightowl.png", badge.ImageUrl);
            Assert.Equal("LoginAfterMidnight", badge.Activity);
            Assert.Null(badge.Users);
        }
    }
}
