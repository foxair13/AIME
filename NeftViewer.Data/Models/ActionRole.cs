using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Models
{
    [Table("ActionRole")]
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
        public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
