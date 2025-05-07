using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class ApplicationUserQuestionQuizUnitTest
    {
        [Fact]
        public void CanCreate_UserResponseToQuestion()
        {
            var response = new ApplicationUserQuestionQuiz
            {
                Id = 1,
                UserId = "abc123",
                QuestionId = 5,
                ResponseValue = 4
            };

            Assert.Equal("abc123", response.UserId);
            Assert.Equal(5, response.QuestionId);
            Assert.Equal(4, response.ResponseValue);
        }
    }
}
