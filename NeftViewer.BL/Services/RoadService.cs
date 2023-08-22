using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class RoadService : IRoadService
    {
        private readonly IUnitOfWork _uow;
        public RoadService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Road> FindRoadAsync(string? id)
        {
            return _uow.Roads.GetAsync(id);
        }

        public async Task<IEnumerable<Road>> GetRoads()
        {
            return await _uow.Roads.GetAllAsync();
        }

        public EntityEntry<Road> UpdateRoad(Road road)
        {
            return _uow.Roads.Update(road);
        }
        public async Task<bool> AddRoad(Road road)
        {
            bool flag = false;
            try
            {
                await _uow.Roads.Add(road);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Road> DeleteRoad(string id)
        {
            var res = _uow.Roads.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddRoadRange(IEnumerable<Road> roads)
        {
            var res = await _uow.Roads.AddRange(roads);
            return res;
        }
    }
}
