using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public virtual async Task<TModel> GetAsync(string? id)
        {
            if (id!="")
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
        public virtual EntityEntry<TModel> DeleteByID(string id)
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
        public virtual async Task<bool> AddRange(IEnumerable<TModel> objs,string connectionString)
        {
            try
            {
                using (var dbContext = new NeftViewerContext(connectionString))
                {
                    var currentItems = await dbContext.Set<TModel>().ToListAsync();
                    var uniqueItems = new List<TModel>();

                    foreach (var obj in objs)
                    {
                        if (!currentItems.Any(existingObj => JsonSerializer.Serialize(existingObj) == JsonSerializer.Serialize(obj)))
                        {
                            uniqueItems.Add(obj);
                            dbContext.Set<TModel>().Add(obj);
                        }
                    }

                   await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }


        }
    }
}
