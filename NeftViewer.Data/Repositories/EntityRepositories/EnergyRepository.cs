using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class EnergyRepository : GenericRepository<Energy>
    {
        private readonly NeftViewerContext _dbContext;

        public EnergyRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
