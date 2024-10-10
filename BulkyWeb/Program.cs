using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBookWeb.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // tạo 1 đối tượng builder để xây dựng ứng dụng web. Đối tượng này sẽ được sử dụng để cấu hình
            // các dịch vụ và đường dẫn của ứng dụng.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Dịch vụ này cho phép ứng dụng sử dụng MVC (Model-View-Controller) để tách logic nghiệp vụ,
            // giao diện người dùng và dữ liệu.
            builder.Services.AddControllersWithViews();

            // Tạo Database
            // DbContext: Lớp trung gian giúp ứng dụng giao tiếp với database.
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                // Sử dụng lambda để lấy ra chuỗi kết nối tới SqlServer trong file appsettings.json nhằm
                // cấu hình DbContext, từ đó mới sử dụng được SQL Server 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            /*     + ASP.NET Core sử dụng Dependency Injection (DI) để tự động cung cấp các dịch vụ và đối tượng
             cần thiết cho các thành phần như controller, service, middleware, v.v. Khi bạn yêu cầu một
             đối tượng (ví dụ IUnitOfWork) thông qua constructor của controller, DI container sẽ
             cố gắng khởi tạo đối tượng đó.
            
                 + Nếu IUnitOfWork không được đăng ký trong DI container(bằng cách gọi builder.Services.
             AddScoped<IUnitOfWork, UnitOfWork>() hoặc phương thức tương tự), DI container sẽ
             không biết phải tạo đối tượng IUnitOfWork bằng cách nào.Do đó, nó sẽ dẫn đến lỗi.
            */
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Xây dựng ứng dụng web dựa trên cấu hình đã tạo ở các bước trước
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
