using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            // Truyền 1 list gồm các category vào View
            return View(objCategoryList);
        }

        // Method Create đại diện cho action Create Category
        // Nhấn chuột phải vào method Create
        // Chọn Add View 
        // Đặt tên file giống tên method action là Create 
        public IActionResult Create()
        {
            return View();
        }

        // [HttpPost]: chỉ định phương thức action này chỉ được gọi khi nhận được một yêu cầu HTTP có phương thức là POST.
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOder.ToString())
            {
                ModelState.AddModelError("Title", "The DisplayOrder cannot exactly match the Title.");
            }
            //if (obj.Title != null && obj.Title.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "Test is an invalid value");
            //}

            // Sau khi có bất thứ thay đổi gì ở database thì để phải saveChanges lại 
            // Sau khi tạo xong thì ta sẽ chuyển hướng đến Action Index trong cùng controller 
            // Nếu khác controller thì ta cx có thể điều hướng được
            //      VD: RedirectToAction("Index", "Category");
            // Điều kiện dưới để kiểm tra tính hợp lệ của dữ liệu được nhập vào bằng cách sử dụng data annotations

            // TempData["success"]: sử dụng để lưu trữ dữ liệu tạm thời vào key là "success"
            // Ttruy xuất dữ liệu bằng cách sử dụng @TempData["success"]
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return Created();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                // Tự động cập nhật dựa trên Id của obj được truyền vào
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // - Nếu đặt tên method là Delete thì lại trùng tên và parameter với method Delete bên trên
        // nên ta sẽ đặt 1 tên khác và thêm biệt danh-ActionTitle("Delete")
        // - ActionTitle("Delete"): Đặt biệt danh cho phương thức, khiến nó được gọi khi có yêu cầu POST đến đường dẫn /Categories/Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            // Trước khi xóa thì phải tìm danh mục đó trong CSDL 
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remote(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
