using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using System.Linq.Expressions;
using System.Text.Json;

namespace NeftViewer.Data.Repositories
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        private readonly NeftViewerContext _dbContext;

        public GenericRepository(NeftViewerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntityEntry<TModel>> Add(TModel obj)
        {
            var res = await _dbContext.Set<TModel>().AddAsync(obj);
            _dbContext.SaveChanges();
            return res;

        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbContext.Set<TModel>().ToListAsync();
        }
        public virtual async Task<TModel> GetAsync(string id)
        {
            if (id != "")
            {
                return await _dbContext.Set<TModel>().FindAsync(id);
            }
            return null;
        }
        public virtual async Task<TModel> GetAsync(int id)
        {
            if (id != null)
            {
                return await _dbContext.Set<TModel>().FindAsync(id);
            }
            return null;
        }

        public virtual EntityEntry<TModel> Update(TModel obj)
        {
            var res = _dbContext.Set<TModel>().Update(obj);
            _dbContext.SaveChanges();
            return res;
        }

        public virtual EntityEntry<TModel> Delete(TModel obj)
        {
            var res = _dbContext.Set<TModel>().Remove(obj);
            _dbContext.SaveChanges();
            return res;
        }
        public virtual EntityEntry<TModel> DeleteByID(int id)
        {
            var obj = _dbContext.Set<TModel>().Find(id);
            if (obj != null)
            {
                var res = _dbContext.Set<TModel>().Remove(obj);
                _dbContext.SaveChanges();
                return res;
            }
            return null;
        }
        public virtual EntityEntry<TModel> DeleteByStringID(string id)
        {
            var obj = _dbContext.Set<TModel>().Find(id);
            if (obj != null)
            {
                var res = _dbContext.Set<TModel>().Remove(obj);
                _dbContext.SaveChanges();
                return res;
            }
            return null;
        }
        public virtual bool DeleteRange(IEnumerable<TModel> objs)
        {
            foreach (var obj in objs)
            {
                _dbContext.Set<TModel>().Remove(obj);
            }
            try
            {
                _dbContext.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<(object? MinValue, object? MaxValue)> GetMinMaxValuesAsync<T>(string filterPropertyName, object filterValue, string valuePropertyName)
        {
            // Формирование выражения для выборки значения
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var valueProperty = Expression.Property(parameter, valuePropertyName);
            var nullableValueProperty = Expression.Convert(valueProperty, typeof(T));
            var valueLambda = Expression.Lambda<Func<TModel, T>>(nullableValueProperty, parameter);


            // Формирование выражения для фильтрации
            var filterProperty = Expression.Property(parameter, filterPropertyName);
            var constant = Expression.Constant(filterValue);
            var equality = Expression.Equal(filterProperty, constant);
            var filterLambda = Expression.Lambda<Func<TModel, bool>>(equality, parameter);

            var maxVal = await _dbContext.Set<TModel>()
                .Where(filterLambda)
                .Select(valueLambda)
                .MaxAsync();
            var minVal = await _dbContext.Set<TModel>()
              .Where(filterLambda)
              .Select(valueLambda)
              .MinAsync();

            return (minVal, maxVal);
        }

        public IQueryable<TModel> GetItems(List<(string filterPropertyName, object filterValue)> filters)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            Expression filterExpression = null;
            foreach (var filter in filters)
            {
                var filterProperty = Expression.Property(parameter, filter.filterPropertyName);
                var constant = Expression.Constant(filter.filterValue);
                var equality = Expression.Equal(filterProperty, constant);
                if (filterExpression == null)
                    filterExpression = equality;
                else
                    filterExpression = Expression.And(filterExpression, equality);
            }
            var filterLambda = Expression.Lambda<Func<TModel, bool>>(filterExpression, parameter);

            return _dbContext.Set<TModel>()
                .Where(filterLambda);
        }



        public IQueryable<T> GetUniqueItems<T>(List<(string filterPropertyName, object filterValue)> filters, string uniqueColumnName)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            Expression filterExpression = null;

            foreach (var filter in filters)
            {
                var filterProperty = Expression.Property(parameter, filter.filterPropertyName);
                var constant = Expression.Constant(filter.filterValue);
                var equality = Expression.Equal(filterProperty, constant);

                if (filterExpression == null)
                    filterExpression = equality;
                else
                    filterExpression = Expression.And(filterExpression, equality);
            }

            var filterLambda = Expression.Lambda<Func<TModel, bool>>(filterExpression, parameter);

            var uniqueColumnProperty = Expression.Property(parameter, uniqueColumnName);
            var boxedUniqueColumnProperty = Expression.Convert(uniqueColumnProperty, typeof(T));

            var groupByLambda = Expression.Lambda<Func<TModel, T>>(boxedUniqueColumnProperty, parameter);

            return _dbContext.Set<TModel>()
                .Where(filterLambda)
                .GroupBy(groupByLambda)
                .Select(group => group.Key);
        }



        public virtual async Task<bool> AddRange(IEnumerable<TModel> objs)
        {
            bool flag = false;
            try
            {
                var currentItems = await _dbContext.Set<TModel>().ToListAsync();
                var uniqueItems = new List<TModel>();

                foreach (var obj in objs)
                {
                    try
                    {
                        if (!currentItems.Any(existingObj => JsonSerializer.Serialize(existingObj) == JsonSerializer.Serialize(obj)))
                        {
                            uniqueItems.Add(obj);
                            _dbContext.Set<TModel>().Add(obj);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }
                }
            
                flag = true;
            }
            catch (System.Exception ex)
            {
            }
            return flag;
        }

    }
}
