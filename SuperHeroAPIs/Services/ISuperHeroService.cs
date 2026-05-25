using SuperHeroAPIs.DTOs;

namespace SuperHeroAPIs.Services
{
    public interface ISuperHeroService
    {
        Task<List<SuperHeroGetAllDto>> GetAllHeroesAsync();
        Task<SuperHeroGetDto?> GetHeroByIdAsync(int id);
        Task<SuperHeroGetDto> CreateHeroAsync(SuperHeroCreateDto dto);
        Task<SuperHeroGetDto?> UpdateHeroAsync(int id, SuperHeroUpdateDto dto);
        Task<bool> DeleteHeroAsync(int id);
    }
}
