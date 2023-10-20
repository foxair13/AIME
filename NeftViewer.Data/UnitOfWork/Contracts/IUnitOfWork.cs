using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;

namespace NeftViewer.Data.UnitOfWork.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<AspNetUser> AspNetUsers { get; }
        IGenericRepository<Models.Action> Action { get; }
        IGenericRepository<ActionRole> ActionRole { get; }
        IGenericRepository<Criteria> Criterias { get; }
        IGenericRepository<ObjectItem> ObjectItems { get; }
        IGenericRepository<Road> Roads { get; }
        IGenericRepository<IndicatorValue> IndicatorValues { get; }
        IGenericRepository<ObjectOnRoad> ObjectOnRoads { get; }
        IGenericRepository<Owner> Owners { get; }
        IGenericRepository<Area> Areas { get; }
        IGenericRepository<Locality> Localities { get; }
        IGenericRepository<CriteriaCalcMethod> CriteriaCalcMethods { get; }
        IGenericRepository<Agregate> Agregates { get; }
        IGenericRepository<Trk> Trks { get; }
        IGenericRepository<Tank> Tanks { get; }
        Task CommitAsync();
    }
}
