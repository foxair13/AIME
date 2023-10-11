using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class ObjectItemService : IObjectItemService
    {
        private readonly IUnitOfWork _uow;
        public ObjectItemService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<ObjectItem> FindObjectItemAsync(string? id)
        {
            return _uow.ObjectItems.GetAsync(id);
        }

        public async Task<IEnumerable<ObjectItem>> GetObjectItems()
        {
            return await _uow.ObjectItems.GetAllAsync();
        }

        public EntityEntry<ObjectItem> UpdateObjectItem(ObjectItem objectItem)
        {
            return _uow.ObjectItems.Update(objectItem);
        }

        public async Task UpdateObjectItemRange(List<ObjectItem> objectItems, string targetPropertyName)
        {
            _uow.ObjectItems.UpdateRange(objectItems, targetPropertyName);
        }

        public async Task<bool> AddObjectItem(ObjectItem objectItem)
        {
            bool flag = false;
            try
            {
                await _uow.ObjectItems.Add(objectItem);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<ObjectItem> DeleteObjectItem(string id)
        {
            var res = _uow.ObjectItems.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddObjectItemRange(IEnumerable<ObjectItem> objectItems)
        {
            var res = await _uow.ObjectItems.AddRange(objectItems);
            return res;
        }

        public async Task AddObjectItemRange(IEnumerable<ObjectItem> objectItems, string targetPropertyName)
        {
            await _uow.ObjectItems.AddRange(objectItems, targetPropertyName);
        }

        public async Task<bool> UpdateObjectItemCoordinatesAsync(string codeSuid, double latitude, double longitude, string ownerName)
        {
            bool success = false;

            try
            {
                var objectItem = await _uow.ObjectItems.GetAsync(codeSuid);
                var owners = await _uow.Owners.GetAllAsync();
                var owner = owners.FirstOrDefault(a => a.Name == ownerName);

                if (objectItem != null && owner != null)
                {
                    objectItem.Latitude = latitude;
                    objectItem.Longitude = longitude;
                    objectItem.OwnerId = owner.Id;
                    _uow.ObjectItems.Update(objectItem);

                    success = true;
                }
            }
            catch (Exception ex)
            {
            }

            return success;
        }

        //public async Task<bool> UpdateObjectItemIdsAsync(string codeSuid, int id)
        //{
        //    bool success = false;

        //    try
        //    {
        //        ObjectItem objectItem = await _uow.ObjectItems.GetAsync(codeSuid);

        //        if (objectItem != null)
        //        {
        //            objectItem.OwnerId = id;
        //            _uow.ObjectItems.Update(objectItem);
        //            await _uow.CommitAsync();

        //            success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return success;
        //}
    }
}
