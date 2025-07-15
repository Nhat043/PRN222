using BLL.Service.Interface;
using BLL.Service;
using DAL.Datas;
using DAL.Repository.Interface;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Razor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<DemoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Repo to the container
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IComRepo, ComRepo>();

            builder.Services.AddScoped<IComService, ComService>();

            builder.Services.AddScoped<ICommentStatusService, CommentStatusService>();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            // Redirect root to ProductPage/Index
            app.MapGet("/", context => {
                context.Response.Redirect("/ProductPage/Index");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}
