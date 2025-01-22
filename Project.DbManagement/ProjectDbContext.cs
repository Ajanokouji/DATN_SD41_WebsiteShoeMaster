using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class ProjectDbContext : DbContext
    {
        public DbSet<giam_gia> giam_gias { get; set; }
        public DbSet<chi_tiet_giam_gia> chi_tiet_giam_gias { get; set; }
        public DbSet<hoa_don> hoa_dons { get; set; }
        public DbSet<phuong_thuc_thanh_toan> phuong_thuc_thanh_toans { get; set; }
        public DbSet<hoa_don_chi_tiet> hoa_don_chi_tiets { get; set; }
        public DbSet<thong_tin_lien_he> thong_tin_lien_hes { get; set; }

        public ProjectDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ProjectDbContext()
        {
        }
    }
}
