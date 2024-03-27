using MongoDB.Driver;

namespace Smashing.Core.Bases;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAsync(CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}

public abstract class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
{
    private readonly IReadContext _context;
    private readonly string _collectionName;

    protected ReadRepository(IReadContext context, string collectionName)
    {
        _context = context;
        _collectionName = collectionName;
    }

    private IMongoCollection<T> Collection => _context.Database.GetCollection<T>(_collectionName);

    public async Task<List<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await Collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }
}