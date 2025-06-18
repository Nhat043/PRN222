using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters
{
    public class AdminAuthorizationFilter: IAsyncPageFilter
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var roleIdString = context.HttpContext.Session.GetString("RoleIdSession");
            if (roleIdString != "1") // 1 = Admin
            {
                context.Result = new RedirectResult("/Auth/CheckIsLogin");
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
