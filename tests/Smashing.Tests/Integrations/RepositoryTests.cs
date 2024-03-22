using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Smashing.Core.Bases;
using Smashing.Core.Features.Movements;

namespace Smashing.Tests.Integrations
{
    /*
    public abstract class IntegrationTest : IAsyncLifetime
    {
        //public Fixture ObjectGenerator { get; } = new Fixture().GeneratingUtcDateTimeValues();
        protected ApiFixture ApiFixture { get; }
        //protected KafkaFixture KafkaFixture { get; }
        //protected HttpServerFixture HttpServerFixture { get; }
        protected MongoFixture MongoFixture { get; }

        protected IntegrationTest(IntegrationTestFixture integrationTestFixture)
        {
            //OperationContext.InitializeCurrentContext();
            ApiFixture = integrationTestFixture.ApiFixture;
            //KafkaFixture = integrationTestFixture.KafkaFixture;
            //HttpServerFixture = integrationTestFixture.HttpServerFixture;
            MongoFixture = integrationTestFixture.MongoFixture;
        }

        // https://github.com/VerifyTests/Verify#static-settings
        [ModuleInitializer]
        public static void Initialize()
        {
            AssertionOptions.FormattingOptions.MaxLines = 10000;
            //CustomVerifierSettings.Initialize();
        }

        public virtual async Task InitializeAsync()
        {
            ApiFixture.Reset();
            //KafkaFixture.Reset();
            //HttpServerFixture.Reset();
            await MongoFixture.Reset();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
    public class IntegrationTestFixture : IAsyncLifetime
    {
        public ApiFixture ApiFixture { get; }
        //public KafkaFixture KafkaFixture { get; }
        //public HttpServerFixture HttpServerFixture { get; }
        public MongoFixture MongoFixture { get; }

        public IntegrationTestFixture()
        {
            ApiFixture = new ApiFixture();
            //KafkaFixture = new KafkaFixture(ApiFixture.Server);
            //HttpServerFixture = new HttpServerFixture();
            MongoFixture = new MongoFixture(ApiFixture.Server);
        }

        public async Task InitializeAsync()
        {
            //await KafkaFixture.WarmUp();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ApiFixture.DisposeAsync();
            //HttpServerFixture.Dispose();
        }
    }
    public sealed class ApiFixture : IAsyncDisposable
    {
        public Api Server { get; } = new();
        public HttpClient Client { get; }

        public ApiFixture()
        {
            Client = Server.CreateDefaultClient();
        }

        public void Reset()
        {
            Client.DefaultRequestHeaders.Clear();

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme: "TestScheme");
        }

        public async ValueTask DisposeAsync()
        {
            Client.Dispose();
            await Server.DisposeAsync();
        }

        public class Api : WebApplicationFactory<Program>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
                => builder.UseEnvironment("Test")
                       .ConfigureTestServices(services =>
                       {
                           //services.AddMockedAzureAdCredentials();
                           //services.AddMockedApiAuthentication();
                           //services.ConfigureKafkaServices();
                           ConfigureConsumers(services);
                       });


            internal Task Consume<TConsumer>(TimeSpan? timeout = null) where TConsumer : IConsumer
            {
                const int defaultTimeoutInSeconds = 1;
                timeout ??= TimeSpan.FromSeconds(defaultTimeoutInSeconds);

                using var scope = Services.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<TConsumer>();
                if (Debugger.IsAttached)
                    return consumer.Consume(CancellationToken.None);

                using var tokenSource = new CancellationTokenSource(timeout.Value);
                return consumer.Consume(tokenSource.Token);
            }

            private static void ConfigureConsumers(IServiceCollection services)
            {
                var workerTypes = Assembly
                    .GetAssembly(typeof(IConsumer))!
                    .GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IConsumer)))
                    .Where(t => !t.IsAbstract);

                foreach (var workerType in workerTypes)
                    services.AddScoped(workerType);
            }
        }
    }
    public class MongoFixture
    {
        public IMongoDatabase MongoDatabase { get; }

        public MongoFixture(ApiFixture.Api server)
        {
            var configuration = server.Services.GetRequiredService<IConfiguration>();
            var mongoUrl = new MongoUrl(configuration.GetConnectionString("MongoDB"));
            var mongoClient = new MongoClient(mongoUrl);
            MongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public async Task Reset()
        {
            var collectionNames = MongoDatabase.ListCollectionNames();
            while (await collectionNames.MoveNextAsync())
            {
                foreach (var collectionName in collectionNames.Current)
                {
                    await MongoDatabase
                        .GetCollection<BsonDocument>(collectionName)
                        .DeleteManyAsync(_ => true);
                }
            }
        }
    }
    
    
    
    */
    public class ApiFixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => builder.UseEnvironment("Testing");
    }
    public class CreditNotesReaderTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly ApiFixture _server = new ApiFixture();
        private HttpClient Client => _server.CreateClient();
        private IWriteRepository<Movement> _creditNotesWriter => _server.Services.GetRequiredService<IWriteRepository<Movement>>();
        private IReadRepository<Movement> _creditNotesReader => _server.Services.GetRequiredService<IReadRepository<Movement>>();

        public CreditNotesReaderTests()
        {
        }

        [Fact]
        public async Task Given_asset_id_filter_should_get_credit_note()
        {
            // Arrange
            var creditNote = _fixture.Build<Movement>().With(x=>x.CreatedAt, DateTime.UtcNow).Create();
            await _creditNotesWriter.CreateAsync(creditNote);

            // Act
            var result = await _creditNotesReader.GetAsync(creditNote.Id, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(creditNote, options => options
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
                    .WhenTypeIs<DateTime>());
        }
    }
}
