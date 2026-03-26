using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            // Global query filter for soft delete
            modelBuilder.Entity<Coach>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Swimmer>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Team>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PerformanceRecord>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PerformanceNote>().HasQueryFilter(e => !e.IsDeleted);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }



    }
}
