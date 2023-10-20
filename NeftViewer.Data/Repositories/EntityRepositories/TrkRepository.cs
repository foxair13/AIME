using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class TrkRepository : GenericRepository<Trk>
    {
        private readonly NeftViewerContext _dbContext;

        public TrkRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
