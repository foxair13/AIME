using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.EntityRepositories;
using NeftViewer.MVC;
using NeftViewer.MVC.Factories;
using NeftViewer.MVC.Options;

namespace NeftViewer.Core.ActionFilters
{

    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {

        private readonly string _selector;
        private readonly string _connectionString;
        NeftViewerContext nvc;
        public CustomAuthorizeAttribute()
        {

        }

        public CustomAuthorizeAttribute(string selector)
        {
            _selector = selector;
            _connectionString = AppConfig.GetConnectionString().BasePostgree;
            nvc = nvc = NeftViewerContextFactory.CreateDbContext(_connectionString);
        }

        private string GetRoles(string actionselector)
        {

            List<ActionRole> list = (from k in nvc.ActionRoles
                                     where k.Action.Name == actionselector
                                     select k).ToList<ActionRole>();
            string str = "";
            int i = 0;
            foreach (ActionRole item in list)
            {
                if (i == 0)
                {
                    str += item.AspNetRoles.Name;
                }
                else
                {
                    str += string.Concat(",", item.AspNetRoles.Name);
                }
                i++;
            }
            return str;
        }
        public bool CheckRoles(string actionselector, string name)
        {
            bool flag = false;
            string[] words = GetRoles(actionselector).Split(',');
            var roles = (from ur in nvc.AspNetUserRoles
                         join role in nvc.AspNetRoles on ur.RoleId equals role.Id
                         where ur.AspNetUsers.Email == name
                         select role.Name
                         );
            foreach (var item in roles)
            {
                var results = from t in words where t.Trim() == item.Trim() select t;

                if (results.Count() == 0)
                {
                    flag = false;
                    break;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }



        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Если пользователь не авторизован, перенаправляем на страницу входа
                context.Result = new RedirectToRouteResult(new { area = "Identity", page = "/Account/Login" });
                return;
            }
            else
            {
                string name = context.HttpContext.User.Identity.Name;
                var currentPath = context.HttpContext.Request.Path.ToString();

                var actionSelector = _selector ?? context.ActionDescriptor.AttributeRouteInfo?.Name;
                if (!string.IsNullOrEmpty(actionSelector) && CheckRoles(_selector, name))
                {
                    context.Result = new RedirectToRouteResult(new { area = "", page = currentPath });
                }

            }

        }
    }
}

