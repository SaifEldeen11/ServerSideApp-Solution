using Core.InterFaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICoachRepository Coaches { get; }
        ISwimmerRepository Swimmers { get; }
        ITeamRepository Teams { get; }
        IPerformanceRecordRepository PerformanceRecords { get; }
        IGenericRepository<PerformanceNote> PerformanceNotes { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
