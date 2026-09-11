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
        private static bool result = false;
        public CustomAuthorizeAttribute()
        {

        }
        public static void SetResult(bool flag) 
        {
            result = flag;
        }
        public CustomAuthorizeAttribute(string selector)
        {
            _selector = selector;
            _connectionString = AppConfig.GetConnectionString().BasePostgree;
        
        }
        private NeftViewerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<NeftViewerContext>()
                .UseNpgsql(_connectionString)
                .Options;

            return new NeftViewerContext(options);
        }
        private string GetRoles(string actionselector)
        {
            using (var nvc = CreateContext())
            {
                List<ActionRole> list = (from k in nvc.ActionRoles.Include(x => x.AspNetRoles)
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
        }

        public bool CheckRoles(string actionselector, string name)
        {
            using (var nvc = CreateContext())
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
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (!result)
            {
                context.Result = new RedirectToRouteResult(new { area = "Identity", page = "/Account/Login" });
                return;
            }
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Если пользователь не авторизован, перенаправляем на страницу входа
                context.Result = new RedirectToRouteResult(new { area = "Identity", page = "/Account/Login" });
                return;
            }
            else
            {
                string name = context.HttpContext.User.Identity.Name;
                bool isAdmin = context.HttpContext.User.IsInRole("Admin");
                var routeData = context.ActionDescriptor.RouteValues;
                var area = routeData["area"]?.ToString();
                var page = routeData["page"]?.ToString();
                if (area == "Identity" && page == "Account/AccessDenied")
                {
                    return;
                }
                var actionSelector = _selector ?? context.ActionDescriptor.AttributeRouteInfo?.Name;
                if ((!string.IsNullOrEmpty(actionSelector) && CheckRoles(_selector, name)) || isAdmin)
                {
                    return;
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new { area = "Identity", page = "/Account/AccessDenied" });
                }

            }

        }
    }
}

