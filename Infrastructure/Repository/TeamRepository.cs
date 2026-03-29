using Core.Interfaces;
using Core.Models;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<Team?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Coach)
                .Include(t => t.Swimmers)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
