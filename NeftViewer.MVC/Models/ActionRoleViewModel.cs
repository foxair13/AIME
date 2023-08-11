using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NeftViewer.MVC.Models
{
    public class ActionRoleViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Наименование события:")]
        public int ActionId { get; set; }
        [Required]
        [DisplayName("Наименование роли:")]
        public string RoleId { get; set; }
    }
}
