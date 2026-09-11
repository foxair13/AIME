using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AI.SocialNetwork.Infrastructure.Repositories.Contracts;

public interface IGenericRepository<TModel> where TModel : class
{
    Task<EntityEntry<TModel>> Add(TModel obj);
    IEnumerable<TModel> GetAll();
    IQueryable<TModel> GetAllQueryable();
    Task<IEnumerable<TModel>> GetAllAsync();
    Task<TModel?> GetAsync(string id);
    Task<TModel?> GetAsync(int id);
    Task<TModel?> GetAsync(long id);
    EntityEntry<TModel> Update(TModel obj);
    EntityEntry<TModel> Delete(TModel obj);
    EntityEntry<TModel>? DeleteByID(long id);
    bool DeleteRange(IEnumerable<TModel> objs);
    Task<bool> AddRange(IEnumerable<TModel> objs);
    IQueryable<TModel> GetItemsWithInclude(List<(string filterPropertyName, object filterValue)> filters, List<string> includeTableNames);
    IQueryable<TModel> GetRangeParamValues(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, List<string> includeTableNames);
}