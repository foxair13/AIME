using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace NeftViewer.Data.Repositories.Contracts
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<EntityEntry<TModel>> Add(TModel obj);

        Task<IEnumerable<TModel>> GetAllAsync();
        Task<(object? MinValue, object? MaxValue)> GetMinMaxValuesAsync<T>(string filterPropertyName, object filterValue, string valuePropertyName);
        IQueryable<TModel> GetItems(List<(string filterPropertyName, object filterValue)> filters);
        Task<TModel> GetAsync(string? id);
        Task<TModel> GetAsync(int id);
        EntityEntry<TModel> Update(TModel obj);

        EntityEntry<TModel> Delete(TModel obj);
        EntityEntry<TModel> DeleteByID(int id);
        EntityEntry<TModel> DeleteByStringID(string id);
        bool DeleteRange(IEnumerable<TModel> objs);
        Task<bool> AddRange(IEnumerable<TModel> objs);
    }
}
