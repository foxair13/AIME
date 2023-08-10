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
    public class ActionRoleService : IActionRoleService
    {
        private readonly IUnitOfWork _uow;
        public ActionRoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<ActionRole> FindActionRoleAsync(string? id)
        {
            return _uow.ActionRole.GetAsync(id);
        }

        public async Task<IEnumerable<ActionRole>> GetActionRoles()
        {

            return await _uow.ActionRole.GetAllAsync();

        }

        public EntityEntry<ActionRole> UpdateActionRole(ActionRole actionRole)
        {
            return _uow.ActionRole.Update(actionRole);
        }
        public async Task<bool> AddActionRole(ActionRole actionRole)
        {
            bool flag = false;
            try
            {
                await _uow.ActionRole.Add(actionRole);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<ActionRole> DeleteActionRole(string id)
        {
            var res = _uow.ActionRole.DeleteByStringID(id);
            return res;
        }
    }
}
