using Application.Dtos.PerformanceNote;
using Application.ServiceInterfaces;
using Application.Services;
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
    public class PerformanceNotesController(IServiceManger _serviceManger) : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        // GET api/performancenotes
        public async Task<ActionResult<IEnumerable<PerformanceNoteDto>>> GetAllNotes()
        {
            var notes = await _serviceManger.PerformanceNoteService.GetAllNotesAsync();
            return Ok(notes);
        }
        [HttpGet("{id:int}")]
        // GET api/performancenotes/{id}
        public async Task<ActionResult<PerformanceNoteDto>> GetNoteById(int id)
        {
            var note = await _serviceManger.PerformanceNoteService.GetNoteByIdAsync(id);
            if (note is null)
                return NotFound(new { message = $"Performance note with ID {id} was not found" });

            // Swimmers can only view their own notes
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerId = User.FindFirstValue("swimmerId");
                if (swimmerId == null || int.Parse(swimmerId) != note.SwimmerId)
                    return Forbid();
            }

            return Ok(note);
        }

        // GET api/performancenotes/swimmer/{swimmerId}
        [HttpGet("swimmer/{swimmerId:int}")]
        public async Task<ActionResult<IEnumerable<PerformanceNoteDto>>> GetNotesBySwimmer(int swimmerId)
        {
            // Swimmers can only view their own notes
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerIdClaim = User.FindFirstValue("swimmerId");
                if (swimmerIdClaim == null || int.Parse(swimmerIdClaim) != swimmerId)
                    return Forbid();
            }

            var notes = await _serviceManger.PerformanceNoteService.GetNotesBySwimmerAsync(swimmerId);
            return Ok(notes);
        }

        // POST api/performancenotes
        [HttpPost]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<ActionResult<PerformanceNoteDto>> CreateNote([FromBody] CreatePerformanceNoteDto createNoteDto)
        {
            var note = await _serviceManger.PerformanceNoteService.CreateNoteAsync(createNoteDto);
            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        // DELETE api/performancenotes/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
        public async Task<ActionResult<PerformanceNoteDto>> DeleteNote(int id)
        {
            await _serviceManger.PerformanceNoteService.DeleteNoteAsync(id);
            return NoContent();
        }
    }
}
