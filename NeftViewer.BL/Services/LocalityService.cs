using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class LocalityService : ILocalityService
    {
        private readonly IUnitOfWork _uow;
        public LocalityService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Locality> FindLocalitiesAsync(string? id)
        {
            return _uow.Localities.GetAsync(id);
        }

        public async Task<IEnumerable<Locality>> GetLocalities()
        {

            return await _uow.Localities.GetAllAsync();

        }

        public EntityEntry<Locality> UpdateLocality(Locality locality)
        {
            return _uow.Localities.Update(locality);
        }
        public async Task<bool> AddLocality(Locality locality)
        {
            bool flag = false;
            try
            {
                await _uow.Localities.Add(locality);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Locality> DeleteLocality(string id)
        {
            var res = _uow.Localities.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddLocalityRange(IEnumerable<Locality> localities)
        {
            var res = await _uow.Localities.AddRange(localities);
            return res;
        }

        public async Task CreateLocality(string codeSUID, string name)
        {
            var locality = new Locality
            {
                CodeSUID = codeSUID,
                Name = name
            };
            AddLocality(locality);
        }
    }
}
