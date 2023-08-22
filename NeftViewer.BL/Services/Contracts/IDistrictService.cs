using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IAreaService
    {
        Task<IEnumerable<Area>> GetAreas();
        Task<Area> FindAreaAsync(string? id);
        EntityEntry<Area> UpdateArea(Area area);
        Task CommitChangesAsync();
        EntityEntry<Area> DeleteArea(string id);
        Task<bool> AddArea(Area area);
        Task<bool> AddAreaRange(IEnumerable<Area> areas);
        Task CreateArea(string codeSUID, string name);
    }
}
