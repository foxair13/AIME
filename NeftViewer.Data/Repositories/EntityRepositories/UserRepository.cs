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
    public class UserRepository : GenericRepository<User>
    {
        private readonly NeftViewerContext _dbContext;

        public UserRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Set<User>().ToListAsync();
        }

        public override async Task<User> GetAsync(int? id)
        {
            return await _dbContext.Set<User>().FirstAsync(z => z.Id == id);
        }
    }
}
