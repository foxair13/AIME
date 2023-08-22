using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetOwners();
        Task<Owner> FindOwnerAsync(string? id);
        EntityEntry<Owner> UpdateOwner(Owner owner);
        Task CommitChangesAsync();
        EntityEntry<Owner> DeleteOwner(string id);
        Task<bool> AddOwner(Owner owner);
        Task<bool> AddOwnerRange(IEnumerable<Owner> owners);
        Task CreateOwner(string codeSUID, string name);
    }
}
