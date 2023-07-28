using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IActionService
    {
        Task<IEnumerable<Data.Models.Action>> GetActions();
        Task<Data.Models.Action> FindActionAsync(string? id);
        EntityEntry<Data.Models.Action> UpdateAction(Data.Models.Action action);
        Task CommitChangesAsync();
        EntityEntry<Data.Models.Action> DeleteAction(string id);
        Task<bool> AddAction(Data.Models.Action action);
    }
}
