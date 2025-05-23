using Project.Business;
using Project.DbManagement;
using Microsoft.EntityFrameworkCore;
using VNPAY.NET;
using Project.Business.Interface.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using SERP.Framework.ApiUtils.Middlewares;
using SERP.Framework.ApiUtils.Utils;
using System.Configuration;
using Project.Business.Intercepter.Implement;
using Project.Business.Intercepter;

namespace Project.MVC
{
    public class Startup
    {
        private readonly IConfiguration _configuration;


        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddHttpClient();
            services.Add(ServiceDescriptor.Singleton(typeof(IExceptionFilter), typeof(ExceptionHandlingFilter)));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpRequestHelper, DefaultHttpRequestHelper>();
            services.AddSingleton<ICookieHelper, CookieHelper>();
            // Add DbContext configuration
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(_configuration["DefaultConnection"]));

            services.AddControllersWithViews();
            services.RegisterServiceComponents(_configuration);

            services.AddScoped<IBillIntercepterAfterSave, SendEmailAfterSaveBill>();
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
            services.AddSingleton(emailSettings);
            // Add session support
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(180);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //Connect VNPay API
            services.AddScoped<IVnPayService, VnPayService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            
            // Enable session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"
                );
                
            });
        }
    }
}
