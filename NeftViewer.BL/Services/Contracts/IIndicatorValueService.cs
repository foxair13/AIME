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
        Task<IEnumerable<IndicatorValue>> GetIndicatorValues(DateTime Dates, int CriteriaValue);
        Task<IndicatorValue> FindIndicatorValueAsync(string? id);
        EntityEntry<IndicatorValue> UpdateIndicatorValue(IndicatorValue indicatorValue);
        Task CommitChangesAsync();
        EntityEntry<IndicatorValue> DeleteIndicatorValue(string id);
        Task<bool> AddIndicatorValue(IndicatorValue indicatorValue);
        Task<bool> AddIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues);
        Task<(decimal? MinValue, decimal? MaxValue, DateTime? MinDate, DateTime? MaxDate,List<DateTime> dates)> GetMinMaxValues(int CriteriaId);

    }
}
