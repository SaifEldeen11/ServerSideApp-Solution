using Application.Dtos.PerformanceRecord;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPerformanceRecordService
    {
        Task<IEnumerable<PerformanceRecordDto>> GetAllRecordsAsync();
        Task<PerformanceRecordDto?> GetRecordByIdAsync(int id);
        Task<IEnumerable<PerformanceRecordDto>> GetRecordsBySwimmerAsync(int swimmerId);
        Task<IEnumerable<PerformanceRecordDto>> GetRecordsBySwimmerAndDistanceAsync(int swimmerId, EventDistance distance);
        Task<PerformanceRecordDto> CreateRecordAsync(CreatePerformanceRecordDto createRecordDto);
        Task<PerformanceRecordDto> UpdateRecordAsync(UpdatePerformanceRecordDto updateRecordDto);
        Task<bool> DeleteRecordAsync(int id);
    }
}
