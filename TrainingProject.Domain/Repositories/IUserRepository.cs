using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);

    Task<User> GetByIdAsync(string userId, CancellationToken cancellationToken);

    Task<string> CreateAsync(User user, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(string userId, User user, CancellationToken cancellationToken);
}
