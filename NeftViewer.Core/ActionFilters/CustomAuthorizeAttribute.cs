using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Core.ActionFilters
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter

    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Здесь вы можете выполнить свою пользовательскую логику авторизации.

            // Например, проверить, что пользователь авторизован:
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Если пользователь не авторизован, перенаправляем на страницу входа или возвращаем ошибку 401 Unauthorized.
                context.Result = new UnauthorizedResult();
                return;
            }

            // Проверяем дополнительные условия доступа, например, роли или другие пользовательские права.
            // ...

            // Если пользователь прошел все проверки, продолжаем выполнение действия контроллера.
        }
    }
}

