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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OwnerId { get; set; } = 1;
        [ForeignKey("OwnerId")]
        public virtual Owner Owners { get; set; }
        public int AreaId { get; set; } = 1;
        [ForeignKey("AreaId")]
        public virtual Area Areas { get; set; }
    }
}
