using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Khachhang
    {
        public Khachhang()
        {
            Phieudathangs = new HashSet<Phieudathang>();
        }

        [Display(Name = "Mã khách hàng")]
        [Required(ErrorMessage = "Vui lòng nhập mã khách hàng.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string Makh { get; set; } = null!;
        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Tenkh { get; set; }
        [Display(Name = "Năm sinh")]
        [Required(ErrorMessage = "Vui lòng nhập năm sinh.")]
        public int? Namsinh { get; set; }
        [Display(Name = "Phái")]
        [Required(ErrorMessage = "Vui lòng nhập phái.")]
        public bool? Phai { get; set; }
        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập điện thoại.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        [RegularExpression(@"[+0-9 ()\-]{8,20}", ErrorMessage = "Điện thoại phải có 8–20 ký tự số, dấu +, -, () hoặc khoảng trắng.")]
        public string? Dienthoai { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Diachi { get; set; }
        [Display(Name = "Mật khẩu")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public virtual ICollection<Phieudathang> Phieudathangs { get; set; }
    }
}
