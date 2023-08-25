using DocumentFormat.OpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Roads")]
    public partial class Road
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Indicator { get; set; }
    }
}
