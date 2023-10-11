using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public virtual async Task<TModel> UpdateRange(IEnumerable<TModel> objs)
        {
            _dbContext.Set<TModel>().UpdateRange(objs);
            _dbContext.SaveChanges();
            return null;
        }

        public virtual async Task UpdateRange(IEnumerable<TModel> objs, string targetPropertyName)
        {
            var targetProperty = typeof(TModel).GetProperty(targetPropertyName);
            var updateObjs = new List<TModel>();
            var properties = objs.FirstOrDefault().GetType().GetProperties();
            foreach (var obj in objs)
            {
                var parameter = Expression.Parameter(typeof(TModel), "o");
                var left = Expression.Property(parameter, targetProperty);
                var right = Expression.Constant(targetProperty.GetValue(obj));
                var body = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<TModel, bool>>(body, parameter);

                var updateObj = _dbContext.Set<TModel>().FirstOrDefault(lambda);

                if (updateObj != null)
                {
                    foreach (var property in properties)
                    {
                        if (property.Name != "Id" && property.Name != targetPropertyName)
                        {
                            property.SetValue(updateObj, property.GetValue(obj));
                        }
                    }
                    updateObjs.Add(updateObj);
                }
            }
            try
            {
                _dbContext.Set<TModel>().UpdateRange(updateObjs);
            }
            catch (Exception ex) { }
            _dbContext.SaveChanges();
        }

        public virtual async Task UpdateRange(IEnumerable<TModel> objs, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3)
        {
            var updateObjs = new List<TModel>();
            var properties = objs.FirstOrDefault().GetType().GetProperties();
            foreach (var obj in objs)
            {
                var parameter = Expression.Parameter(typeof(TModel), "o");

                var left1 = Expression.Property(parameter, targetPropertyName1);
                var right1 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName1).GetValue(obj));
                var equal1 = Expression.Equal(left1, right1);

                var left2 = Expression.Property(parameter, targetPropertyName2);
                var right2 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName2).GetValue(obj));
                var equal2 = Expression.Equal(left2, right2);

                var left3 = Expression.Property(parameter, targetPropertyName3);
                var right3 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName3).GetValue(obj));
                var equal3 = Expression.Equal(left3, right3);

                var body = Expression.AndAlso(Expression.AndAlso(equal1, equal2), equal3);
                var lambda = Expression.Lambda<Func<TModel, bool>>(body, parameter);

                var updateObj = _dbContext.Set<TModel>().FirstOrDefault(lambda);
                
                updateObjs.Add(updateObj);

                if (updateObj != null)
                {
                    foreach (var property in properties)
                    {
                        if (property.Name != "Id" && property.Name != targetPropertyName1 && property.Name != targetPropertyName2 && property.Name != targetPropertyName3)
                        {
                            property.SetValue(updateObj, property.GetValue(obj));
                        }
                    }
                    updateObjs.Add(updateObj);
                }
            }
            try
            {
                _dbContext.Set<TModel>().UpdateRange(updateObjs);
            }
            catch (Exception ex) { }
            _dbContext.SaveChanges();
        }

        //public virtual async Task UpdateRange(IEnumerable<TModel> objs, string targetPropertyName)
        //{
        //    var targetProperty = typeof(TModel).GetProperty(targetPropertyName);
        //    const int batchSize = 10000;

        //    var allTargetValues = objs.Select(o => targetProperty.GetValue(o)).ToList();
        //    int totalObjects = allTargetValues.Count;

        //    for (int i = 0; i < totalObjects; i += batchSize)
        //    {
        //        var currentBatchValues = allTargetValues.Skip(i).Take(batchSize).ToList();

        //        var itemsToUpdate = await _dbContext.Set<TModel>().Where(item => currentBatchValues.Contains(targetProperty.GetValue(item))).ToListAsync();

        //        foreach (var updateObj in itemsToUpdate)
        //        {
        //            var targetValue = targetProperty.GetValue(updateObj);
        //            var correspondingObj = objs.FirstOrDefault(obj => targetProperty.GetValue(obj).Equals(targetValue));

        //            if (correspondingObj != null)
        //            {
        //                var properties = correspondingObj.GetType().GetProperties();
        //                foreach (var property in properties)
        //                {
        //                    if (property.Name != "Id" && property.Name != targetPropertyName)
        //                    {
        //                        property.SetValue(updateObj, property.GetValue(correspondingObj));
        //                    }
        //                }
        //            }
        //        }

        //        _dbContext.Set<TModel>().UpdateRange(itemsToUpdate);
        //        await _dbContext.SaveChangesAsync();
        //    }
        //}


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

        public IQueryable<TModel> GetRangeParamValues(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, List<string> includeTableNames)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            Expression filterExpression = null;

            foreach (var filter in filters)
            {
                var filterProperty = Expression.Property(parameter, filter.filterPropertyName);
                var constant = Expression.Constant(filter.filterValue);
                Expression comparisonExpression = null;
                if (filter.comparisonOperator == ">=")
                {
                    comparisonExpression = Expression.GreaterThanOrEqual(filterProperty, constant);
                }
                else if (filter.comparisonOperator == "<=")
                {
                    comparisonExpression = Expression.LessThanOrEqual(filterProperty, constant);
                }
                else
                {
                    comparisonExpression = Expression.Equal(filterProperty, constant);
                }

                if (filterExpression == null)
                {
                    filterExpression = comparisonExpression;
                }
                else
                {
                    filterExpression = Expression.And(filterExpression, comparisonExpression);
                }
            }

            var filterLambda = Expression.Lambda<Func<TModel, bool>>(filterExpression, parameter);
            var query = _dbContext.Set<TModel>().Where(filterLambda);
         
            foreach (var tableName in includeTableNames)
            {
                query = query.Include(tableName);
            }
            return query;
        }

        public IQueryable<TModel> GetItemsWithInclude(List<(string filterPropertyName, object filterValue)> filters, List<string> includeTableNames)
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

            var query = _dbContext.Set<TModel>().Where(filterLambda);

            // Добавляем Include для указанных таблиц
            foreach (var tableName in includeTableNames)
            {
                query = query.Include(tableName);
            }

            return query;
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
            try
            {
                var currentItems = await _dbContext.Set<TModel>().ToListAsync();
                var uniqueItems = objs.Where(obj => !currentItems.Contains(obj)).ToList();

                if (uniqueItems.Any())
                {
                    await _dbContext.Set<TModel>().AddRangeAsync(uniqueItems);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public virtual async Task AddRange(IEnumerable<TModel> objs, string targetPropertyName)
        {
            var targetProperty = typeof(TModel).GetProperty(targetPropertyName);
            var insertObjs = new List<TModel>();

            foreach (var obj in objs)
            {
                var parameter = Expression.Parameter(typeof(TModel), "o");
                var left = Expression.Property(parameter, targetProperty);
                var right = Expression.Constant(targetProperty.GetValue(obj));
                var body = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<TModel, bool>>(body, parameter);

                var existingObj = _dbContext.Set<TModel>().FirstOrDefault(lambda);

                if (existingObj == null)
                {
                    insertObjs.Add(obj);
                }
            }
            try
            {
                _dbContext.Set<TModel>().AddRange(insertObjs);
            }
            catch (System.Exception ex) { }
            _dbContext.SaveChanges();
        }

        public virtual async Task AddRange(IEnumerable<TModel> indicatorValues, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3)
        {
            var insertObjs = new List<TModel>();

            foreach (var obj in indicatorValues)
            {
                var parameter = Expression.Parameter(typeof(TModel), "o");

                var left1 = Expression.Property(parameter, targetPropertyName1);
                var right1 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName1).GetValue(obj));
                var equal1 = Expression.Equal(left1, right1);

                var left2 = Expression.Property(parameter, targetPropertyName2);
                var right2 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName2).GetValue(obj));
                var equal2 = Expression.Equal(left2, right2);

                var left3 = Expression.Property(parameter, targetPropertyName3);
                var right3 = Expression.Constant(typeof(TModel).GetProperty(targetPropertyName3).GetValue(obj));
                var equal3 = Expression.Equal(left3, right3);

                var body = Expression.AndAlso(Expression.AndAlso(equal1, equal2), equal3);
                var lambda = Expression.Lambda<Func<TModel, bool>>(body, parameter);

                var existingObj = _dbContext.Set<TModel>().FirstOrDefault(lambda);

                if (existingObj == null)
                {
                    insertObjs.Add(obj);
                }
            }

            try
            {
                _dbContext.Set<TModel>().AddRange(insertObjs);
            }
            catch (System.Exception ex)
            {
                
            }
            await _dbContext.SaveChangesAsync();
        }


        //public virtual async Task<bool> AddRangeByProperty(IEnumerable<TModel> objs, string propertyName)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        var prop = typeof(TModel).GetProperty(propertyName);
        //        var newKeys = objs.Select(o => prop.GetValue(o)).ToList();
        //        var allObjects = await _dbContext.Set<TModel>().AsNoTracking().ToListAsync();
        //        var existingKeys = allObjects.Where(o => newKeys.Contains(prop.GetValue(o))).Select(o => prop.GetValue(o)).ToList();
        //        var uniqueItems = objs.Where(o => !existingKeys.Contains(prop.GetValue(o))).ToList();

        //        if (uniqueItems.Any())
        //        {
        //            _dbContext.Set<TModel>().AddRange(uniqueItems);
        //            await _dbContext.SaveChangesAsync();
        //        }

        //        flag = true;
        //    }
        //    catch (System.Exception ex)
        //    {

        //    }
        //    return flag;
        //}

        //public virtual async Task<bool> AddRange(IEnumerable<TModel> objs)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        var currentItems = await _dbContext.Set<TModel>().ToListAsync();
        //        var uniqueItems = new List<TModel>();

        //        foreach (var obj in objs)
        //        {
        //            try
        //            {
        //                if (!currentItems.Any(existingObj => JsonSerializer.Serialize(existingObj) == JsonSerializer.Serialize(obj)))
        //                {
        //                    uniqueItems.Add(obj);
        //                    _dbContext.Set<TModel>().Add(obj);
        //                    try
        //                    {
        //                       await _dbContext.SaveChangesAsync();
        //                    }
        //                    catch (System.Exception ex)
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //            catch (System.Exception ex)
        //            {
        //                continue;
        //            }
        //        }

        //        flag = true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //    }
        //    return flag;
        //}
    }
}
