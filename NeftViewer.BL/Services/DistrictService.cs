using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _uow;
        public DistrictService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<District> FindDistrictAsync(string? id)
        {
            return _uow.Districts.GetAsync(id);
        }

        public async Task<IEnumerable<District>> GetDistricts()
        {
            return await _uow.Districts.GetAllAsync();
        }

        public EntityEntry<District> UpdateDistrict(District district)
        {
            return _uow.Districts.Update(district);
        }
        public async Task<bool> AddDistrict(District district)
        {
            bool flag = false;
            try
            {
                await _uow.Districts.Add(district);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<District> DeleteDistrict(string id)
        {
            var res = _uow.Districts.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddDistrictRange(IEnumerable<District> districts)
        {
            var res = await _uow.Districts.AddRange(districts);
            return res;
        }

        public async Task CreateDistrict(string codeSUID, string name)
        {
            var district = new District
            {
                CodeSUID = codeSUID,
                Name = name
            };
            AddDistrict(district);
        }
    }
}
