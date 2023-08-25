using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("ObjectOnRoad")]
    public partial class ObjectOnRoad
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RoadId { get; set; }
        [ForeignKey("RoadId")]
        public virtual Road Roads { get; set; }
        public string CodeSUID { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem Objects { get; set; }
    }
}
