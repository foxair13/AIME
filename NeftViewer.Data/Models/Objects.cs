using DocumentFormat.OpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Objects")]
    public partial class Objects
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        public string CodeSuid { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        public int RegionId { get; set; }
        public int CityId { get; set; }
    }
}
