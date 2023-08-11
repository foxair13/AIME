using NeftViewer.Data.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeftViewer.MVC.Models
{
    public class ActionViewModel
    {
        public int Id { get; set; }
        [DisplayName("Наименование действия:")]
        public string Name { get; set; }
        [DisplayName("Описание действия:")]
        public string Description { get; set; }

    }
}
