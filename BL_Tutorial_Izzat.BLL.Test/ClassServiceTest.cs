using BL_Tutorial_Izzat.DAL.Models;
using Moq;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BL_Tutorial_Izzat.BLL.Test
{
    public class ClassServiceTest
    {
        public class GetClassById
        {
            [Theory]
            [InlineData("1")]
            [InlineData("3")]
            public async Task GetDataById_ResultFound(string id)
            {
                // arrange
                var repo = new Mock<IDocumentDBRepository<DTOClass>>();

                IEnumerable<DTOClass> classes = new List<DTOClass>
                {
                    {new DTOClass() { Id = "1", Description = "abcd"} },
                    {new DTOClass() { Id = "2", Description = "xyz0"} }
                };

                var classData = classes.Where(o => o.Id == id).FirstOrDefault();

                repo.Setup(c => c.GetByIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>()
                )).Returns(
                    Task.FromResult<DTOClass>(classData)
                );

                var svc = new ClassService(repo.Object);

                // act
                var act = await svc.GetClassById("", null);

                // assert
                Assert.Equal(classData, act);

            }
        }

        // TODO: crud yang lain jg d test ya
    }
}
