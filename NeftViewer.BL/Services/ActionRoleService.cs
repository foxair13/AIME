using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

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

        public Task<ActionRole> FindActionRoleAsync(int id)
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
            // Проверяем наличие дубликата
            var isDuplicate = await IsDuplicate(actionRole.ActionId, actionRole.RoleId);
            if (isDuplicate)
            {
                return false; // Возврат false, если дубликат найден
            }

            // Добавляем запись, если дубликат не обнаружен
            try
            {
                await _uow.ActionRole.Add(actionRole);
                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public EntityEntry<ActionRole> DeleteActionRole(int id)
        {
            var res = _uow.ActionRole.DeleteByID(id);
            return res;
        }
        public async Task<bool> IsDuplicate(int actionId, String roleId)
        {
            var allActionRoles = await _uow.ActionRole.GetAllAsync();
            var existingRecord = allActionRoles.FirstOrDefault(ar => ar.ActionId == actionId && ar.RoleId == roleId);
            return existingRecord != null;
        }
    }
}
