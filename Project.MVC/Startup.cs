using Project.Business;
using Project.DbManagement;
using Microsoft.EntityFrameworkCore;
using VNPAY.NET;

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
            // Add DbContext configuration
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(_configuration["DefaultConnection"]));

            services.AddControllersWithViews();
            services.RegisterServiceComponents(_configuration);
            services.AddSingleton<IVnpay, Vnpay>();

            // Add session support
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
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
