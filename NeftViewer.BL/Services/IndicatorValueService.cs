using DocumentFormat.OpenXml.Spreadsheet;
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
        public async Task<(decimal? MinValue, decimal? MaxValue,DateTime? MinDate,DateTime? MaxDate)> GetMinMaxValues(int CriteriaId)
        {
          
            var value = await _uow.IndicatorValues.GetMinMaxValuesAsync<decimal?>("CriteriaId", CriteriaId, "Value");
            var date = await _uow.IndicatorValues.GetMinMaxValuesAsync<DateTime?>("CriteriaId", CriteriaId, "DateStart");
            decimal? minValue = value.MinValue as decimal?;
            decimal? maxValue = value.MaxValue as decimal?;
            DateTime? minDate = date.MinValue as DateTime?;
            DateTime? maxDate = date.MaxValue as DateTime?;
            return (minValue, maxValue, minDate, maxDate);
        }

 
      
        public Task<IndicatorValue> FindIndicatorValueAsync(string? id)
        {
            return _uow.IndicatorValues.GetAsync(id);
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorValues(DateTime Dates,  int CriteriaValue)
        {
            DateTime dateInput = Dates;
            DateTimeOffset dateWithOffset = new DateTimeOffset(dateInput, new TimeSpan(2, 0, 0));
            DateTime dateForFilter = dateWithOffset.UtcDateTime;
            List<(string filterPropertyName, object filterValue)> filters = new List<(string, object)>();
            filters.Add(new ValueTuple<string, object>("DateStart", dateForFilter));
            filters.Add(new ValueTuple<string, object>("CriteriaId", CriteriaValue));
            var v = _uow.IndicatorValues.GetItems(filters);
            return v.ToList();
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
