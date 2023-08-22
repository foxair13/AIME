using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ILocalityService
    {
        Task<IEnumerable<Locality>> GetLocalities();
        Task<Locality> FindLocalitiesAsync(string? id);
        EntityEntry<Locality> UpdateLocality(Locality locality);
        Task CommitChangesAsync();
        EntityEntry<Locality> DeleteLocality(string id);
        Task<bool> AddLocality(Locality locality);
        Task<bool> AddLocalityRange(IEnumerable<Locality> localities);
        Task CreateLocality(string codeSUID, string name);
    }
}
