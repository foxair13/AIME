using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class AgregateRepository : GenericRepository<Agregate>
    {
        private readonly NeftViewerContext _dbContext;

        public AgregateRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
