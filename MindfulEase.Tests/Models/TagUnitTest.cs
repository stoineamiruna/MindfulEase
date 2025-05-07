using MindfulEase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindfulEase.Tests.Models
{
    public class TagUnitTest
    {
        [Fact]
        public void CanCreate_TagWithTitle()
        {
            var tag = new Tag { Id = 1, Title = "Relaxation" };

            Assert.Equal(1, tag.Id);
            Assert.Equal("Relaxation", tag.Title);
        }

        [Fact]
        public void CanAssign_MultipleRelations()
        {
            var tag = new Tag
            {
                Quizzes = new List<QuizTag> { new QuizTag { QuizId = 1 } },
                Resources = new List<ResourceTag> { new ResourceTag { ResourceId = 2 } },
                TherapeuticGames = new List<TherapeuticGameTag> { new TherapeuticGameTag { TherapeuticGameId = 3 } }
            };

            Assert.Single(tag.Quizzes);
            Assert.Single(tag.Resources);
            Assert.Single(tag.TherapeuticGames);
        }
    }
}
