using Application.Dtos.Swimmer_Dto;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISwimmerService
    {
        Task<IEnumerable<SwimmerDto>> GetAllSwimmersAsyn();
        Task<SwimmerDto?> GetSwimmerByIdAsync(int id);

        Task<IEnumerable<SwimmerDto>> GetSwimmersByTeamIdAsync(int teamId);

        Task<IEnumerable<SwimmerDto>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness);
        Task<SwimmerDto> CreateSwimmerAsync(CreateSwimmerDto createSwimmerDto);
        Task<SwimmerDto> UpdateSwimmerAsync(UpdateSwimmerDto updateSwimmerDto);
        Task<bool> DeleteSwimmerAsync(int id);
        Task<SwimmerDashboardDto?> GetSwimmerDashboardAsync(int swimmerId);
    }
}
