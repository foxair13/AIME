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
    public class AspNetUsersRepository : GenericRepository<AspNetUsers>
    {
        private readonly NeftViewerContext _dbContext;

        public AspNetUsersRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<AspNetUsers>> GetAllAsync()
        {
            return await _dbContext.Set<AspNetUsers>().ToListAsync();
        }

        public override async Task<AspNetUsers> GetAsync(string? id)
        {
            return await _dbContext.Set<AspNetUsers>().FirstAsync(z => z.Id == id);
        }
    }
}
