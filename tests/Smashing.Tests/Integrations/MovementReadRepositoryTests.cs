using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Smashing.Core.Bases;
using Smashing.Core.Features.Movements;

namespace Smashing.Tests.Integrations
{
    public static class PostgresqlFixture
    {
        public static IWriteContext Context { get; private set; }
        //new WriteContext(mongoConn, mongoDbConnRead)
    }
    public class MovementReadRepositoryTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        private readonly MovementReadRepository _movementWriteRepository;
        private readonly CancellationToken _token = CancellationToken.None;
        public MovementReadRepositoryTests()
        {
            //_fixture.Register(() => DateTime.UtcNow.AddDays(_fixture.Create<int>()));
            IWriteContext
            _movementWriteRepository = new MovementWriteRepository(PostgresqlFixture.Context);
            //    PostgresqlFixture.Context,
            //    PostgresqlFixture.ConnectionFactory,
            //    new PostgresCompiler());

        }
        [Fact]
        public async Task Given_an_order_list_should_persist_and_validate_count()
        {
            // Arrange
            var movements = _fixture.CreateMany<Movement>();
            await PostgresqlFixture.Context.AddRangeAsync(movements);
            await PostgresqlFixture.Context.SaveChangesAsync();
            var documents = movements.Select(x => new Movement()
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                UserName = x.UserName,
                Title = x.Title,
                Valor = x.Valor
            }).ToList();

            // Act
            var copyResult = await _movementWriteRepository.GetAsync(_token);

            // Assert
            copyResult.Should().BeEquivalentTo(movements);
        }
    }
}
