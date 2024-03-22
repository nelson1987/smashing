using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IWriteContext
{
    IMongoDatabase Database { get; }
}

public class WriteContext : IWriteContext
{
    private readonly MongoClient _mongoClient;

    public WriteContext(string connectionString, string databaseName)
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