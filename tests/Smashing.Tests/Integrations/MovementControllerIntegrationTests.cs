using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using Smashing.Core.Bases;
using Smashing.Core.Features.Movements;
using System.Text;
using System.Text.Json;

namespace Smashing.Tests.Integrations;

public class MovementControllerIntegrationTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly ApiFixture _server = new();
    private readonly AddMovementCommand _command;

    public MovementControllerIntegrationTests()
    {
        _command = _fixture.Build<AddMovementCommand>()
            .Create();
    }

    private HttpClient Client => _server.CreateClient();

    private IWriteRepository<Movement> _creditNotesWriter =>
        _server.Services.GetRequiredService<IWriteRepository<Movement>>();

    private IReadRepository<Movement> _creditNotesReader =>
        _server.Services.GetRequiredService<IReadRepository<Movement>>();

    [Fact]
    public async Task Given_empty_filter_should_return_Success()
    {

        // Act
        var response = await Client.GetAsync("api/Movement");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(200, (int)response.StatusCode);
        //response.Should().BeOfType<OkObjectResult>();
        //.Which.Value.Should().BeEquivalentTo(_students);
        //response.Should().Be400BadRequest().And
        //                 .MatchInContent("*You need at least one filter value filled.*");
    }
    [Fact]
    public async Task Given_Invalid_Request_Post_Return_NotAuthorized()
    {
        //https://medium.com/asos-techblog/testing-authorization-scenarios-in-asp-net-core-web-api-484bc95d5f6f
        var content = new StringContent(JsonSerializer.Serialize(_command with { Valor = 0 }),
            Encoding.UTF8, "application/json");
        var result = await Client.PostAsync("api/Movement", content);
        Assert.Equal(422, (int)result.StatusCode);
    }

    [Fact]
    public async Task Given_empty_filter_should_return_bad_request()
    {
        // Act
        var content = new StringContent(JsonSerializer.Serialize(_command),
            Encoding.UTF8, "application/json");
        var result = await Client.PostAsync("api/Movement", content);

        // Assert
        result.EnsureSuccessStatusCode();
        Assert.Equal(204, (int)result.StatusCode);
        //response.Should().BeOfType<OkObjectResult>();
        //.Which.Value.Should().BeEquivalentTo(_students);
        //response.Should().Be400BadRequest().And
        //                 .MatchInContent("*You need at least one filter value filled.*");
    }

    [Fact]
    public async Task Given_Invalid_Request_Post_Return_UnprocessableEntity()
    {
        var content = new StringContent(JsonSerializer.Serialize(_command with { Valor = 0 }),
            Encoding.UTF8, "application/json");
        var result = await Client.PostAsync("api/Movement", content);
        Assert.Equal(422, (int)result.StatusCode);
    }
}