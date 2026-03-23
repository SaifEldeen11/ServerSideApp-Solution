using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
        Task SeedCoachesAsync();
        Task SeedTeamsAsync();
        Task SeedSwimmersAsync();
        Task SeedPerformanceRecordsAsync();
        Task SeedPerformanceNotesAsync();
    }
}
