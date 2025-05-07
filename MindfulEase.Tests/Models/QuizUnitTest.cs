using MindfulEase.Models;
using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Tests.Models
{
    public class QuizUnitTest
    {
        [Fact]
        public void CanCreate_QuizWithData()
        {
            var quiz = new Quiz
            {
                Id = 1,
                Title = "Anxiety Test",
                Description = "Evaluate anxiety level.",
                Result = "Mild",
                CategoryMapping = "{\"Anxiety\":\"1,2,3\"}",
                Background = "blue.jpg"
            };

            Assert.Equal("Anxiety Test", quiz.Title);
            Assert.Equal("Mild", quiz.Result);
        }

        [Fact]
        public void CanAssign_QuestionsAndTags()
        {
            var quiz = new Quiz
            {
                Questions = new List<QuestionQuiz> { new QuestionQuiz { Text = "Do you worry?", Order = 1 } },
                Tags = new List<QuizTag> { new QuizTag { Tag = new Tag { Title = "Anxiety" } } }
            };

            Assert.Single(quiz.Questions);
            Assert.Single(quiz.Tags);
        }
    }
}
