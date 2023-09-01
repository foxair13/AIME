using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IAgregateService
    {
        Task<IEnumerable<Agregate>> GetAgregates();
        Task<Agregate> FindAgregateAsync(int id);
        EntityEntry<Agregate> UpdateAgregate(Agregate agregate);
        Task CommitChangesAsync();
        EntityEntry<Agregate> DeleteAgregate(int id);
        Task<bool> AddAgregate(Agregate agregate);
        Task<bool> AddAgregateRange(IEnumerable<Agregate> agregates);

    }
}
