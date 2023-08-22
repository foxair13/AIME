using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IObjectItemService
    {
        Task<IEnumerable<ObjectItem>> GetObjectItems();
        Task<ObjectItem> FindObjectItemAsync(string? id);
        EntityEntry<ObjectItem> UpdateObjectItem(ObjectItem objectItem);
        Task CommitChangesAsync();
        EntityEntry<ObjectItem> DeleteObjectItem(string id);
        Task<bool> AddObjectItem(ObjectItem objectItem);
        Task<bool> AddObjectItemRange(IEnumerable<ObjectItem> objectItems);
        Task<bool> UpdateObjectItemCoordinatesAsync(string codeSuid, double latitude, double longitude, string ownerName);
    }
}
