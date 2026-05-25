using SuperHeroAPIs.DTOs;
using SuperHeroAPIs.Entities;
using SuperHeroAPIs.Repositories;

namespace SuperHeroAPIs.Services
{
    public class SuperHeroService : ISuperHeroService
    {
        private readonly ISuperHeroRepository _repository;

        public SuperHeroService(ISuperHeroRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SuperHeroGetAllDto>> GetAllHeroesAsync()
        {
            var heroes = await _repository.GetAllAsync();
            return heroes.Select(MapToGetAllDto).ToList();
        }

        public async Task<SuperHeroGetDto?> GetHeroByIdAsync(int id)
        {
            var hero = await _repository.GetByIdAsync(id);
            return hero is null ? null : MapToGetDto(hero);
        }

        public async Task<SuperHeroGetDto> CreateHeroAsync(SuperHeroCreateDto dto)
        {
            var hero = new SuperHero
            {
                Name = dto.Name,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Place = dto.Place
            };

            var created = await _repository.CreateAsync(hero);
            return MapToGetDto(created);
        }

        public async Task<SuperHeroGetDto?> UpdateHeroAsync(int id, SuperHeroUpdateDto dto)
        {
            var hero = await _repository.GetByIdAsync(id);
            if (hero is null) return null;

            hero.Name = dto.Name;
            hero.FirstName = dto.FirstName;
            hero.LastName = dto.LastName;
            hero.Place = dto.Place;

            var updated = await _repository.UpdateAsync(hero);
            return MapToGetDto(updated);
        }

        public async Task<bool> DeleteHeroAsync(int id)
        {
            var hero = await _repository.GetByIdAsync(id);
            if (hero is null) return false;

            await _repository.DeleteAsync(hero);
            return true;
        }

        private static SuperHeroGetAllDto MapToGetAllDto(SuperHero hero) => new()
        {
            Id = hero.Id,
            Name = hero.Name,
            FirstName = hero.FirstName,
            LastName = hero.LastName,
            Place = hero.Place
        };

        private static SuperHeroGetDto MapToGetDto(SuperHero hero) => new()
        {
            Id = hero.Id,
            Name = hero.Name,
            FirstName = hero.FirstName,
            LastName = hero.LastName,
            Place = hero.Place
        };
    }
}
