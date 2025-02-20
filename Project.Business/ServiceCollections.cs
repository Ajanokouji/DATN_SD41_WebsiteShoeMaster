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
        public static IServiceCollection RegisterServiceComponents(this IServiceCollection services,
       IConfiguration configuration)
        {
          var connectionString = configuration["DefaultConnection"];
            services.AddDbContext<ProjectDbContext>(options =>
            {
                options.UseSqlServer(connectionString,b=>b.MigrationsAssembly("Project.DbManagement"));
            });

            services.AddScoped<IProductBusiness, ProductBusiness>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IProductCategoriesRelationBusiness, ProductCategoriesRelationBusiness>();
            services.AddScoped<IProductCategoriesRelationRepository, ProductCategoriesRelationRepository>();

            services.AddScoped<IBillBusiness, BillBusiness>();
            services.AddScoped<IBillRepository, BillRepository>();

            services.AddScoped<IBillDetailsBusiness, BillDetailsBusiness>();
            services.AddScoped<IBillDetailsRepository, BillDetailsRepository>();

            services.AddScoped<ICartBusiness, CartBusiness>();
            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<ICartDetailsBusiness, CartDetailsBusiness>();
            services.AddScoped<ICartDetailsRepository, CartDetailsRepository>();

            services.AddScoped<IContactBusiness, ContactBusiness>();
            services.AddScoped<IContactRepository, ContactRepository>();

            services.AddScoped<ICustomerBusiness, CustomerBusiness>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IPaymentMethodsBusiness, PaymentMethodsBusiness>();
            services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();

            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IVoucherBusiness, VoucherBusiness>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();

            services.AddScoped<IVoucherDetailsBusiness, VoucherDetailsBusiness>();
            services.AddScoped<IVoucherDetailsRepository, VoucherDetailsRepository>();

            services.AddScoped<ICategoriesBusiness, CategoriesBusiness>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();


            return services;
        }
    }
}
