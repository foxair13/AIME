using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<User> FindUserAsync(int? id)
        {
            return _uow.Users.GetAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {

            return await _uow.Users.GetAllAsync();

        }

        public EntityEntry<User> UpdateUser(User user)
        {
            return _uow.Users.Update(user);
        }
        public async Task<bool> AddUser(User user)
        {
            bool flag = false;
            try
            {
                await _uow.Users.Add(user);
                flag = true;
            }
            catch 
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<User> DeleteUser(int id)
        {
            var res = _uow.Users.DeleteByID(id);
            return res;
        }
    }
}