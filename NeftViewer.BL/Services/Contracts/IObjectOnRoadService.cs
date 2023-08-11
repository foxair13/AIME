using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IObjectOnRoadService
    {
        Task<IEnumerable<ObjectOnRoad>> GetObjectOnRoads();
        Task<ObjectOnRoad> FindObjectOnRoadAsync(string? id);
        EntityEntry<ObjectOnRoad> UpdateObjectOnRoad(ObjectOnRoad objectOnRoad);
        Task CommitChangesAsync();
        EntityEntry<ObjectOnRoad> DeleteObjectOnRoad(string id);
        Task<bool> AddObjectOnRoad(ObjectOnRoad objectOnRoad);
        Task<bool> AddObjectOnRoadRange(IEnumerable<ObjectOnRoad> objectOnRoads);
    }
}
