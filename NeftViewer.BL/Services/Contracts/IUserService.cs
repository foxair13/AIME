using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> FindUserAsync(int? id);
        EntityEntry<User> UpdateUser(User user);
        Task CommitChangesAsync();
        EntityEntry<User> DeleteUser(int id);
        Task<bool> AddUser(User user);
    }
}
