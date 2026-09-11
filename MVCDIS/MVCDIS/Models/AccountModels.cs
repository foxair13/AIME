using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace MVCDIS.Models
{

    public class MembershipContext : DbContext
    {
        public MembershipContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Membership> MembershipProfiles { get; set; }
        public DbSet<Roles> MembershipRoles { get; set; }
        public DbSet<ActionList> _ActionList { get; set; }
        public DbSet<ActionByRole> _ActionByRole { get; set; }



    }
    public class getInfoMode
    {
        public bool Get(String Name)
        {
            using (var db = new UsersContext())
            {
                int uid = WebSecurity.GetUserId(Name);
                UserProfile up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                return (bool)up.State;

            }
        }
    }

    [Table("webpages_Membership")]
    public class Membership
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string ConfirmationToken { get; set; }
    }
    [Table("ActionByRole")]
    public class ActionByRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int ActionID { get; set; }
        [Required]
        public int RoleID { get; set; }

        [ForeignKey("ActionID")]
        public virtual ActionList ActionList { get; set; }
        [ForeignKey("RoleID")]
        public virtual Roles Roles { get; set; }



    }
    [Table("ActionList")]
    public class ActionList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Action { get; set; }

          public string   Description { get; set; } 

    }

   
    [Table("webpages_Roles")]
    public class LRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<ActionList> ActionAttending { get; set; }
    }
    [Table("webpages_Roles")]
    public class Roles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
         [Required]
         [Display(Name = "Название роли")]

        public string RoleName { get; set; }
         [Display(Name = "Описание роли")]
        public string RoleDescription { get; set; }
    }
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<webpages_UsersInRoles> _webpages_UsersInRoles { get; set; }
        public DbSet<Images> _Images { get; set; }
       
    }

    [Table("Images")]
    public class Images
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public byte[] Picture { get; set; }
        public string mime { get; set; }

    }
    public class RegInfo
    {
        public String UserName { get; set; }
        public String Fio { get; set; }
        public String Email { get; set; }
        public String CreateDate { get; set; }
        public String LastPasswordFailureDate { get; set; }
        public String PasswordChangedDate { get; set; }
    }
    public class UserDetails
    {
        public String UserName { get; set; }
        public String Email { get; set; }
        public bool[] Flag { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Неверный формат Email")]
        public String Email { get; set; }
        public int? ClientsID { get; set; }
        [ForeignKey("ClientsID")]
        public virtual Clients Clients { get; set; }
        public int? ImagesID { get; set; }
        public bool? State { get; set; }

        [ForeignKey("ImagesID")]
        public virtual Images Images { get; set; }

    }

    [Table("webpages_UsersInRoles")]
    public class webpages_UsersInRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public int RoleId { get; set; }




    }



    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Повторить пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
    public class LostPasswordModel
    {
        [Required(ErrorMessage = "Необходимо указать ваш Email, что бы мы могли выслать вам ссылку!")]
        [Display(Name = "Ваш электронный адресс")]
        [EmailAddress(ErrorMessage = "Неверный формат ввода Email")]
        public string Email { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Ваш пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть больше {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть больше {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Неверный формат Email")]
        public String Email { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}