using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Hanghoa
    {
        public Hanghoa()
        {
            Chitietphieudathangs = new HashSet<Chitietphieudathang>();
            Chitietphieugiaohangs = new HashSet<Chitietphieugiaohang>();
        }

        [Display(Name = "Mã hàng hóa")]
        [Required(ErrorMessage = "Vui lòng nhập mã hàng hóa.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string Mahang { get; set; } = null!;
        [Display(Name = "Tên hàng hóa")]
        [Required(ErrorMessage = "Vui lòng nhập tên hàng hóa.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Tenhang { get; set; }
        [Display(Name = "Đơn vị tính")]
        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Donvitinh { get; set; }
        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Vui lòng nhập đơn giá.")]
        [Range(0, 1000000000000d, ErrorMessage = "Đơn giá phải từ 0 đến 1.000 tỷ.")]
        public double? Dongia { get; set; }
        [Display(Name = "Hình ảnh")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Hinh { get; set; }
        [Display(Name = "Loại hàng hóa")]
        [Required(ErrorMessage = "Vui lòng nhập loại hàng hóa.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Maloai { get; set; }
        [Display(Name = "Nhà sản xuất")]
        [Required(ErrorMessage = "Vui lòng nhập nhà sản xuất.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Mansx { get; set; }

        public virtual Loaihanghoa? MaloaiNavigation { get; set; }
        public virtual Nhasanxuat? MansxNavigation { get; set; }
        public virtual ICollection<Chitietphieudathang> Chitietphieudathangs { get; set; }
        public virtual ICollection<Chitietphieugiaohang> Chitietphieugiaohangs { get; set; }
    }
}
