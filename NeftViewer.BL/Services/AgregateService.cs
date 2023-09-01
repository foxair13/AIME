using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class AgregateService : IAgregateService
    {
        private readonly IUnitOfWork _uow;
        public AgregateService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Agregate> FindAgregateAsync(int id)
        {
            return _uow.Agregates.GetAsync(id);
        }

        public async Task<IEnumerable<Agregate>> GetAgregates()
        {
            return await _uow.Agregates.GetAllAsync();
        }

        public EntityEntry<Agregate> UpdateAgregate(Agregate agregate)
        {
            return _uow.Agregates.Update(agregate);
        }
        public async Task<bool> AddAgregate(Agregate agregate)
        {
            bool flag = false;
            try
            {
                await _uow.Agregates.Add(agregate);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Agregate> DeleteAgregate(int id)
        {
            var res = _uow.Agregates.DeleteByID(id);
            return res;
        }

        public async Task<bool> AddAgregateRange(IEnumerable<Agregate> agregates)
        {
            var res = await _uow.Agregates.AddRange(agregates);
            return res;
        }
    }
}
