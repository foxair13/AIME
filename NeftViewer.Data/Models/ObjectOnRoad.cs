using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("ObjectOnRoad")]
    public partial class ObjectOnRoad
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public string RoadId { get; set; }
        [ForeignKey("RoadId")]
        public virtual Roads Roads { get; set; }
        public string UIDObject { get; set; }
        [ForeignKey("UIDObject")]
        public virtual Objects Objects { get; set; }
    }
}
