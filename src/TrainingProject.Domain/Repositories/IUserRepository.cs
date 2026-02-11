using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);

    Task<User> GetByIdAsync(string userId, CancellationToken cancellationToken);

    Task<User> CreateAsync(User user, CancellationToken cancellationToken);

    Task<User> DeleteAsync(string userId, CancellationToken cancellationToken);

    Task<User> UpdateAsync(string userId, User user, CancellationToken cancellationToken);
}
