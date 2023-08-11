using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("IndicatorValues")]
    public partial class IndicatorValue
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public virtual Customer Customers { get; set; }
        public virtual Criteria Criterias { get; set; }
        public string CodeSUID { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem Objects { get; set; }
        private DateTime _dateStart;

        [Column("DateStart", TypeName = "timestamp with time zone")]
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }
        public string Value { get; set; }
    }
}
