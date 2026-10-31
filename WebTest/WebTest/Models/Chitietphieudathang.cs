using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Chitietphieudathang
    {
        public string Mapdh { get; set; } = null!;
        public string Mahang { get; set; } = null!;
        public double? Dongia { get; set; }
        public double? Soluong { get; set; }

        public virtual Hanghoa MahangNavigation { get; set; } = null!;
        public virtual Phieudathang MapdhNavigation { get; set; } = null!;
    }
}
