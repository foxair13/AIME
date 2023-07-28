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

    }
}
