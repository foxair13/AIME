using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services
{
    public class IndicatorValueService : IIndicatorValueService
    {
        private readonly IUnitOfWork _uow;
        public IndicatorValueService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<IndicatorValue> FindIndicatorValueAsync(string? id)
        {
            return _uow.IndicatorValues.GetAsync(id);
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorValues()
        {

            return await _uow.IndicatorValues.GetAllAsync();

        }

        public EntityEntry<IndicatorValue> UpdateIndicatorValue(IndicatorValue indicatorValue)
        {
            return _uow.IndicatorValues.Update(indicatorValue);
        }
        public async Task<bool> AddIndicatorValue(IndicatorValue indicatorValue)
        {
            bool flag = false;
            try
            {
                await _uow.IndicatorValues.Add(indicatorValue);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<IndicatorValue> DeleteIndicatorValue(string id)
        {
            var res = _uow.IndicatorValues.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues)
        {
            var res = await _uow.IndicatorValues.AddRange(indicatorValues);
            return res;
        }
    }
}
