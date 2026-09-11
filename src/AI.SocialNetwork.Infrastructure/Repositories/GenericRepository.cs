using AI.SocialNetwork.Infrastructure.DataContext;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace AI.SocialNetwork.Infrastructure.Repositories;

public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
{
    private readonly AISocialNetworkContext _dbContext;

    public GenericRepository(AISocialNetworkContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EntityEntry<TModel>> Add(TModel obj)
    {
        var res = await _dbContext.Set<TModel>().AddAsync(obj);
        return res;
    }

    public virtual IQueryable<TModel> GetAllQueryable()
    {
        return _dbContext.Set<TModel>().AsQueryable();
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync()
    {
        return await _dbContext.Set<TModel>().ToListAsync();
    }

    public virtual IEnumerable<TModel> GetAll()
    {
        return _dbContext.Set<TModel>().ToList();
    }

    public virtual async Task<TModel?> GetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        return await _dbContext.Set<TModel>().FindAsync(id);
    }

    public virtual async Task<TModel?> GetAsync(int id)
    {
        return await _dbContext.Set<TModel>().FindAsync(id);
    }

    public virtual async Task<TModel?> GetAsync(long id)
    {
        return await _dbContext.Set<TModel>().FindAsync(id);
    }

    public virtual EntityEntry<TModel> Update(TModel obj)
    {
        return _dbContext.Set<TModel>().Update(obj);
    }

    public virtual EntityEntry<TModel> Delete(TModel obj)
    {
        return _dbContext.Set<TModel>().Remove(obj);
    }

    public virtual EntityEntry<TModel>? DeleteByID(long id)
    {
        var obj = _dbContext.Set<TModel>().Find(id);
        if (obj != null)
        {
            return _dbContext.Set<TModel>().Remove(obj);
        }
        return null;
    }

    public virtual bool DeleteRange(IEnumerable<TModel> objs)
    {
        try
        {
            _dbContext.Set<TModel>().RemoveRange(objs);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public virtual async Task<bool> AddRange(IEnumerable<TModel> objs)
    {
        try
        {
            await _dbContext.Set<TModel>().AddRangeAsync(objs);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IQueryable<TModel> GetItemsWithInclude(List<(string filterPropertyName, object filterValue)> filters, List<string> includeTableNames)
    {
        var query = ApplyFilters(GetAllQueryable(), filters);

        foreach (var tableName in includeTableNames)
        {
            query = query.Include(tableName);
        }
        return query;
    }

    public IQueryable<TModel> GetRangeParamValues(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, List<string> includeTableNames)
    {
        var parameter = Expression.Parameter(typeof(TModel), "x");
        Expression? filterExpression = null;

        foreach (var filter in filters)
        {
            var filterProperty = Expression.Property(parameter, filter.filterPropertyName);
            var constant = Expression.Constant(filter.filterValue);
            Expression comparisonExpression = filter.comparisonOperator switch
            {
                ">=" => Expression.GreaterThanOrEqual(filterProperty, constant),
                "<=" => Expression.LessThanOrEqual(filterProperty, constant),
                ">" => Expression.GreaterThan(filterProperty, constant),
                "<" => Expression.LessThan(filterProperty, constant),
                _ => Expression.Equal(filterProperty, constant)
            };

            if (filterExpression == null)
            {
                filterExpression = comparisonExpression;
            }
            else
            {
                filterExpression = Expression.AndAlso(filterExpression, comparisonExpression);
            }
        }

        if (filterExpression != null)
        {
            var filterLambda = Expression.Lambda<Func<TModel, bool>>(filterExpression, parameter);
            var query = _dbContext.Set<TModel>().Where(filterLambda);

            foreach (var tableName in includeTableNames)
            {
                query = query.Include(tableName);
            }
            return query;
        }

        return GetAllQueryable();
    }

    private static IQueryable<TModel> ApplyFilters(IQueryable<TModel> query, List<(string filterPropertyName, object filterValue)> filters)
    {
        var parameter = Expression.Parameter(typeof(TModel), "x");
        Expression? filterExpression = null;

        foreach (var filter in filters)
        {
            var filterProperty = Expression.Property(parameter, filter.filterPropertyName);
            var constant = Expression.Constant(filter.filterValue);
            var equality = Expression.Equal(filterProperty, constant);
            if (filterExpression == null)
            {
                filterExpression = equality;
            }
            else
            {
                filterExpression = Expression.AndAlso(filterExpression, equality);
            }
        }

        if (filterExpression == null)
        {
            return query;
        }

        var filterLambda = Expression.Lambda<Func<TModel, bool>>(filterExpression, parameter);
        return query.Where(filterLambda);
    }
}