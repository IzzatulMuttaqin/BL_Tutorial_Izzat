using BL_Tutorial_Izzat.DAL.Models;
using BL_Tutorial_Izzat.BLL;
using Moq;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BL_Tutorial_Izzat.BLL.Test
{
    public class FetchServiceTest
    {
        public class GetServiceTest
        {
            [Theory]
            [InlineData("1")]
            [InlineData("3")]
            public async Task GetDataById(string id)
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

                var svc = new FetchService(repo.Object);

                // act
                var act = await svc.GetDtoClassById("");

                // assert
                Assert.Equal(classData, act);
            }
        }

        public class CreateServiceTest
        {
            [Fact]
            public async Task CreateData()
            {
                var repo = new Mock<IDocumentDBRepository<DTOClass>>();

                var data = new DTOClass
                {
                    Id = "1",
                    ClassCode = "codes-xx",
                    Description = "desc"
                };

                repo.Setup(c => c.CreateAsync(
                    It.IsAny<DTOClass>(),
                    It.IsAny<EventGridOptions>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )).Returns(Task.FromResult(data));

                var svc = new FetchService(repo.Object);

                var act = await svc.CreateNewDtoClass(data);
                Assert.Equal(data.Id, act.Id);

            }
        }

        public class UpdateServiceTest
        {
            [Theory]
            [InlineData("1")]
            public async Task UpdateLesson_Updated(string id)
            {
                var repo = new Mock<IDocumentDBRepository<DTOClass>>();

                var data = new DTOClass
                {
                    Id = "1",
                    ClassCode = "codes-xx",
                    Description = "desc"
                };

                repo.Setup(c => c.UpdateAsync(
                    It.IsAny<string>(),
                    It.IsAny<DTOClass>(),
                    It.IsAny<EventGridOptions>(),
                    It.IsAny<string>()
                )).Returns(Task.FromResult(data));

                var svc = new FetchService(repo.Object);

                var act = await svc.UpdateDtoClas(id, data);
                Assert.Equal(data.ClassCode, act.ClassCode);

            }
        }
    }
}
