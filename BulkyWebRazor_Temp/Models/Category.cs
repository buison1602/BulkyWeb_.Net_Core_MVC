using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
        // Sử dụng data annotations(chú thích dữ liệu) 
        [Key]
        public int Id { get; set; } // Khóa chính 
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Oder must be between 1-100")]
        public int DisplayOder { get; set; } // thứ tự hiển thị 
    }
}
