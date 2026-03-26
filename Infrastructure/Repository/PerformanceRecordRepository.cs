using Core.Enums;
using Core.Models;
using Core.Interfaces;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PerformanceRecordRepository : GenericRepository<PerformanceRecord>, IPerformanceRecordRepository
    {
        public PerformanceRecordRepository(ApplicationDbContext context):base(context)
        {
            
        }
        public async Task<PerformanceRecord?> GetBestTimeAsync(int swimmerId, EventDistance distance)
        {
            return await _dbSet
                .Where(pr => pr.SwimmerId == swimmerId && pr.Distance == distance)
                .OrderBy(pr => pr.TimeInSeconds)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PerformanceRecord>> GetRecentRecordsAsync(int swimmerId, int count = 10)
        {
            return await _dbSet
                .Where(pr => pr.SwimmerId == swimmerId)
                .Include(pr => pr.RecordedByCoach)
                .OrderByDescending(pr => pr.RecordedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<PerformanceRecord>> GetRecordsBySwimmerAndDistanceAsync(int swimmerId, EventDistance distance)
        {
            return await _dbSet
                .Where(pr => pr.SwimmerId == swimmerId && pr.Distance == distance)
                .Include(pr => pr.RecordedByCoach)
                .OrderByDescending(pr => pr.RecordedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PerformanceRecord>> GetRecordsBySwimmerAsync(int swimmerId)
        {
            return await _dbSet
                .Where(pr => pr.SwimmerId == swimmerId)
                .Include(pr => pr.RecordedByCoach)
                .OrderByDescending(pr => pr.RecordedDate)
                .ToListAsync();
        }
    }
}
