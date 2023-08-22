using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class OwnerRepository : GenericRepository<Owner>
    {
        private readonly NeftViewerContext _dbContext;

        public OwnerRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
