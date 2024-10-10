using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public int ListPrice { get; set; } // Giá niêm yết

        // Khi khách hàng mua với số lượng sách nhất định thì sẽ có giá tương ứng
        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        public int Price { get; set; } // Giá niêm yết

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public int Price50 { get; set; } // Giá niêm yết

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public int Price100 { get; set; } // Giá niêm yết 

        // Thuộc tính Category này được sử dụng để điều hướng khóa ngoại cho CategoryId
        // Sử dụng chú thích dữ liệu 
        public int CategoryId { get; set; }

        // Navigation property trỏ đến Category
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
