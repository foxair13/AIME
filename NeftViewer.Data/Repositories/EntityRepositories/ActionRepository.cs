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
    public class ActionRepository : GenericRepository<Models.Action>
    {
        private readonly NeftViewerContext _dbContext;

        public ActionRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<IEnumerable<Models.Action>> GetAllAsync()
        {
            return await _dbContext.Set<Models.Action>().ToListAsync();
        }

        public override async Task<Models.Action> GetAsync(int id)
        {
            return await _dbContext.Set<Models.Action>().FirstAsync(z => z.Id == id);
        }
    }
}
