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
    public class ActionService : IActionService
    {
        private readonly IUnitOfWork _uow;
        public ActionService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Data.Models.Action> FindActionAsync(int id)
        {
           
                return _uow.Action.GetAsync(id);

        }

        public async Task<IEnumerable<Data.Models.Action>> GetActions()
        {

            return await _uow.Action.GetAllAsync();

        }

        public EntityEntry<Data.Models.Action> UpdateAction(Data.Models.Action action)
        {
            return _uow.Action.Update(action);
        }
        public async Task<bool> AddAction(Data.Models.Action action)
        {
            bool flag = false;
            try
            {
                await _uow.Action.Add(action);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<Data.Models.Action> DeleteAction(int id)
        {
            var res = _uow.Action.DeleteByID(id);
            return res;
        }
    }
}
