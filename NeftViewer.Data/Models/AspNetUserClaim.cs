using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Models
{
    [Table("AspNetUserClaims")]
    public partial class AspNetUserClaim
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]  
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser AspNetUsers { get; set; }
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
    }
}
