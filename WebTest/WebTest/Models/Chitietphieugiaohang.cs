using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Chitietphieugiaohang
    {
        public string Mapgh { get; set; } = null!;
        public string Mahang { get; set; } = null!;
        public string? Donvitinh { get; set; }
        public double? Soluong { get; set; }

        public virtual Hanghoa MahangNavigation { get; set; } = null!;
        public virtual Phieugiaohang MapghNavigation { get; set; } = null!;
    }
}
