using BLL.Service.Interface;
using BLL.Service;
using DAL.Datas;
using Microsoft.EntityFrameworkCore;
using DAL.Repository.Interface;
using DAL.Repository;
using Microsoft.Extensions.FileProviders;
using MVC.Filters;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AddFolderRouteModelConvention("/", model =>
                {
                    foreach (var selector in model.Selectors)
                    {
                        selector.AttributeRouteModel.Template = "razor/" + selector.AttributeRouteModel.Template;
                    }
                });
                // Apply filter cho toàn bộ Razor Pages
                options.Conventions.AddFolderApplicationModelConvention("/", model =>
                {
                    model.Filters.Add(new AdminAuthorizationFilter());
                });
            });// For Razor Pages


            builder.Services.AddDbContext<DemoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped(typeof(IAccountService), typeof(AccountService));

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService, ProductService>();
            // Add session services
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Cho phép truy cập ảnh từ folder bên ngoài
            app.UseStaticFiles(); // Đừng bỏ dòng này, dùng cho wwwroot

            // Thêm dòng này để cấu hình dùng SharedImages ở gốc solution
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "..", "SharedImages")),
                RequestPath = "/SharedImages"
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Use session middleware
            app.UseSession();

            app.MapControllerRoute(
                name: "mvc",
                pattern: "mvc/{controller=Auth}/{action=Login}/{id?}");

            app.MapRazorPages();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/mvc/auth/CheckIsLogin");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}
