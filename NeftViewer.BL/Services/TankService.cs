using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class TankService : ITankService
    {
        private readonly IUnitOfWork _uow;
        public TankService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Tank> FindTankAsync(string? id)
        {
            return _uow.Tanks.GetAsync(id);
        }

        public async Task<IEnumerable<Tank>> GetTanks()
        {

            return await _uow.Tanks.GetAllAsync();

        }

        public EntityEntry<Tank> UpdateTank(Tank tank)
        {
            return _uow.Tanks.Update(tank);
        }
        public async Task<bool> AddTank(Tank tank)
        {
            bool flag = false;
            try
            {
                await _uow.Tanks.Add(tank);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Tank> DeleteTank(string id)
        {
            var res = _uow.Tanks.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddTankRange(IEnumerable<Tank> tanks)
        {
            var res = await _uow.Tanks.AddRange(tanks);
            return res;
        }
    }
}
