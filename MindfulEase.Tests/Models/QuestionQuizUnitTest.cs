using MindfulEase.Models;
using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Tests.Models
{
    public class QuestionQuizUnitTest
    {
        [Fact]
        public void CanCreate_QuestionWithData()
        {
            var question = new QuestionQuiz
            {
                Id = 10,
                Text = "I feel tense often.",
                IsReversed = false,
                Order = 2
            };

            Assert.Equal("I feel tense often.", question.Text);
            Assert.False(question.IsReversed);
        }

        [Fact]
        public void CanAssign_UserQuestionQuizzes()
        {
            var question = new QuestionQuiz
            {
                UserQuestionQuizzes = new List<ApplicationUserQuestionQuiz>
                {
                    new ApplicationUserQuestionQuiz { ResponseValue = 3, UserId = "user1" }
                }
            };

            Assert.Single(question.UserQuestionQuizzes);
        }
    }
}
