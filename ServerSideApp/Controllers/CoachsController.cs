using Application.Dtos.Coach_Dto;
using Application.ServiceInterfaces;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ServerSideApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("general")]
public class CoachesController(IServiceManger _serviceManager) : ControllerBase
{
    // GET api/coaches
    [HttpGet]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
    public async Task<ActionResult<IEnumerable<CoachDto>>> GetAllCoaches()
    {
        var coaches = await _serviceManager.CoachService.GetAllCoachesAsync();
        return Ok(coaches);
    }

    // GET api/coaches/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
    public async Task<ActionResult<CoachDto>> GetCoachById(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == UserRoles.RegularCoach || userRole == UserRoles.HeadCoach)
        {
            var coachId = User.FindFirstValue("coachId");
            if (coachId == null || int.Parse(coachId) != id)
                return Forbid();
        }

        var coach = await _serviceManager.CoachService.GetCoachByIdAsync(id);
        return Ok(coach);
    }

    // POST api/coaches
    [HttpPost]
    [Authorize(Roles = "Admin,SeniorCoach")]
    public async Task<ActionResult<CoachDto>> CreateCoach([FromBody] CreateCoachDto coachDto)
    {
        var coach = await _serviceManager.CoachService.CreateCoachAsync(coachDto);
        return CreatedAtAction(nameof(GetCoachById), new { id = coach.Id }, coach);
    }

    // PUT api/coaches/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
    public async Task<ActionResult<CoachDto>> UpdateCoach(int id, [FromBody] UpdateCoachDto coachDto)
    {
        if (id != coachDto.Id)
            return BadRequest(new { message = "ID mismatch." });

        var updatedCoach = await _serviceManager.CoachService.UpdateCoachAsync(coachDto);
        return Ok(updatedCoach);
    }

    // DELETE api/coaches/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CoachDto>> DeleteCoach(int id)
    {
        await _serviceManager.CoachService.DeleteCoachAsync(id);
        return NoContent();
    }
}