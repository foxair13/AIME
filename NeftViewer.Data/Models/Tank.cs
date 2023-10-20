using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Tanks")]
    public partial class Tank
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CodeSUID { get; set; }
        public string Number { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem ObjectItems { get; set; }
    }
}
