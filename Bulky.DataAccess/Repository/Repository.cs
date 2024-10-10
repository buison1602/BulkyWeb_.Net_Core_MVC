using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BulkyBook.DataAccess.Repository
{
    /*Vì<T> là một kiểu generic(kiểu tổng quát) nên khi kế thừa lại interface IRepository thì có 2 cách 
          + C1: Duy trì generic.Giúp nó có thể hoạt động linh hoạt với mọi đối tượng class: Category, Product, ...
              var categoryRepository = new Repository<Category>(dbContext); 
      
          + C2: Cụ thể hóa
              public class ABC : IRepository<Category> { }*/
    public class Repository<T> : IRepository<T> where T : class
    {
        // Đại diện cho kết nối tới CSDL, thực hiện CRUD thông qua nó
        private readonly ApplicationDbContext _db;
        // Tập hợp các đối tượng thuộc kiểu T được ánh xạ (map) tới bảng tương ứng trong CSDL
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db) {
            _db = db;
            this.dbSet = _db.Set<T>();
            //_db.Categories = dbSet = bảng Categories trong CSDL 

            // Sử dụng Inclue để khi truy vấn dữ liệu từ một bảng, ta có thể dễ dàng truy cập dữ liệu từ các bảng liên quan mà không cần 
            // phải thực hiện truy vấn thủ công(join).
                    _db.Products.Include(u => u.Category).Include(u => u.CategoryId);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        // Category, CoverType
        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            /*
             includeProperties là 1 CHUỖI gồm danh sách các thực thể liên quan. VD: "Category,Supplier"
            Split: tách chuỗi bởi dấu ',' 

             */
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var includeProp in includeProperties
                    .Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remote(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoteRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
