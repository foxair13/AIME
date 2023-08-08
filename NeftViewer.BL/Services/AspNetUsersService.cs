using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class AspNetUsersService : IAspNetUsersService
    {
        private readonly IUnitOfWork _uow;
        public AspNetUsersService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<AspNetUser> FindAspNetUserAsync(string? id)
        {
            return _uow.AspNetUsers.GetAsync(id);
        }

        public async Task<IEnumerable<AspNetUser>> GetAspNetUsers()
        {

            return await _uow.AspNetUsers.GetAllAsync();

        }

        public EntityEntry<AspNetUser> UpdateAspNetUser(AspNetUser aspNetUsers)
        {
            return _uow.AspNetUsers.Update(aspNetUsers);
        }
        public async Task<bool> AddAspNetUser(AspNetUser aspNetUsers)
        {
            bool flag = false;
            try
            {
                await _uow.AspNetUsers.Add(aspNetUsers);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<AspNetUser> DeleteAspNetUser(string id)
        {
            var res = _uow.AspNetUsers.DeleteByID(id);
            return res;
        }
    }
}