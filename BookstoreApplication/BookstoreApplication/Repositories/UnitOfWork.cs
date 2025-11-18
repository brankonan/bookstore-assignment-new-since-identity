using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using BookstoreApplication.Models;

namespace BookstoreApplication.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _tran;

        public UnitOfWork(AppDbContext ctx)
        {
            _context = ctx;
        }

        public async Task BeginTransactionAsync()
        {
            _tran = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            if (_tran != null)
            {
                await _tran.CommitAsync();
                await _tran.DisposeAsync();
                _tran = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_tran != null)
            {
                await _tran.RollbackAsync();
                await _tran.DisposeAsync();
                _tran = null;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
