using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class AspNetUsersRepository : GenericRepository<AspNetUser>
    {
        private readonly NeftViewerContext _dbContext;

        public AspNetUsersRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<AspNetUser>> GetAllAsync()
        {
            return await _dbContext.Set<AspNetUser>().ToListAsync();
        }

        public override async Task<AspNetUser> GetAsync(string? id)
        {
            return await _dbContext.Set<AspNetUser>().FirstAsync(z => z.Id == id);
        }
    }
}
