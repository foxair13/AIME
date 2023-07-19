using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL
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

        public Task<AspNetUsers> FindAspNetUserAsync(string? id)
        {
            return _uow.AspNetUsers.GetAsync(id);
        }

        public async Task<IEnumerable<AspNetUsers>> GetAspNetUsers()
        {

            return await _uow.AspNetUsers.GetAllAsync();

        }

        public EntityEntry<AspNetUsers> UpdateAspNetUser(AspNetUsers aspNetUsers)
        {
            return _uow.AspNetUsers.Update(aspNetUsers);
        }
        public async Task<bool> AddAspNetUser(AspNetUsers aspNetUsers)
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


        public EntityEntry<AspNetUsers> DeleteAspNetUser(string id)
        {
            var res = _uow.AspNetUsers.DeleteByID(id);
            return res;
        }
    }
}