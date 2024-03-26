using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IMongoWriteContextOptions
{
    MongoClient MongoClient { get; set; }
    string Database { get; set; }
}
public class MongoWriteContextOptions : IMongoWriteContextOptions
{
    public MongoClient MongoClient { get; set; }
    public string Database { get; set; }
}
public interface IMongoReadContextOptions
{
    MongoClient MongoClient { get; set; }
    string Database { get; set; }
}
public class MongoReadContextOptions : IMongoReadContextOptions
{
    public MongoClient MongoClient { get; set; }
    public string Database { get; set; }
}
public interface IWriteContext
{
    IMongoDatabase Database { get; }
}

public class WriteContext : IWriteContext
{
    public IMongoDatabase Database { get; }
    public WriteContext(MongoWriteContextOptions options)
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