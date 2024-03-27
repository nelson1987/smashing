using MongoDB.Bson;
using MongoDB.Driver;
using Smashing.Core.Features.Movements;

namespace Smashing.Core.Bases;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task<List<T>?> Select(CancellationToken cancellationToken = default);

    Task CreateAsync(T newBook, CancellationToken cancellationToken = default);

    Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default);
}

public abstract class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly IWriteContext _context;
    private readonly string _mongoCollection;

    private IMongoCollection<T> Collection => _context.Database.GetCollection<T>(_mongoCollection);

    protected WriteRepository(IWriteContext context, string mongoCollection)
    {
        _context = context;
        _mongoCollection = mongoCollection;
    }

    public async Task<List<T>?> Select(CancellationToken cancellationToken = default)
    {
        var lista = await Collection.FindAsync(new BsonDocument(), cancellationToken: cancellationToken);
        return lista.ToList();
    }
    public async Task CreateAsync(T newBook, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(newBook, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default)
    {
        //Update(FilterDefinition < T > filter, UpdateDefinition < T > tasko)
        //col.UpdateOne(filter, tasko);
        await Collection.ReplaceOneAsync(x => x.Id == updatedBook.Id, updatedBook, cancellationToken: cancellationToken);
    }
}
