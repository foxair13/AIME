using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Areas")]
    public partial class Area
    {
        [Key]
        public string CodeSUID { get; set; }
        public string Name { get; set; }
    }
}
