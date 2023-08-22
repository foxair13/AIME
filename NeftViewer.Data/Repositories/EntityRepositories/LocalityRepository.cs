using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class LocalityRepository : GenericRepository<Locality>
    {
        private readonly NeftViewerContext _dbContext;

        public LocalityRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
