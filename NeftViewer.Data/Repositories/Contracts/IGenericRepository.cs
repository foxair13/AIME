using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System.Linq.Expressions;

namespace NeftViewer.Data.Repositories.Contracts
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<EntityEntry<TModel>> Add(TModel obj);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<(object? MinValue, object? MaxValue)> GetMinMaxValuesAsync<T>(string filterPropertyName, object filterValue, string valuePropertyName);
        IQueryable<TModel> GetRangeParamValues(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, List<string> includeTableNames);
        IQueryable<T> GetUniqueItems<T>(List<(string filterPropertyName, object filterValue)> filters, string uniqueColumnName);
        IQueryable<TModel> GetItemsWithInclude(List<(string filterPropertyName, object filterValue)> filters, List<string> includeTableNames);
        Task<TModel> GetAsync(string? id);
        Task<TModel> GetAsync(int id);
        EntityEntry<TModel> Update(TModel obj);
        Task<TModel> UpdateRange(IEnumerable<TModel> objs);
        Task UpdateRange(IEnumerable<TModel> objs, string targetPropertyName);
        EntityEntry<TModel> Delete(TModel obj);
        EntityEntry<TModel> DeleteByID(int id);
        EntityEntry<TModel> DeleteByStringID(string id);
        bool DeleteRange(IEnumerable<TModel> objs);
        Task<bool> AddRange(IEnumerable<TModel> objs);
        Task AddRange(IEnumerable<TModel> objs, string propertyName);
        //Task<bool> AddRangeByCodeSuid(IEnumerable<ObjectItem> objs);
        //Task<bool> AddRangeByProperty(IEnumerable<TModel> objs, string propertyName);
    }
}
