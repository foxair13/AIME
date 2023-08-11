using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class IndicatorValueRepository : GenericRepository<IndicatorValue>
    {
        private readonly NeftViewerContext _dbContext;

        public IndicatorValueRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

   
    }
}
