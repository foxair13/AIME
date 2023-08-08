using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.MVC.FinanceViewModels
{
    [Table("Criterias")]
    public class CriteriasViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
