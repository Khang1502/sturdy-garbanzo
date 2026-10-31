using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Loaihanghoa
    {
        public Loaihanghoa()
        {
            Hanghoas = new HashSet<Hanghoa>();
        }

        [Display(Name = "Mã loại hàng hóa")]
        [Required(ErrorMessage = "Vui lòng nhập mã loại hàng hóa.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string Maloai { get; set; } = null!;
        [Display(Name = "Tên loại hàng hóa")]
        [Required(ErrorMessage = "Vui lòng nhập tên loại hàng hóa.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Tenloai { get; set; }

        public virtual ICollection<Hanghoa> Hanghoas { get; set; }
    }
}
