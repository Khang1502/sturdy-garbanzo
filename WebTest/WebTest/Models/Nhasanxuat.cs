using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Nhasanxuat
    {
        public Nhasanxuat()
        {
            Hanghoas = new HashSet<Hanghoa>();
        }

        [Display(Name = "Mã nhà sản xuất")]
        [Required(ErrorMessage = "Vui lòng nhập mã nhà sản xuất.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string Mansx { get; set; } = null!;
        [Display(Name = "Tên nhà sản xuất")]
        [Required(ErrorMessage = "Vui lòng nhập tên nhà sản xuất.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Tennsx { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự.")]
        public string? Diachi { get; set; }

        public virtual ICollection<Hanghoa> Hanghoas { get; set; }
    }
}
