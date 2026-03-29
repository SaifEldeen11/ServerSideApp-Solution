using Core.Interfaces;
using Core.InterFaces;
using Core.Models;
using Infrastructure.Data.Contexts;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = new();
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICoachRepository Coaches =>
            GetRepository<CoachRepository, Coach>();

        public ISwimmerRepository Swimmers =>
            GetRepository<SwimmerRepository, Swimmer>();

        public ITeamRepository Teams =>
            GetRepository<TeamRepository, Team>();

        public IPerformanceRecordRepository PerformanceRecords =>
            GetRepository<PerformanceRecordRepository, PerformanceRecord>();

        public IGenericRepository<PerformanceNote> PerformanceNotes =>
            GetRepository<GenericRepository<PerformanceNote>, PerformanceNote>();

        private T GetRepository<T, TEntity>(T? repo = null) where T : class where TEntity : class
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.ContainsKey(typeName))
                return (T)_repositories[typeName];

            var repository = repo ?? Activator.CreateInstance(typeof(T), _context) as T;
            _repositories.Add(typeName, repository!);
            return repository!;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}