using Application.Dtos.Swimmer_Dto;
using Application.Pagination;
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
        Task<PaginatedResult<SwimmerDto>> GetAllSwimmersAsync(PaginationParams pagination);
        Task<SwimmerDto?> GetSwimmerByIdAsync(int id);

        Task<PaginatedResult<SwimmerDto>> GetSwimmersByTeamIdAsync(int teamId, PaginationParams pagination);
        Task<PaginatedResult<SwimmerDto>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness, PaginationParams pagination);
        Task<SwimmerDto> CreateSwimmerAsync(CreateSwimmerDto createSwimmerDto);
        Task<SwimmerDto> UpdateSwimmerAsync(UpdateSwimmerDto updateSwimmerDto);
        Task<bool> DeleteSwimmerAsync(int id);
        Task<SwimmerDashboardDto?> GetSwimmerDashboardAsync(int swimmerId);
    }
}
