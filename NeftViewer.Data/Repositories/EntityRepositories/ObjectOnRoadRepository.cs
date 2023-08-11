using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class ObjectOnRoadRepository : GenericRepository<ObjectOnRoad>
    {
        private readonly NeftViewerContext _dbContext;

        public ObjectOnRoadRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

   
    }
}
