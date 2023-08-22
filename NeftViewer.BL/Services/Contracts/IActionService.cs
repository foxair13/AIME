using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IActionService
    {
        Task<IEnumerable<Data.Models.Action>> GetActions();
        Task<Data.Models.Action> FindActionAsync(int id);
        EntityEntry<Data.Models.Action> UpdateAction(Data.Models.Action action);
        Task CommitChangesAsync();
        EntityEntry<Data.Models.Action> DeleteAction(int id);
        Task<bool> AddAction(Data.Models.Action action);
    }
}
