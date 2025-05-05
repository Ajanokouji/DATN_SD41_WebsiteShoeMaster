using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Project.Business.Interface;
using Project.Business.Interface.Project.Business.Interface.Repositories;
using Project.Business.Interface.Repositories;
using Project.Business.ModelFactory;
using Project.Business.ModelFactory.Implement;
using Project.DbManagement;
using System.Data;

namespace Project.Business
{
    public static class ServiceCollections
    {
        public static void RegisterServiceComponents(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            var x = configuration["DefaultConnection"];
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(x));

            // Register Memory Cache
            services.AddMemoryCache();

            // Register Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IVoucherDetailsRepository, VoucherDetailsRepository>();
            services.AddScoped<ICartDetailsRepository, CartDetailsRepository>();
            services.AddScoped<IBillDetailsRepository, BillDetailsRepository>();
            services.AddScoped<IProductCategoriesRelationRepository, ProductCategoriesRelationRepository>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<IImageFileRepository, ImageFileRepository>();
            services.AddScoped<IVoucherProductsRepository, VoucherProductRepository>();

            // Register Business Services

            services.AddScoped<IProductBusiness, ProductBusiness>();
            services.AddScoped<ICartBusiness, CartBusiness>();
            services.AddScoped<IBillBusiness, BillBusiness>();
            services.AddScoped<IPaymentMethodsBusiness, PaymentMethodsBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<ICustomerBusiness, CustomerBusiness>();
            services.AddScoped<IContactBusiness, ContactBusiness>();
            services.AddScoped<IVoucherBusiness, VoucherBusiness>();
            services.AddScoped<IVoucherDetailsBusiness, VoucherDetailsBusiness>();
            services.AddScoped<ICartDetailsBusiness, CartDetailsBusiness>();
            services.AddScoped<IBillDetailsBusiness, BillDetailsBusiness>();
            services.AddScoped<IProductCategoriesRelationBusiness, ProductCategoriesRelationBusiness>();
            services.AddScoped<ICategoriesBusiness, CategoriesBusiness>();
            services.AddScoped<IImageFileBusiness, ImageFileBusiness>();
            services.AddScoped<IVoucherProductsBusiness, VoucherProductsBusiness>();

            services.AddScoped<IBillModelFactory, BillModelFactory>();
            services.AddScoped<IBillDetailModelFactory, BillDetailModelFactory>();
            services.AddTransient<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration["DefaultConnection"];
                return new SqlConnection(connectionString);
            });
            services.AddScoped<IProductRepository, ProductDapperRepository>();


            // Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }
    }
}
