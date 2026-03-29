using Application.Dtos.Swimmer_Dto;
using Application.Pagination;
using Application.ServiceInterfaces;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ServerSideApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class SwimmersController(IServiceManger _serviceManager) : ControllerBase
    {
        // GET api/swimmers
        [HttpGet]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<IActionResult> GetAllSwimmers([FromQuery] PaginationParams paginationParams)
        {
            var swimmers = await _serviceManager.SwimmerService.GetAllSwimmersAsync(paginationParams);
            return Ok(swimmers);
        }

        // GET api/swimmers/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SwimmerDto>> GetSwimmerById(int id)
        {
            // Swimmers can only view their own profile
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerId = User.FindFirstValue("swimmerId");
                if (swimmerId == null || int.Parse(swimmerId) != id)
                    return Forbid();
            }

            var swimmer = await _serviceManager.SwimmerService.GetSwimmerByIdAsync(id);
            if (swimmer is null)
                return NotFound(new { message = $"Swimmer with ID {id} was not found" });

            return Ok(swimmer);
        }

        // GET api/swimmers/team/{teamId}
        [HttpGet("team/{teamId:int}")]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<ActionResult<IEnumerable<SwimmerDto>>> GetSwimmersByTeam(int teamId,[FromQuery]PaginationParams paginationParams)
        {
            var swimmers = await _serviceManager.SwimmerService.GetSwimmersByTeamIdAsync(teamId, paginationParams);
            return Ok(swimmers);
        }

        // GET api/swimmers/readiness/{status}
        [HttpGet("readiness/{status}")]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<ActionResult<IEnumerable<SwimmerDto>>> GetSwimmersByReadiness(CompetitionReadiness status,[FromQuery]PaginationParams paginationParams)
        {
            var swimmers = await _serviceManager.SwimmerService.GetSwimmersByReadinessAsync(status, paginationParams);
            return Ok(swimmers);
        }

         //GET api/swimmers/{id}/dashboard
        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<SwimmerDto>> GetSwimmerDashboard(int id)
        {
            // Swimmers can only view their own dashboard
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerId = User.FindFirstValue("swimmerId");
                if (swimmerId == null || int.Parse(swimmerId) != id)
                    return Forbid();
            }

            var dashboard = await _serviceManager.SwimmerService.GetSwimmerDashboardAsync(id);
            if (dashboard is null)
                return NotFound(new { message = $"Swimmer with ID {id} was not found" });

            return Ok(dashboard);
        }

        // POST api/swimmers
        [HttpPost]
        [Authorize(Roles = "Admin,SeniorCoach")]
        public async Task<ActionResult<SwimmerDto>> CreateSwimmer([FromBody] CreateSwimmerDto createSwimmerDto)
        {
            var swimmer = await _serviceManager.SwimmerService.CreateSwimmerAsync(createSwimmerDto);
            return CreatedAtAction(nameof(GetSwimmerById), new { id = swimmer.Id }, swimmer);
        }

        // PUT api/swimmers/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
        public async Task<ActionResult<SwimmerDto>> UpdateSwimmer(int id, [FromBody] UpdateSwimmerDto updateSwimmerDto)
        {
            if (id != updateSwimmerDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            var swimmer = await _serviceManager.SwimmerService.UpdateSwimmerAsync(updateSwimmerDto);
            return Ok(swimmer);
        }

         //DELETE api/swimmers/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SwimmerDto>> DeleteSwimmer(int id)
        {
            await _serviceManager.SwimmerService.DeleteSwimmerAsync(id);
            return NoContent();
        }
    }
}