using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ICriteriaCalcMethodService
    {
        Task<IEnumerable<CriteriaCalcMethod>> GetCriteriaCalcMethods();
        Task<CriteriaCalcMethod> FindCriteriaCalcMethodAsync(int id);
        EntityEntry<CriteriaCalcMethod> UpdateCriteriaCalcMethod(CriteriaCalcMethod criteriaCalcMethod);
        Task CommitChangesAsync();
        EntityEntry<CriteriaCalcMethod> DeleteCriteriaCalcMethod(int id);
        Task<bool> AddCriteriaCalcMethod(CriteriaCalcMethod criteriaCalcMethod);
        Task<bool> AddCriteriaCalcMethodRange(IEnumerable<CriteriaCalcMethod> criteriaCalcMethods);
    }
}
