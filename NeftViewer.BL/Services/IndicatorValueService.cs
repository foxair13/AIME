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
        public async Task<(decimal? MinValue, decimal? MaxValue,DateTime? MinDate,DateTime? MaxDate,List<DateTime> dates)> GetMinMaxValues(int CriteriaId)
        {
          
            var value = await _uow.IndicatorValues.GetMinMaxValuesAsync<decimal?>("CriteriaId", CriteriaId, "Value");
            var date = await _uow.IndicatorValues.GetMinMaxValuesAsync<DateTime?>("CriteriaId", CriteriaId, "DateStart");
            List<(string filterPropertyName, object filterValue)> filters = new List<(string, object)>();
            filters.Add(new ValueTuple<string, object>("CriteriaId", CriteriaId));
            try
            {
                var dates = _uow.IndicatorValues.GetUniqueItems<DateTime>(filters, "DateStart").ToList();

                decimal? minValue = value.MinValue as decimal?;
                decimal? maxValue = value.MaxValue as decimal?;
                DateTime? minDate = date.MinValue as DateTime?;
                DateTime? maxDate = date.MaxValue as DateTime?;
                return (minValue, maxValue, minDate, maxDate, dates);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
      
        public Task<IndicatorValue> FindIndicatorValueAsync(string? id)
        {
            return _uow.IndicatorValues.GetAsync(id);
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorByCriteria(DateTime Dates,  int CriteriaValue)
        {
            DateTime dateInput = Dates;

            List<(string filterPropertyName, object filterValue)> filters = new List<(string, object)>();
            List<string> includs = new List<string>();
            filters.Add(new ValueTuple<string, object>("DateStart", Dates));
            filters.Add(new ValueTuple<string, object>("CriteriaId", CriteriaValue));
           
            var v = _uow.IndicatorValues.GetItemsWithInclude(filters, includs);
            return v.ToList();
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorRangeByCriteria(DateTime MinDateValue, DateTime MaxDateValue, int CriteriaValue)
        {
            var filters = new List<(string filterPropertyName, object filterValue, string comparisonOperator)>
                                            {
                                             ("DateStart", MinDateValue, ">="),
                                             ("DateStart", MaxDateValue, "<="),
                                                ("CriteriaId", CriteriaValue, ""),
                                            };
            List<string> includs = new List<string>();
   
            includs.Add("Criterias");
            includs.Add("Objects");


            var v = _uow.IndicatorValues.GetRangeParamValues(filters, includs);
            return v.ToList();
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorRangeByObject(DateTime MinDateValue, DateTime MaxDateValue, string CodeSUID)
        {
            var filters = new List<(string filterPropertyName, object filterValue, string comparisonOperator)>
                                            {
                                             ("DateStart", MinDateValue, ">="),
                                             ("DateStart", MaxDateValue, "<="),
                                                ("CodeSUID", CodeSUID, ""),
                                            };
            List<string> includs = new List<string>();

            includs.Add("Criterias");
            includs.Add("Objects");


            var v = _uow.IndicatorValues.GetRangeParamValues(filters, includs);
            return v.ToList();
        }

        public List<AggregatedResult> GetAgregateValuesAsync(List<(string filterPropertyName, object filterValue, string comparisonOperator)> filters, string AgregateBy)
        {
            List<string> includs = new List<string>();

            includs.Add("Criterias");
            includs.Add("Objects");
            var query = _uow.IndicatorValues.GetRangeParamValues(filters, includs);

            List<AggregatedResult> result = null;

            switch (AgregateBy)
            {
                case "Max":
                    result = query.GroupBy(x => x.CodeSUID)
                        .Select(g => new AggregatedResult
                        {
                            CodeSUID = g.Key,
                            AggregatedValue = g.Max(x => x.Value)
                        })
                        .ToList();
                    break;
                case "Min":
                    result = query.GroupBy(x => x.CodeSUID)
                        .Select(g => new AggregatedResult
                        {
                            CodeSUID = g.Key,
                            AggregatedValue = g.Min(x => x.Value)
                        })
                        .ToList();
                    break;
                case "Avg":
                    result = query.GroupBy(x => x.CodeSUID)
                        .Select(g => new AggregatedResult
                        {
                            CodeSUID = g.Key,
                            AggregatedValue = g.Average(x => x.Value)
                        })
                        .ToList();
                    break;
                case "Sum":
                    result = query.GroupBy(x => x.CodeSUID)
                        .Select(g => new AggregatedResult
                        {
                            CodeSUID = g.Key,
                            AggregatedValue = g.Sum(x => x.Value)
                        })
                        .ToList();
                    break;
                default:
                    break;
            }

            return result;
        }

        public async Task<IEnumerable<IndicatorValue>> GetIndicatorByObject(DateTime Dates, string CodeSUID)
        {
            DateTime dateInput = Dates;

            List<(string filterPropertyName, object filterValue)> filters = new List<(string, object)>();
            List<string> includs = new List<string>();
            filters.Add(new ValueTuple<string, object>("DateStart", Dates));
            filters.Add(new ValueTuple<string, object>("CodeSUID", CodeSUID));
            includs.Add("Criterias");
            includs.Add("Objects");
            var v = _uow.IndicatorValues.GetItemsWithInclude(filters, includs);
            return v.ToList();
        }

        public async Task<IndicatorValue> GetIndicatorValue(string codeSUID, DateTime dateStart, int criteriaId)
        {
            var indicatorValues = await _uow.IndicatorValues.GetAllAsync();
            var indicatorValue = indicatorValues
                .FirstOrDefault(i => i.CodeSUID == codeSUID && i.DateStart == dateStart && i.CriteriaId == criteriaId);
            return indicatorValue;
        }

        public async Task UpdateIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3)
        {
            await _uow.IndicatorValues.AddRange(indicatorValues, targetPropertyName1, targetPropertyName2, targetPropertyName3);
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

        public async Task AddIndicatorValueRange(IEnumerable<IndicatorValue> indicatorValues, string targetPropertyName1, string targetPropertyName2, string targetPropertyName3)
        {
            await _uow.IndicatorValues.AddRange(indicatorValues, targetPropertyName1, targetPropertyName2, targetPropertyName3);
        }
    }
}
