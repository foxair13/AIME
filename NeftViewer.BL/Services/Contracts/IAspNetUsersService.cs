using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IAspNetUsersService
    {
        Task<IEnumerable<AspNetUser>> GetAspNetUsers();
        Task<AspNetUser> FindAspNetUserAsync(string? id);
        EntityEntry<AspNetUser> UpdateAspNetUser(AspNetUser aspNetUsers);
        Task CommitChangesAsync();
        EntityEntry<AspNetUser> DeleteAspNetUser(string id);
        Task<bool> AddAspNetUser(AspNetUser aspNetUsers);
    }
}
