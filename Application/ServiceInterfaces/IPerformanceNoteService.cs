using Application.Dtos.PerformanceNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPerformanceNoteService
    {
        Task<IEnumerable<PerformanceNoteDto>> GetAllNotesAsync();
        Task<PerformanceNoteDto?> GetNoteByIdAsync(int id);
        Task<IEnumerable<PerformanceNoteDto>> GetNotesBySwimmerAsync(int swimmerId);
        Task<PerformanceNoteDto> CreateNoteAsync(CreatePerformanceNoteDto createNoteDto);
        Task<bool> DeleteNoteAsync(int id);
    }
}
