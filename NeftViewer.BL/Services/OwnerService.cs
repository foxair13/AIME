using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IUnitOfWork _uow;
        public OwnerService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Owner> FindOwnerAsync(string? id)
        {
            return _uow.Owners.GetAsync(id);
        }

        public async Task<IEnumerable<Owner>> GetOwners()
        {
            return await _uow.Owners.GetAllAsync();
        }

        public EntityEntry<Owner> UpdateOwner(Owner owner)
        {
            return _uow.Owners.Update(owner);
        }
        public async Task<bool> AddOwner(Owner owner)
        {
            bool flag = false;
            try
            {
                await _uow.Owners.Add(owner);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Owner> DeleteOwner(string id)
        {
            var res = _uow.Owners.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddOwnerRange(IEnumerable<Owner> owners)
        {
            var res = await _uow.Owners.AddRange(owners);
            return res;
        }

        public async Task CreateOwner(string codeSUID, string name)
        {
            var owners = await _uow.Owners.GetAllAsync();
            if(!owners.Any(x => x.Name == name))
            {
                var owner = new Owner
                {
                    Name = name
                };
                try
                {
                    await _uow.Owners.Add(owner);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
