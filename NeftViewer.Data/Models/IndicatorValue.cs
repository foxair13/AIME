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
      
        public string CodeSUID { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem Objects { get; set; }
        [ForeignKey("CriteriaId")]
        public virtual Criteria Criterias { get; set; }

        private DateTime _dateStart;
        [Column("DateStart", TypeName = "timestamp with time zone")]
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }        

        public int CriteriaId { get; set; }
        public decimal Value { get; set; }
        private DateTime _lastUpdate;
        //private DateTime _lastUpdate;
        //[Column("LastUpdate", TypeName = "timestamp with time zone")]
        [Column("LastUpdate", TypeName = "timestamp with time zone")]
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }
        //{
        //    get { return _lastUpdate; }
        //    set { _lastUpdate = value; }
        //}
        [NotMapped] // This property is not mapped to the database
        public string FormattedLastUpdate
        {
            get { return LastUpdate.ToString("dd.MM.yyyy HH:mm:ss"); }
            set
            {
                DateTime parsedDate;
                if (DateTime.TryParseExact(value, "dd.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    LastUpdate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }
            }
        }
    }
}
