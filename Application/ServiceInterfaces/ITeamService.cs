using Application.Dtos.TeamDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto?> GetTeamByIdAsync(int id);
        Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto);
        Task<TeamDto> UpdateTeamAsync(UpdateTeamDto updateTeamDto);
        Task<bool> DeleteTeamAsync(int id);
    }
}
