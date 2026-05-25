using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SuperHeroController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllHeroes()
        {
            var heroes = new List<SuperHero>
            {
                new() {
                    Id = 1,
                    Name = "Spiderman",
                    FirstName = "Peter",
                    LastName = "Parker",
                    Place = "New York City"
                },
                new() {
                    Id = 2,
                    Name = "Ironman",
                    FirstName = "Tony",
                    LastName = "Stark",
                    Place = "Long Island"
                }
            };

        return Ok(heroes);
                    
        }
    }
}