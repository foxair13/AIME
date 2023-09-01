using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Criterias")]
    public partial class Criteria
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AgregateId { get; set; }
        [ForeignKey("AgregateId")]
        public virtual Agregate Agregates { get; set; }
    }
}
