using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class RoadRepository : GenericRepository<Road>
    {
        private readonly NeftViewerContext _dbContext;

        public RoadRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

   
    }
}
