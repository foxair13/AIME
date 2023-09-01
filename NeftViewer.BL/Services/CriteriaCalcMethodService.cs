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
    public class CriteriaCalcMethodService : ICriteriaCalcMethodService
    {
        private readonly IUnitOfWork _uow;
        public async Task<bool> IsDuplicate(int CriteriaId)
        {
            var allCriterias = await _uow.Criterias.GetAllAsync();
            var existingRecord = allCriterias.FirstOrDefault(ar => ar.Id == CriteriaId);
            return existingRecord != null;
        }
        public CriteriaCalcMethodService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<CriteriaCalcMethod> FindCriteriaCalcMethodAsync(int id)
        {
            return _uow.CriteriaCalcMethods.GetAsync(id);
        }

        public async Task<IEnumerable<CriteriaCalcMethod>> GetCriteriaCalcMethods()
        {

            return await _uow.CriteriaCalcMethods.GetAllAsync();

        }

        public EntityEntry<CriteriaCalcMethod> UpdateCriteriaCalcMethod(CriteriaCalcMethod criteriaCalcMethod)
        {
            return _uow.CriteriaCalcMethods.Update(criteriaCalcMethod);
        }
        public async Task<bool> AddCriteriaCalcMethod(CriteriaCalcMethod criteriaCalcMethod)
        {
            var isDuplicate = await IsDuplicate(criteriaCalcMethod.CriteriaId);
          
            if (isDuplicate)
            {
                return false; 
            }
            try
            {
                await _uow.CriteriaCalcMethods.Add(criteriaCalcMethod);
                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public EntityEntry<CriteriaCalcMethod> DeleteCriteriaCalcMethod(int id)
        {
            var res = _uow.CriteriaCalcMethods.DeleteByID(id);
            return res;
        }

        public async Task<bool> AddCriteriaCalcMethodRange(IEnumerable<CriteriaCalcMethod> criteriaCalcMethod)
        {
            var res = await _uow.CriteriaCalcMethods.AddRange(criteriaCalcMethod);
            return res;
        }
    }
}
