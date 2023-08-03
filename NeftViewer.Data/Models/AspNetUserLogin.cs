using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Models
{
    [Table("AspNetUserLogins")]
    public partial class AspNetUserLogin
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LoginProvider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public string? ProviderDisplayName { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; } = null!;
        public string UserId { get; set; }
    }
}
