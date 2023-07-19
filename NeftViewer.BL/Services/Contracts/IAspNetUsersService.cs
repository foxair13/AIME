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
        Task<IEnumerable<AspNetUsers>> GetAspNetUsers();
        Task<AspNetUsers> FindAspNetUserAsync(string? id);
        EntityEntry<AspNetUsers> UpdateAspNetUser(AspNetUsers aspNetUsers);
        Task CommitChangesAsync();
        EntityEntry<AspNetUsers> DeleteAspNetUser(string id);
        Task<bool> AddAspNetUser(AspNetUsers aspNetUsers);
    }
}
