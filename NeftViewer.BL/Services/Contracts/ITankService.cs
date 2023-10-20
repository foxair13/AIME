using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ITankService
    {
        Task<IEnumerable<Tank>> GetTanks();
        Task<Tank> FindTankAsync(string? id);
        EntityEntry<Tank> UpdateTank(Tank tank);
        Task CommitChangesAsync();
        EntityEntry<Tank> DeleteTank(string id);
        Task<bool> AddTank(Tank tank);
        Task<bool> AddTankRange(IEnumerable<Tank> tank);
    }
}
