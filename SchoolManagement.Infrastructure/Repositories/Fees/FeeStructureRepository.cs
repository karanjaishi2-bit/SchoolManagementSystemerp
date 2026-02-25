using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities.Fees;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories.Fees
{
    public class FeeStructureRepository : Repository<FeeStructure>, IFeeStructureRepository
    {
        public FeeStructureRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ADD THIS — overrides base GetPagedAsync to include FeeItems
        public new async Task<IEnumerable<FeeStructure>> GetPagedAsync(int page, int limit)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeStructure>> GetByClassIdAsync(string classId)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.ClassId == classId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeStructure>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(f => f.FeeItems)
                .Where(f => f.Status == status)
                .ToListAsync();
        }
    }
}