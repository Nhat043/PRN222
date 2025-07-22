using BLL.Service.Interface;
using BLL.Service;
using DAL.Datas;
using DAL.Repository.Interface;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Razor.Hubs;

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
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<ICommentStatusRepo, CommentStatusRepo>();
            builder.Services.AddScoped<ICommentStatusService, CommentStatusService>();

            builder.Services.AddScoped<IComRepo, ComRepo>();
            builder.Services.AddScoped<IComService, ComService>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IProductItemStatusRepo, ProductItemStatusRepo>();
            builder.Services.AddScoped<IProductItemStatusService, ProductItemStatusService>();

            builder.Services.AddScoped<IProductItemRepo, ProductItemRepo>();
            builder.Services.AddScoped<IProductItemService, ProductItemService>();

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IRatingRepo, RatingRepo>();
            builder.Services.AddScoped<IRatingService, RatingService>();

            builder.Services.AddScoped<IStatisticsRepo, StatisticsRepo>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();

            builder.Services.AddScoped<IVariationOptionRepo, VariationOptionRepo>();
            builder.Services.AddScoped<IVariationOptionService, VariationOptionService>();


            builder.Services.AddScoped<IVariationRepo, VariationRepo>();
            builder.Services.AddScoped<IVariationService, VariationService>();
            builder.Services.AddSignalR();


            builder.Services.AddDistributedMemoryCache(); // Cho phép lưu session trong RAM
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // thời gian hết hạn session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //Add CORS policy for SignalR
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // SignalR cần dòng này
                });
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRouting();
            app.UseCors(); // Enable CORS for SignalR
            app.UseHttpsRedirection();
            app.UseStaticFiles();

           
           
            app.UseSession();
           
            app.MapHub<DataSignalR>("/DataSignalRChanel");
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
