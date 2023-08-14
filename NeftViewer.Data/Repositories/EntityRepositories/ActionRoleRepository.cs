using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class ActionRoleRepository : GenericRepository<ActionRole>
    {
        private readonly NeftViewerContext _dbContext;

        public ActionRoleRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<ActionRole> GetAsync(int id)
        {
            return await _dbContext.Set<ActionRole>().Include(x=>x.AspNetRoles).Include(y=>y.Action).FirstAsync(z => z.Id == id);
        }
        public override async Task<IEnumerable<ActionRole>> GetAllAsync()
        {
            return await _dbContext.Set<ActionRole>().Include(x => x.AspNetRoles).Include(y => y.Action).ToListAsync();
        }
    }
}
