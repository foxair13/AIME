using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.Data.Models
{
    [Table("Energies")]
    public partial class Energy
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CodeSUID { get; set; }
        [ForeignKey("CodeSUID")]
        public virtual ObjectItem Objects { get; set; }
        public DateTime ShiftBegin { get; set; }
        public string Oil { get; set; }
        public string Trk { get; set; }
        public decimal Value { get; set; }
    }
}
