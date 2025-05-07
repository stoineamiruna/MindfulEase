using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class QuizTagUnitTest
    {
        [Fact]
        public void QuizTag_Properties_InitializedCorrectly()
        {
            // Arrange
            var quizTag = new QuizTag
            {
                TagId = 1,
                QuizId = 1
            };

            // Act & Assert
            Assert.Equal(1, quizTag.TagId);
            Assert.Equal(1, quizTag.QuizId);
        }
    }
}
