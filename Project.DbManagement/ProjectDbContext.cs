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
        public DbSet<BillDetails> BillDetails { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Contacts> Contacts{ get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<Customers> Customers { get; set; }



        public ProjectDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ProjectDbContext()
        {
        }
    }
}
