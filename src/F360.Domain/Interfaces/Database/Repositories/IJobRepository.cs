using F360.Domain.Entities;

namespace F360.Domain.Interfaces.Database.Repositories;

public interface IJobRepository
{
    Task<Job> CreateAsync(Job job, CancellationToken cancellationToken);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Job job, CancellationToken cancellationToken);
}
