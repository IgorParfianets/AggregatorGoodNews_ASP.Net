using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;

namespace AspNet.MvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // For Logger Serilog !!!

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //------------------------------------------------------------
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //------------------------------------------------------------

            // Authentication configuration
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString(@"/Account/Login");
                    options.LogoutPath = new PathString(@"/Account/Logout");
                    options.AccessDeniedPath = new PathString(@"/Account/Login");
                });

            // Db Context
            builder.Services.AddDbContext<AggregatorContext>(optionBuilder =>
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default"))); // For DB (connectionString in config files)

            // Mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // For Mapping to collect all profiles

            // Configuration
            builder.Configuration.AddJsonFile("hashingsalt.json"); // for custom configuration

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseSession(); // Check that is mean

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        /*
         * todo add Authenticate => Claims => Roles
         * todo add Identity?
         * todo implement Edit for user ( private room ) => add Image/Ava
         * todo refactor names / ASYNC / try-catch in controllers
         * todo edit entities some add/some remove
         *
         * todo implement Logger Serilog
         */
    }
}