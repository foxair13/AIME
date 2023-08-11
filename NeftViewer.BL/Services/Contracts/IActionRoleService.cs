using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IActionRoleService
    {
        Task<IEnumerable<ActionRole>> GetActionRoles();
        Task<ActionRole> FindActionRoleAsync(int id);
        EntityEntry<ActionRole> UpdateActionRole(ActionRole actionRole);
        Task CommitChangesAsync();
        EntityEntry<ActionRole> DeleteActionRole(string id);
        Task<bool> AddActionRole(ActionRole actionRole);
    }
}
