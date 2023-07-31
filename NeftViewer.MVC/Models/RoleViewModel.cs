using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NeftViewer.MVC.Models
{
    public class RoleViewModel    {

       
        public string? Id { get; set; }
        [DisplayName("Наименование роли:")]
        public string RoleName { get; set; }
    }
}
