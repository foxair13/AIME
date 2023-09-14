using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class EnergyService : IEnergyService
    {
        private readonly IUnitOfWork _uow;
        public EnergyService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Energy> FindEnergyAsync(string? id)
        {
            return _uow.Energies.GetAsync(id);
        }

        public async Task<IEnumerable<Energy>> GetEnergies()
        {

            return await _uow.Energies.GetAllAsync();

        }

        public EntityEntry<Energy> UpdateEnergy(Energy energy)
        {
            return _uow.Energies.Update(energy);
        }
        public async Task<bool> AddEnergy(Energy energy)
        {
            bool flag = false;
            try
            {
                await _uow.Energies.Add(energy);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Energy> DeleteEnergy(string id)
        {
            var res = _uow.Energies.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddEnergyRange(IEnumerable<Energy> energys)
        {
            var res = await _uow.Energies.AddRange(energys);
            return res;
        }
    }
}
