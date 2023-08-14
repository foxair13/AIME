using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Customers")]
    public partial class Customer
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        //public virtual ICollection<IndicatorValue> IndicatorValues { get; set; }
    }
}
