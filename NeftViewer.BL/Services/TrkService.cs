using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;

namespace NeftViewer.BL.Services
{
    public class TrkService : ITrkService
    {
        private readonly IUnitOfWork _uow;
        public TrkService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Trk> FindTrkAsync(string? id)
        {
            return _uow.Trks.GetAsync(id);
        }

        public async Task<IEnumerable<Trk>> GetTrks()
        {
            return await _uow.Trks.GetAllAsync();
        }

        public EntityEntry<Trk> UpdateTrk(Trk trk)
        {
            return _uow.Trks.Update(trk);
        }

        public async Task<bool> AddTrk(Trk trk)
        {
            bool flag = false;
            try
            {
                await _uow.Trks.Add(trk);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Trk> DeleteTrk(string id)
        {
            var res = _uow.Trks.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddTrkRange(IEnumerable<Trk> trks)
        {
            var res = await _uow.Trks.AddRange(trks);
            return res;
        }
    }
}
