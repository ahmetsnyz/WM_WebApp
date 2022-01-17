using ItServiceApp.Data;
using ItServiceApp.MapperProfiles;
using ItServiceApp.Models.Identity;
using ItServiceApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItServiceApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;

                //lockout setting 
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //atma s�resi
                options.Lockout.MaxFailedAccessAttempts = 3; //3ten fazla yanl�� girerse atar
                options.Lockout.AllowedForNewUsers = false; //kitlediyse kay�t olamas�n
                //User Setting
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                //options.SignIn.RequireConfirmedEmail = true; //confirm edilmemi� e maillerin giri�ini engelliyor.
            }).AddEntityFrameworkStores<MyContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                //cookie settings
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddAutoMapper(options =>
            {
                options.AddProfile<PaymentProfile>();
                //options.AddProfile(typeof(PaymentProfile)); ya b�yle ya da �stteki gibi ama ayn� bok
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(); //wwwroot klas�r�ndeki statik dosyalar� kullanabilmek i.in

            
            app.UseRouting();
            app.UseAuthentication();//login logout kullanabilmek i�in
            app.UseAuthorization();//authorization attiribute kullanabilmek i�in

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
