using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Customers")]
    public partial class Customer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        //public string ClaimType { get; set; }
        //public string ClaimValue { get; set; }
        //[ForeignKey("RoleId")]
        //public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
