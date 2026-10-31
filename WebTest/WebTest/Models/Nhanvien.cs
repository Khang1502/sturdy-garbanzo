using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Nhanvien
    {
        public Nhanvien()
        {
            Phieugiaohangs = new HashSet<Phieugiaohang>();
        }

        [Display(Name = "Mã nhân viên")]
        [Required(ErrorMessage = "Vui lòng nhập mã nhân viên.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string Manv { get; set; } = null!;
        [Display(Name = "Tên nhân viên")]
        [Required(ErrorMessage = "Vui lòng nhập tên nhân viên.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Tennv { get; set; }
        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        public DateTime? Ngaysinh { get; set; }
        [Display(Name = "Phái")]
        [Required(ErrorMessage = "Vui lòng nhập phái.")]
        public bool? Phai { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Diachi { get; set; }
        [Display(Name = "Mật khẩu")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public virtual ICollection<Phieugiaohang> Phieugiaohangs { get; set; }
    }
}
