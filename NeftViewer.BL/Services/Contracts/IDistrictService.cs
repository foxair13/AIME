using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IDistrictService
    {
        Task<IEnumerable<District>> GetDistricts();
        Task<District> FindDistrictAsync(string? id);
        EntityEntry<District> UpdateDistrict(District district);
        Task CommitChangesAsync();
        EntityEntry<District> DeleteDistrict(string id);
        Task<bool> AddDistrict(District district);
        Task<bool> AddDistrictRange(IEnumerable<District> districts);
        Task CreateDistrict(string codeSUID, string name);
    }
}
