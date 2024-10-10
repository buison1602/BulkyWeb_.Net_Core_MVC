using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Nhập "ctor" để tạo ra hàm tạo nhanh chóng
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Dùng để tạo Category Table 
        public DbSet<Category> Categories { get; set; }

        // Đây là 1 method mặc định của DbContext 
        // 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Thêm records vào trong bảng Categories ở Database
            // Để thêm thì cần phải thêm 1 migration - add-migration SeedCategoryTable 
            // Bất cứ khi nào cần cập nhật thứ gì vào DB ta đều cần thêm 1 migration
            // Sau khi thêm thì cần phải cập nhật lại database - update-database
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOder = 2 },
                new Category { Id = 3, Name = "History", DisplayOder = 3 }
                );
        }
    }
}
