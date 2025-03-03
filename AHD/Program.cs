using AHD.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Stripe;
using Utility;

namespace AHD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = false;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; 
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
            });
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<CityIRepository, CityRepository>();
            builder.Services.AddScoped<CemeteryIRepository, CemeteryRepository>();
            builder.Services.AddScoped<MosqueIRepository, MosqueRepository>();
            builder.Services.AddScoped<ProductIRepository, ProductRepository>();
            builder.Services.AddScoped<CartIRepository, CartRepository>();
            builder.Services.AddScoped<OrderIRepository, OrderRepository>();
            builder.Services.AddScoped<paymentIRepository, paymentRepository>();
            builder.Services.AddScoped<DeliveryLocationIRepository, DeliveryLocationRepository>();



            builder.Services.AddAuthentication("Cookies")
         .AddCookie(options =>
         {
             options.LoginPath = "/Account/Login";
             options.AccessDeniedPath = "/Account/AccessDenied"; 
         });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole(SD.AdminRoles));
                options.AddPolicy("SubAdminOnly", policy => policy.RequireRole(SD.SubAdminRoles));
                options.AddPolicy("DAgentOnly", policy => policy.RequireRole(SD.DAgentRoles));
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}
