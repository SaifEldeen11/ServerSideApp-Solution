using Application.Interfaces;
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
    public class CoachRepository : GenericRepository<Coach>, ICoachRepository
    {
        public CoachRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<Coach?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
