using Microsoft.EntityFrameworkCore;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherDetails> VoucherDetails { get; set; }
        public DbSet<BillEntity> Bills { get; set; }
        public DbSet<BillDetailsEntity> BillDetails { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Contacts> Contacts{ get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CategoriesEntity> Categories { get; set; }
        public DbSet<UserEntity> Users{ get; set; }
        public DbSet<ContentBase>  ContentBases{ get; set; }
        public DbSet<CustomersEntity> Customers { get; set; }
        public DbSet<ImageFileEntity> ImageFiles { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<ProductCategoriesRelation> ProductCategoriesRelations { get; set; }

        public DbSet<VoucherProducts> VoucherProducts { get; set; }
        public DbSet<VoucherUsers> VoucherUsers { get; set; }

        public DbSet<CommentsEntity> Comments { get; set; }
        public ProjectDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ProjectDbContext()
        {
        }
    }
}
