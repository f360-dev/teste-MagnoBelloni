using F360.Domain.Entities;

namespace F360.Domain.Interfaces.Database.Repositories;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> GetByKeyAsync(string key, CancellationToken cancellationToken);
    Task CreateAsync(IdempotencyKey record, CancellationToken cancellationToken);
}
