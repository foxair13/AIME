using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IEnergyService
    {
        Task<IEnumerable<Energy>> GetEnergies();
        Task<Energy> FindEnergyAsync(string? id);
        EntityEntry<Energy> UpdateEnergy(Energy energy);
        Task CommitChangesAsync();
        EntityEntry<Energy> DeleteEnergy(string id);
        Task<bool> AddEnergy(Energy energy);
        Task<bool> AddEnergyRange(IEnumerable<Energy> energy);
    }
}
