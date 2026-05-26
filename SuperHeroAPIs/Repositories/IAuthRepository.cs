using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User?> GetByIdAsync(int id);
    }
}
