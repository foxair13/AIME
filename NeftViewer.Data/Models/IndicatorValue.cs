using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("IndicatorValues")]
    public partial class IndicatorValue
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }     
        public string CodeSUID { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem Objects { get; set; }
        [ForeignKey("CriteriaId")]
        public virtual Criteria Criterias { get; set; }
        
        private DateTime _dateStart;
        [Column("DateStart", TypeName = "date")]
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }

        public int CriteriaId { get; set; }
        public decimal Value { get; set; }
    }
}
