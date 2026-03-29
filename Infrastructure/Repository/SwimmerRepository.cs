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
using Core.InterFaces;

namespace Infrastructure.Repository
{
    public class SwimmerRepository : GenericRepository<Swimmer>,ISwimmerRepository
    {
        private readonly ApplicationDbContext _context;

        public SwimmerRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public override async Task<Swimmer?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Team)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Swimmer>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness)
        {
            return await _dbSet
                .Where(s => s.CompetitionReadiness == readiness)
                .Include(s => s.Team)
                .ToListAsync();
        }

        public async Task<IEnumerable<Swimmer>> GetSwimmersByTeamAsync(int teamId)
        {
            return await _dbSet
                .Where(S => S.TeamId == teamId)
                .Include(s => s.Team)
                .ToListAsync();
        }

        public async Task<Swimmer?> GetSwimmerWithNotesAsync(int swimmerId)
        {
            return await _dbSet
                .Include(s => s.PerformanceNotes)
                .ThenInclude(n => n.Coach)
                .FirstOrDefaultAsync(s => s.Id == swimmerId);
        }

        public async Task<Swimmer?> GetSwimmerWithPerformanceRecordsAsync(int swimmerId)
        {
            return await _dbSet
                .Include(s => s.PerformanceRecords)
                .ThenInclude(pr => pr.RecordedByCoach)
                .Include(s => s.Team)
                .FirstOrDefaultAsync(s => s.Id == swimmerId);
        }
    }
}
