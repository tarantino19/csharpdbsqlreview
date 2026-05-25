using Microsoft.EntityFrameworkCore;
using SuperHeroAPIs.Data;
using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Repositories
{
    public class SuperHeroRepository : ISuperHeroRepository
    {
        private readonly DataContext _context;

        public SuperHeroRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<SuperHero>> GetAllAsync()
        {
            return await _context.SuperHeroes.ToListAsync();
        }

        public async Task<SuperHero?> GetByIdAsync(int id)
        {
            return await _context.SuperHeroes.FindAsync(id);
        }

        public async Task<SuperHero> CreateAsync(SuperHero hero)
        {
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();
            return hero;
        }

        public async Task<SuperHero> UpdateAsync(SuperHero hero)
        {
            await _context.SaveChangesAsync();
            return hero;
        }

        public async Task DeleteAsync(SuperHero hero)
        {
            _context.SuperHeroes.Remove(hero);
            await _context.SaveChangesAsync();
        }
    }
}
