using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
     /*IRepository<T>: Đây là một interface generic(chung).

     <T>: Khai báo một tham số kiểu generic(kiểu chung). Nghĩa là interface này có thể làm việc với
     nhiều kiểu dữ liệu khác nhau, được đại diện bởi chữ T.

     where T : class: Đây là một constraint (ràng buộc) đối với tham số kiểu T.Nó yêu cầu rằng T phải
     là một kiểu tham chiếu (reference type), tức là phải là một class (lớp). Điều này loại trừ các
     kiểu giá trị(value type) như int, double, bool, struct, enum.

     Ở đây ta muốn thực hiện các chức năng CRUD*/
    public interface IRepository<T> where T : class 
    {
        // T là 1 class, có thể là Category, Product, ...  
        // IEnumerable<T> là 1 danh sách các obj của class T
        IEnumerable<T> GetAll(string? includeProperties = null);

        // Expression<Func<T, bool>>: Kiểu của biểu thức lambda
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remote(T entity);
        void RemoteRange(IEnumerable<T> entity);
    }
}
