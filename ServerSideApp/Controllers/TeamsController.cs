using Application.Dtos.TeamDto;
using Application.ServiceInterfaces;
using Application.Services;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServerSideApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeamsController(IServiceManger _serviceManager) : ControllerBase
{
    // GET api/teams
    [HttpGet]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeams()
    {
        var teams = await _serviceManager.TeamService.GetAllTeamsAsync();
        return Ok(teams);
    }

    // GET api/teams/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
    public async Task<ActionResult<TeamDto>> GetTeamById(int id)
    {
        var team = await _serviceManager.TeamService.GetTeamByIdAsync(id);
        return Ok(team);
    }

    // POST api/teams
    [HttpPost]
    [Authorize(Roles = "Admin,SeniorCoach")]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamDto createTeamDto)
    {
        var team = await _serviceManager.TeamService.CreateTeamAsync(createTeamDto);
        return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, team);
    }

    // PUT api/teams/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
    public async Task<ActionResult<TeamDto>> UpdateTeam(int id, [FromBody] UpdateTeamDto updateTeamDto)
    {
        // Simple guard clause remains here as it's a request-level validation
        if (id != updateTeamDto.Id)
            return BadRequest(new { message = "ID mismatch." });

        var team = await _serviceManager.TeamService.UpdateTeamAsync(updateTeamDto);
        return Ok(team);
    }

    // DELETE api/teams/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        await _serviceManager.TeamService.DeleteTeamAsync(id);
        return NoContent();
    }
}