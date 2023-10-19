using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class TankRepository : GenericRepository<Tank>
    {
        private readonly NeftViewerContext _dbContext;

        public TankRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
