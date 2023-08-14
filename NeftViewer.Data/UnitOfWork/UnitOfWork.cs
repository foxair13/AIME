using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using NeftViewer.Data.Repositories.EntityRepositories;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NeftViewerContext _context;

        public UnitOfWork(
            NeftViewerContext context,
            IGenericRepository<AspNetUser> AspNetUsersRepository,
             IGenericRepository<Models.Action> ActionRepository,
             IGenericRepository<ActionRole> ActionRoleRepository,
              IGenericRepository<Criteria> CriteriaRepository,
              IGenericRepository<Road> RoadRepository,
              IGenericRepository<ObjectItem> ObjectItemRepository,
              IGenericRepository<ObjectOnRoad> ObjectOnRoadRepository,
              IGenericRepository<Customer> CustomerRepository,
              IGenericRepository<IndicatorValue> IndicatorValueRepository
            )
        {
            _context = context;
            AspNetUsers = AspNetUsersRepository;
            Action = ActionRepository;
            ActionRole = ActionRoleRepository;
            Criterias = CriteriaRepository;
            Roads = RoadRepository;
            ObjectItems = ObjectItemRepository;
            ObjectOnRoads = ObjectOnRoadRepository;
            Customers = CustomerRepository;
            IndicatorValues = IndicatorValueRepository;
        }
        public IGenericRepository<AspNetUser> AspNetUsers { get; }
        public IGenericRepository<Models.Action> Action { get; }
        public IGenericRepository<ActionRole> ActionRole { get; }
        public IGenericRepository<Criteria> Criterias { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<ObjectItem> ObjectItems { get; }
        public IGenericRepository<Road> Roads { get; }
        public IGenericRepository<IndicatorValue> IndicatorValues { get; }
        public IGenericRepository<ObjectOnRoad> ObjectOnRoads { get; }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
