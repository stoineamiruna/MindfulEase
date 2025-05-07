using Microsoft.AspNetCore.Mvc.Rendering;
using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserUnitTest
    {
        [Fact]
        public void Constructor_Test()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            Assert.NotNull(user);
            Assert.IsType<ApplicationUser>(user);
        }

        [Fact]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var user = new ApplicationUser
            {
                FirstName = "Ana",
                LastName = "Popescu",
                Email = "ana@example.com",
                EmailAddress = "ana@example.com",
                Birthday = new DateTime(1995, 5, 10),
                Description = "Therapist specialized in CBT",
                Age = 29,
                Sex = "Female",
                IsTherapist = true,
                ProfilePicture = "https://example.com/profile.jpg",
                Studies = "University of Psychology",
                PhoneNumber = "+40123456789",
                BackgroundColor = "#FFFFFF",
                Rating = 4.8,
                NumberOfReviews = 25,
                ClusterId = 3,
                Diaries = new List<Diary>(),
                WeeklyReports = new List<WeeklyReport>(),
                Rewards = new List<Reward>(),
                TherapeuticGames = new List<ApplicationUserTherapeuticGame>(),
                Quizzes = new List<ApplicationUserQuiz>(),
                UserQuestionQuizzes = new List<ApplicationUserQuestionQuiz>(),
                Resources = new List<ApplicationUserResource>(),
                Objectives = new List<UserObjective>(),
                Badges = new List<UserBadge>(),
                WeeklyChallenges = new List<ApplicationUserWeeklyChallenge>(),
                Notifications = new List<Notification>(),
                Emotions = new List<ApplicationUserEmotion>(),
                SavedResources = new List<SavedResource>(),
                AllRoles = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Admin", Value = "admin" },
                    new SelectListItem { Text = "User", Value = "user" }
                }
            };

            // Act & Assert
            Assert.Equal("Ana", user.FirstName);
            Assert.Equal("Popescu", user.LastName);
            Assert.Equal("ana@example.com", user.Email);
            Assert.Equal("ana@example.com", user.EmailAddress);
            Assert.Equal(new DateTime(1995, 5, 10), user.Birthday);
            Assert.Equal("Therapist specialized in CBT", user.Description);
            Assert.Equal(29, user.Age);
            Assert.Equal("Female", user.Sex);
            Assert.True(user.IsTherapist);
            Assert.Equal("https://example.com/profile.jpg", user.ProfilePicture);
            Assert.Equal("University of Psychology", user.Studies);
            Assert.Equal("+40123456789", user.PhoneNumber);
            Assert.Equal("#FFFFFF", user.BackgroundColor);
            Assert.Equal(4.8, user.Rating);
            Assert.Equal(25, user.NumberOfReviews);
            Assert.Equal(3, user.ClusterId);

            Assert.NotNull(user.Diaries);
            Assert.NotNull(user.WeeklyReports);
            Assert.NotNull(user.Rewards);
            Assert.NotNull(user.TherapeuticGames);
            Assert.NotNull(user.Quizzes);
            Assert.NotNull(user.UserQuestionQuizzes);
            Assert.NotNull(user.Resources);
            Assert.NotNull(user.Objectives);
            Assert.NotNull(user.Badges);
            Assert.NotNull(user.WeeklyChallenges);
            Assert.NotNull(user.Notifications);
            Assert.NotNull(user.Emotions);
            Assert.NotNull(user.SavedResources);
            Assert.NotNull(user.AllRoles);
        }

        [Fact]
        public void NullProperties_Allowed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                FirstName = null,
                LastName = null,
                EmailAddress = null,
                Birthday = null,
                Description = null,
                Age = null,
                Sex = null,
                IsTherapist = null,
                ProfilePicture = null,
                Studies = null,
                PhoneNumber = null,
                BackgroundColor = null,
                Rating = null,
                NumberOfReviews = null,
                ClusterId = null,
                Diaries = null,
                WeeklyReports = null,
                Rewards = null,
                TherapeuticGames = null,
                Quizzes = null,
                UserQuestionQuizzes = null,
                Resources = null,
                Objectives = null,
                Badges = null,
                WeeklyChallenges = null,
                Notifications = null,
                Emotions = null,
                SavedResources = null,
                AllRoles = null
            };

            // Act & Assert
            Assert.Null(user.FirstName);
            Assert.Null(user.LastName);
            Assert.Null(user.EmailAddress);
            Assert.Null(user.Birthday);
            Assert.Null(user.Description);
            Assert.Null(user.Age);
            Assert.Null(user.Sex);
            Assert.Null(user.IsTherapist);
            Assert.Null(user.ProfilePicture);
            Assert.Null(user.Studies);
            Assert.Null(user.PhoneNumber);
            Assert.Null(user.BackgroundColor);
            Assert.Null(user.Rating);
            Assert.Null(user.NumberOfReviews);
            Assert.Null(user.ClusterId);
            Assert.Null(user.Diaries);
            Assert.Null(user.AllRoles);
        }
    }
}
