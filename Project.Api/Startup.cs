using Project.Business;
using SERP.Framework.ApiUtils;
using Microsoft.Extensions.DependencyInjection;
using SERP.Framework.LoggingConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting; // Add this using directive
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SERP.Framework.ApiUtils.Utils;
using SERP.Framework.ApiUtils.Middlewares;
using Microsoft.EntityFrameworkCore;
using Project.DbManagement;
namespace Project.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = StartupHelpers.CreateDefaultConfigurationBuilder(env)
                            .AddLogging()
                            .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            if (Configuration is IConfigurationRoot configurationRoot) Console.WriteLine(configurationRoot.GetDebugView());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddHttpClient();
            services.Add(ServiceDescriptor.Singleton(typeof(IExceptionFilter), typeof(ExceptionHandlingFilter)));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpRequestHelper, DefaultHttpRequestHelper>();
            services.AddSingleton<ICookieHelper, CookieHelper>();
            services.AddMvc(delegate (MvcOptions x)
            {
                x.Filters.AddService(typeof(IExceptionFilter));
                x.EnableEndpointRouting = false;
            });
            services.AddAuthorization();

            services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlServer(Configuration["DefaultConnection"]));

            services.RegisterServiceComponents(Configuration);
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Thêm cấu hình CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000", builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:5173",  //Them localhost cua Fe Admin site de khong bi loi CORS
                        "http://localhost:3000"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();

            app.UseRouting();
            // app.UseAuthorization();
            app.UseCors("AllowLocalhost3000");

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
