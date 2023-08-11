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
    }
}
