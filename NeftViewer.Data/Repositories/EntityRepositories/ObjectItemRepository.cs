using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class ObjectItemRepository : GenericRepository<ObjectItem>
    {
        private readonly NeftViewerContext _dbContext;

        public ObjectItemRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
