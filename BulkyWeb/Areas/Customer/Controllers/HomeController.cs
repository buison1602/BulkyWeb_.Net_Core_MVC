using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]

    // Khi có folder Views chứa nhiều folder con. Mỗi folder con ứng với mỗi Controller và chứa nhiều file View.
    // Khi đó sẽ dựa vào tên class để chọn ra folder con nào. Tại đây ta có tên class là HomeController nên
    // chọn ra folder Views/Home 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            // Trả về 1 số file View bên trong folder Views 
            // Nếu trong dấu ngoặc đơn ta không xác định tên file View cần trả về thì nó sẽ mặc định lấy 
            // file có cùng tên với tên Action( ở đây là Privacy ) 
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
