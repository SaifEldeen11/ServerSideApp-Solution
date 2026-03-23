using Application.Dtos.Coach_Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICoachService
    {
        Task<IEnumerable<CoachDto>> GetAllCoachesAsync();
        Task<CoachDto> GetCoachByIdAsync(int id);
        Task<CoachDto> CreateCoachAsync(CreateCoachDto createCoachDto);
        Task<CoachDto> UpdateCoachAsync(UpdateCoachDto updateCoachDto);
        Task<bool> DeleteCoachAsync(int id);
        Task<bool> CoachExistsAsync(int id);
    }
}
