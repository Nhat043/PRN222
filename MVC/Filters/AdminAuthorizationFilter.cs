using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters
{
    public class AdminAuthorizationFilter: IAsyncPageFilter
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var roleId = context.HttpContext.Session.GetInt32("RoleId");

            if (roleId != 1) // 1 = Admin
            {
                context.Result = new RedirectResult("/mvc/Auth/CheckIsLogin");
                return;
            }

            await next();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
