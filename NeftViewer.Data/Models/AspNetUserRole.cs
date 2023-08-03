using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Models
{
    [Table("AspNetUserRoles")]
    public partial class AspNetUserRole
    {
       
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser AspNetUsers { get; set; }
        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual AspNetRole AspNetRoles { get; set; }
    }
}
