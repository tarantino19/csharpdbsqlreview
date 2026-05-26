using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPIs.DTOs;
using SuperHeroAPIs.Services;

namespace SuperHeroAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly ISuperHeroService _service;

        public SuperHeroController(ISuperHeroService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHeroGetAllDto>>> GetAllHeroes()
        {
            var heroes = await _service.GetAllHeroesAsync();
            return Ok(heroes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHeroGetDto>> GetHero(int id)
        {
            var hero = await _service.GetHeroByIdAsync(id);
            if (hero is null)
                return NotFound("Hero not found");

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<SuperHeroGetDto>> AddHero(SuperHeroCreateDto request)
        {
            var hero = await _service.CreateHeroAsync(request);
            return Ok(hero);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SuperHeroGetDto>> UpdateHero(int id, SuperHeroUpdateDto request)
        {
            var hero = await _service.UpdateHeroAsync(id, request);
            if (hero is null)
                return NotFound("Hero not found");


            return Ok(hero);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHero(int id)
        {
            var deleted = await _service.DeleteHeroAsync(id);
            if (!deleted)
                return NotFound("Hero not found");

            return Ok("Hero successfully deleted");
        }
    }
}