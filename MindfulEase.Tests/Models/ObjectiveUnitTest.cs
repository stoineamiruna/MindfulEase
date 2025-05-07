using MindfulEase.Models;
using Xunit;

namespace MindfulEase.Tests.Models
{
    public class ObjectiveUnitTest
    {
        [Fact]
        public void CanCreate_ObjectiveWithValidData()
        {
            var obj = new Objective
            {
                Id = 1,
                Title = "Meditation",
                Category = "Wellness",
                ValueType = "Time"
            };

            Assert.Equal(1, obj.Id);
            Assert.Equal("Meditation", obj.Title);
            Assert.Equal("Wellness", obj.Category);
            Assert.Equal("Time", obj.ValueType);
        }

        [Fact]
        public void CanAssign_UserObjectives()
        {
            var obj = new Objective
            {
                Users = new List<UserObjective>
                {
                    new UserObjective { ObjectiveId = 1, UserId = "abc" }
                }
            };

            Assert.Single(obj.Users);
        }
    }
}
