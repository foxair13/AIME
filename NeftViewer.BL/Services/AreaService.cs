using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _uow;
        public AreaService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Area> FindAreaAsync(string? id)
        {
            return _uow.Areas.GetAsync(id);
        }

        public async Task<IEnumerable<Area>> GetAreas()
        {
            return await _uow.Areas.GetAllAsync();
        }

        public EntityEntry<Area> UpdateArea(Area area)
        {
            return _uow.Areas.Update(area);
        }
        public async Task<bool> AddArea(Area area)
        {
            bool flag = false;
            try
            {
                await _uow.Areas.Add(area);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Area> DeleteArea(string id)
        {
            var res = _uow.Areas.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddAreaRange(IEnumerable<Area> areas)
        {
            var res = await _uow.Areas.AddRange(areas);
            return res;
        }

        public async Task CreateArea(string codeSUID, string name)
        {
            var area = new Area
            {
                CodeSUID = codeSUID,
                Name = name
            };
            AddArea(area);
        }
    }
}
