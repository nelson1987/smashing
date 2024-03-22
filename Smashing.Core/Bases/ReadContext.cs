using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IReadContext
{
    IMongoDatabase Database { get; }
}

public class ReadContext : IReadContext
{
    private readonly MongoClient _mongoClient;

    public ReadContext(string connectionString, string databaseName)
    {
        try
        {
            _mongoClient = new MongoClient(connectionString);
            Database = _mongoClient.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }

    public IMongoDatabase Database { get; }
}