using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBookWeb.DataAccess.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // IWebHostEnvironment: cung cấp thông tin về môi trường lưu trữ ứng dụng web (hosting environment)
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            // Truyền 1 list gồm các Product vào View
            return View(objProductList);
        }

         /*Method Create đại diện cho action Create Product
         Nhấn chuột phải vào method Create
         Chọn Add View
         Đặt tên file giống tên method action là Create
         Upsert = Update + Insert*/
        public IActionResult Upsert(int? id)
        {
            /*Lấy tất cả các Category từ db
            Sử dụng Select để tạo 1 projection
                 - Chọn thuộc tính cụ thể: Thay vì lấy toàn bộ đối tượng Category, bạn chỉ lấy Name và Id, giúp giảm lượng dữ liệu cần xử lý và lưu trữ
            SelectListItem là 1 lớp biểu diễn 1 mục trong danh sách lựa chọn(Dropdown list*/
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = new Product(),
            };
            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                //update 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);   
                return View(productVM);
            }
        }

        // [HttpPost]: chỉ định phương thức action này chỉ được gọi khi nhận được một yêu cầu HTTP có phương thức là POST.
        // file thường là ảnh
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            /* Sau khi có bất thứ thay đổi gì ở database thì để phải saveChanges lại
            Sau khi tạo xong thì ta sẽ chuyển hướng đến Action Index trong cùng controller
                Nếu khác controller thì ta cx có thể điều hướng được
                    VD: RedirectToAction("Index", "Product");
            Điều kiện dưới để kiểm tra tính hợp lệ của dữ liệu được nhập vào bằng cách sử dụng data annotations

            TempData["success"]: sử dụng để lưu trữ dữ liệu tạm thời vào key là "success"
                Truy xuất dữ liệu bằng cách sử dụng @TempData["success"] */

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // tạo ra 1 tên file ngẫu nhiêu(NewGuid) + phần mở rộng(GetExtension). VD: "6e1f29c8-a8ef-4a9f.jpg"
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    // Tạo ra 1 Path bằng cách kết hợp(Combine) đường dẫn đến thư mục gốc(wwwRootPath) với nới muốn lưu trữ product(@"images\product").
                    //      + @".." dùng để tránh lỗi liên quan tới ký tự '\'
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) 
                    {
                        // Delete the old image khi cập nhật
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath)) 
                        { 
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                } 

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                } 
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            } 
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

         /*- Nếu đặt tên method là Delete thì lại trùng tên và parameter với method Delete bên trên
         nên ta sẽ đặt 1 tên khác và thêm biệt danh-ActionTitle("Delete")
         - ActionTitle("Delete") : Đặt biệt danh cho phương thức, khiến nó được gọi khi có yêu cầu POST đến đường dẫn /Categories/Delete*/
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            // Trước khi xóa thì phải tìm danh mục đó trong CSDL 
            Product ? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remote(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        #endregion 
    }
}
