using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IWriteRepository<in T> where T : BaseEntity
{
    Task CreateAsync(T newBook, CancellationToken cancellationToken = default);

    Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default);
}

public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly IWriteContext _context;
    private IMongoCollection<T> _collection => _context.Database.GetCollection<T>(nameof(T));
    public WriteRepository(IWriteContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(T newBook, CancellationToken cancellationToken = default) =>
        await _collection.InsertOneAsync(newBook);

    public async Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default) =>
        await _collection.ReplaceOneAsync(x => x.Id == updatedBook.Id, updatedBook);
}
