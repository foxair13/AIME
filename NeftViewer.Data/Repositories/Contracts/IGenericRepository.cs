using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.Contracts
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<EntityEntry<TModel>> Add(TModel obj);

        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel> GetAsync(string? id);
        Task<TModel> GetAsync(int id);
        EntityEntry<TModel> Update(TModel obj);

        EntityEntry<TModel> Delete(TModel obj);
        EntityEntry<TModel> DeleteByID(string id);
        bool DeleteRange(IEnumerable<TModel> objs);
        bool AddRange(IEnumerable<TModel> objs);
    }
}
