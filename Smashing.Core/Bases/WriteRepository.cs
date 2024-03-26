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

public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly IWriteContext _context;
    private readonly string _mongoCollection;

    private IMongoCollection<T> _collection => _context.Database.GetCollection<T>(_mongoCollection);

    public WriteRepository(IWriteContext context, string mongoCollection)
    {
        _context = context;
        _mongoCollection = mongoCollection;
    }

    public async Task<List<T>> Select(CancellationToken cancellationToken = default)
    {
        var lista = await _collection.FindAsync(new BsonDocument(), cancellationToken: cancellationToken);
        return lista.ToList();
    }
    public async Task CreateAsync(T newBook, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(newBook, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default)
    {
        //Update(FilterDefinition < T > filter, UpdateDefinition < T > tasko)
        //col.UpdateOne(filter, tasko);
        await _collection.ReplaceOneAsync(x => x.Id == updatedBook.Id, updatedBook, cancellationToken: cancellationToken);
    }
}
