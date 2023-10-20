using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ITrkService
    {
        Task<IEnumerable<Trk>> GetTrks();
        Task<Trk> FindTrkAsync(string? id);
        EntityEntry<Trk> UpdateTrk(Trk trk);
        Task CommitChangesAsync();
        EntityEntry<Trk> DeleteTrk(string id);
        Task<bool> AddTrk(Trk trk);
        Task<bool> AddTrkRange(IEnumerable<Trk> trk);
    }
}
