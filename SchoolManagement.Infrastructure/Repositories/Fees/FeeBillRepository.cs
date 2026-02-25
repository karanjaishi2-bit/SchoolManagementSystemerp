using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities.Fees;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories.Fees
{
    public class FeeBillRepository : Repository<FeeBill>, IFeeBillRepository
    {
        public FeeBillRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ADD THIS — overrides base GetPagedAsync to include FeeItems
        public new async Task<IEnumerable<FeeBill>> GetPagedAsync(int page, int limit)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeBill>> GetByStudentIdAsync(int studentId)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeBill>> GetByClassIdAsync(string classId)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.ClassId == classId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeBill>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeBill>> GetOverdueBillsAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.Status != "Paid" && string.Compare(f.DueDate, today) < 0)
                .ToListAsync();
        }
    }
}