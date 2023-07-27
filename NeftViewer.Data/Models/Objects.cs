using DocumentFormat.OpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Objects")]
    public partial class Objects
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CodeSuid { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        public int RegionId { get; set; }
        public int CityId { get; set; }

        //public string ClaimType { get; set; }
        //public string ClaimValue { get; set; }
        //[ForeignKey("RoleId")]
        //public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
