using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services
{
    public class CriteriaService : ICriteriaService
    {
        private readonly IUnitOfWork _uow;
        public CriteriaService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Criteria> FindCriteriaAsync(int id)
        {
            return _uow.Criterias.GetAsync(id);
        }

        public async Task<IEnumerable<Criteria>> GetCriterias()
        {
            return await _uow.Criterias.GetAllAsync();
        }

        public EntityEntry<Criteria> UpdateCriteria(Criteria criteria)
        {
            return _uow.Criterias.Update(criteria);
        }

        public async Task<bool> AddCriteria(Criteria criteria)
        {
            bool flag = false;
            try
            {
                await _uow.Criterias.Add(criteria);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public EntityEntry<Criteria> DeleteCriteria(string id)
        {
            var res = _uow.Criterias.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddCriteriaRange(IEnumerable<Criteria> criterias)
        {
            var res = await _uow.Criterias.AddRange(criterias);
            return res;
        }
    }
}
