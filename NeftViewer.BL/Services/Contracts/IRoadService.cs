using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IRoadService
    {
        Task<IEnumerable<Road>> GetRoads();
        Task<Road> FindRoadAsync(string? id);
        EntityEntry<Road> UpdateRoad(Road road);
        Task CommitChangesAsync();
        EntityEntry<Road> DeleteRoad(string id);
        Task<bool> AddRoad(Road road);
        Task<bool> AddRoadRange(IEnumerable<Road> roads, string connectionstring);
    }
}
