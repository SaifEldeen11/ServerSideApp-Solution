using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InterFaces
{
    public interface ISwimmerRepository : IGenericRepository<Swimmer>
    {
        Task<IEnumerable<Swimmer>> GetSwimmersByTeamAsync(int teamId);
        Task<IEnumerable<Swimmer>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness);
        Task<Swimmer?> GetSwimmerWithPerformanceRecordsAsync(int swimmerId);
        Task<Swimmer?> GetSwimmerWithNotesAsync(int swimmerId);
    }
}
