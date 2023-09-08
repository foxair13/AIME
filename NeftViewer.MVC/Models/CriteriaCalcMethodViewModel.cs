using NeftViewer.Data.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.MVC.Models
{
    public class CriteriaCalcMethodViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Наименование критерия:")]
        public int CriteriaId { get; set; }
        [DisplayName("Наименование критерия:")]
        public string? Name { get; set; }
        [Required]
        [DisplayName("Лучшие показатели считать по максимальному значению?")]
        public bool СalculationByMax { get; set; }
        [DisplayName("Скрытый критерий:")]
        public bool IsHidden { get; set; }
    }
}
