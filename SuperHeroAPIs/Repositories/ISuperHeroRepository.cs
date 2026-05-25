using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Repositories
{
    public interface ISuperHeroRepository
    {
        Task<List<SuperHero>> GetAllAsync();
        Task<SuperHero?> GetByIdAsync(int id);
        Task<SuperHero> CreateAsync(SuperHero hero);
        Task<SuperHero> UpdateAsync(SuperHero hero);
        Task DeleteAsync(SuperHero hero);
    }
}
