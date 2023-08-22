using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NeftViewer.Data.Models
{
    [Table("ActionRoles")]
    public class ActionRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ActionId { get; set; }

        public string RoleId { get; set; }

        [ForeignKey("ActionId")]
        public virtual Action Action { get; set; }
        [ForeignKey("RoleId")]
        public virtual AspNetRole AspNetRoles { get; set; }

    }
}
