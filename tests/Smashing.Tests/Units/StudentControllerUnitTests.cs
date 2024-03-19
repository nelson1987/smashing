using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Smashing.Tests.Units
{
    public class StudentControllerUnitTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        private readonly StudentController _controller;
        private readonly CancellationToken token = CancellationToken.None;
        private readonly List<Student> _students;
        public StudentControllerUnitTests()
        {
            _students = _fixture.Build<Student>()
                .CreateMany(5)
                .ToList();

            _controller = _fixture.Build<StudentController>()
                .OmitAutoProperties()
                .Create();
        }

        [Fact]
        public async Task Dado_Request_Valido_Metodo_Get_Retorno_200Ok()
        {
            // Act
            var result = await _controller.Get(token);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_students);
        }

        [Fact]
        public async Task Dado_Request_Valido_Metodo_GetById_Retorno_200Ok()
        {
            var expectedStudent = _students.First();
            // Act
            var result = await _controller.GetById(expectedStudent.Id, token);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedStudent);
        }

        [Fact]
        public async Task Dado_Request_Valido_Metodo_Post_Retorno_200Ok()
        {
            // Act
            var result = await _controller.Post(token);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
