using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface IIndicatorValueService
    {
        Task<IEnumerable<IndicatorValue>> GetIndicatorByCriteria(DateTime Dates, int CriteriaValue);
        Task<IEnumerable<IndicatorValue>> GetIndicatorByObject(DateTime Dates, string CodeSUID);
        Task<IndicatorValue> GetIndicatorValue(string codeSUID, DateTime dateStart, int criteriaId);
        Task<IndicatorValue> FindIndicatorValueAsync(string? id);
        Task UpdateIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3);
        Task CommitChangesAsync();
        Task<IEnumerable<IndicatorValue>> GetIndicatorRangeByObject(DateTime MinDateValue, DateTime MaxDateValue, string CodeSUID);
        Task<IEnumerable<IndicatorValue>> GetIndicatorRangeByCriteria(DateTime MinDateValue, DateTime MaxDateValue, int CriteriaValue);
        List<AggregatedResult> GetAgregateValuesAsync(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, string AgregateBy);
        EntityEntry<IndicatorValue> DeleteIndicatorValue(string id);
        Task<bool> AddIndicatorValue(IndicatorValue indicatorValue);
        Task AddIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3);
        Task<(decimal? MinValue, decimal? MaxValue, DateTime? MinDate, DateTime? MaxDate,List<DateTime> dates)> GetMinMaxValues(int CriteriaId);
        List<CalculatCriteriaResult> GetCalculatesValuesByCryteriaAsync(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters,  List<int> avgCriteriaIds, List<int> desiredCriteriaIds);
    }
}
