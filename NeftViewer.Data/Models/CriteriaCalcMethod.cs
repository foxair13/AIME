using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("CriteriaCalcMethod")]
    public partial class CriteriaCalcMethod
    {
        [Key]
        public int Id { get; set; }
        public int CriteriaId { get; set; }
       
        public bool СalculationByMax { get; set; }
        [ForeignKey("CriteriaId")]
        public virtual Criteria Criterias { get; set; }
        public bool IsHidden { get; set; }
        
    }
}
