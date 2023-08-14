using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Objects")]
    public partial class ObjectItem
    {
        [Key]

        public string CodeSUID { get; set; }
        public string Name { get; set; }
        //public virtual ICollection<IndicatorValue> IndicatorValues { get; set; }
    }
}
