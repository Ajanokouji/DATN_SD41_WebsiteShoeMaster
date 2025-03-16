using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business
{
    public static class ServiceCollections
    {
        public static void RegisterServiceComponents(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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
