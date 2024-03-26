using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IReadContext
{
    IMongoDatabase Database { get; }
}

public class ReadContext : IReadContext
{
    public IMongoDatabase Database { get; }

    public ReadContext(MongoReadContextOptions options)
    {
        try
        {
            Database = options.MongoClient.GetDatabase(options.Database);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }
}