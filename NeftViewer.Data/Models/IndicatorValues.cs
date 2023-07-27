using DocumentFormat.OpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("IndicatorValues")]
    public partial class IndicatorValues
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customers Customers { get; set; }
        [ForeignKey("CriteriaId")]
        public virtual Criterias Criterias { get; set; }
        public string CodeSuid { get; set; }
        [ForeignKey("CodeSuid")]
        public virtual Objects Objects { get; set; }
        public DateTime DateStart { get; set; }
        public string Value { get; set; }
    }
}
