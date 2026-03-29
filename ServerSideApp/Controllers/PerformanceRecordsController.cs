using Application.Dtos.PerformanceRecord;
using Application.ServiceInterfaces;
using Application.Services;
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
    public class PerformanceRecordsController(IServiceManger _serviceManger) : Controller
    {

        // GET api/performancerecords
        [HttpGet]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<ActionResult<IEnumerable<PerformanceRecordDto>>> GetAllRecords()
        {
            var records = await _serviceManger.PerformanceRecordService.GetAllRecordsAsync();
            return Ok(records);
        }

        // GET api/performancerecords/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PerformanceRecordDto>> GetRecordById(int id)
        {
            var record = await _serviceManger.PerformanceRecordService.GetRecordByIdAsync(id);
            // Swimmers can only view their own records
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerId = User.FindFirstValue("swimmerId");
                if (swimmerId == null || int.Parse(swimmerId) != record!.SwimmerId)
                    return Forbid();
            }

            return Ok(record);
        }

        // GET api/performancerecords/swimmer/{swimmerId}
        [HttpGet("swimmer/{swimmerId:int}")]
        public async Task<ActionResult<IEnumerable<PerformanceRecordDto>>> GetRecordsBySwimmer(int swimmerId)
        {
            // Swimmers can only view their own records
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerIdClaim = User.FindFirstValue("swimmerId");
                if (swimmerIdClaim == null || int.Parse(swimmerIdClaim) != swimmerId)
                    return Forbid();
            }

            var records = await _serviceManger.PerformanceRecordService.GetRecordsBySwimmerAsync(swimmerId);
            return Ok(records);
        }

        // GET api/performancerecords/swimmer/{swimmerId}/distance/{distance}
        [HttpGet("swimmer/{swimmerId:int}/distance/{distance}")]
        public async Task<ActionResult<IEnumerable<PerformanceRecordDto>>> GetRecordsBySwimmerAndDistance(int swimmerId, EventDistance distance)
        {
            // Swimmers can only view their own records
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Swimmer")
            {
                var swimmerIdClaim = User.FindFirstValue("swimmerId");
                if (swimmerIdClaim == null || int.Parse(swimmerIdClaim) != swimmerId)
                    return Forbid();
            }

            var records = await _serviceManger.PerformanceRecordService
                .GetRecordsBySwimmerAndDistanceAsync(swimmerId, distance);
            return Ok(records);
        }

        // POST api/performancerecords
        [HttpPost]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach,RegularCoach")]
        public async Task<ActionResult<PerformanceRecordDto>> CreateRecord([FromBody] CreatePerformanceRecordDto createRecordDto)
        {
            var record = await _serviceManger.PerformanceRecordService.CreateRecordAsync(createRecordDto);
            return CreatedAtAction(nameof(GetRecordById), new { id = record.Id }, record);
        }

        // PUT api/performancerecords/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,SeniorCoach,HeadCoach")]
        public async Task<ActionResult<PerformanceRecordDto>> UpdateRecord(int id, [FromBody] UpdatePerformanceRecordDto updateRecordDto)
        {
            if (id != updateRecordDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            var record = await _serviceManger.PerformanceRecordService.UpdateRecordAsync(updateRecordDto);
            return Ok(record);
        }

        // DELETE api/performancerecords/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,SeniorCoach")]
        public async Task<ActionResult<PerformanceRecordDto>> DeleteRecord(int id)
        {
            await _serviceManger.PerformanceRecordService.DeleteRecordAsync(id);
            return NoContent();
        }
    }
}
