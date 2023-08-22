using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        EntityEntry<TModel> DeleteByID(int id);
        EntityEntry<TModel> DeleteByStringID(string id);
        bool DeleteRange(IEnumerable<TModel> objs);
        Task<bool> AddRange(IEnumerable<TModel> objs);
    }
}
