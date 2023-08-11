using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services
{
    public class ObjectOnRoadService : IObjectOnRoadService
    {
        private readonly IUnitOfWork _uow;
        public ObjectOnRoadService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<ObjectOnRoad> FindObjectOnRoadAsync(string? id)
        {
            return _uow.ObjectOnRoads.GetAsync(id);
        }

        public async Task<IEnumerable<ObjectOnRoad>> GetObjectOnRoads()
        {

            return await _uow.ObjectOnRoads.GetAllAsync();

        }

        public EntityEntry<ObjectOnRoad> UpdateObjectOnRoad(ObjectOnRoad objectOnRoad)
        {
            return _uow.ObjectOnRoads.Update(objectOnRoad);
        }
        public async Task<bool> AddObjectOnRoad(ObjectOnRoad objectOnRoad)
        {
            bool flag = false;
            try
            {
                await _uow.ObjectOnRoads.Add(objectOnRoad);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<ObjectOnRoad> DeleteObjectOnRoad(string id)
        {
            var res = _uow.ObjectOnRoads.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddObjectOnRoadRange(IEnumerable<ObjectOnRoad> objectOnRoads)
        {
            var res = await _uow.ObjectOnRoads.AddRange(objectOnRoads);
            return res;
        }
    }
}
