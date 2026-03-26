using Application.Interfaces;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ServerSideApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IDataSeeder _dataSeeder;
        private readonly ILogger<SeedController> _logger;

        public SeedController(IDataSeeder dataSeeder, ILogger<SeedController> logger)
        {
            _dataSeeder = dataSeeder;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                await _dataSeeder.SeedAllAsync();
                return Ok(new { message = "Database seeded successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding database");
                return StatusCode(500, new { error = "Error seeding database" });
            }
        }
    }
}