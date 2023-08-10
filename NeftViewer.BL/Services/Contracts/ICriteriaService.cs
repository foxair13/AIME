using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ICriteriaService
    {
        Task<IEnumerable<Criteria>> GetCriterias();
        Task<Criteria> FindCriteriaAsync(string? id);
        EntityEntry<Criteria> UpdateCriteria(Criteria criteria);
        Task CommitChangesAsync();
        EntityEntry<Criteria> DeleteCriteria(string id);
        Task<bool> AddCriteria(Criteria criteria);
        Task<bool> AddCriteriaRange(IEnumerable<Criteria> criterias);
    }
}
