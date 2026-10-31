using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Phieudathang
    {
        public Phieudathang()
        {
            Chitietphieudathangs = new HashSet<Chitietphieudathang>();
            Phieugiaohangs = new HashSet<Phieugiaohang>();
        }

        public string Mapdh { get; set; } = null!;
        public DateTime? Ngaydh { get; set; }
        public DateTime? Ngaygh { get; set; }
        public string? Diachigh { get; set; }
        public string? Makh { get; set; }

        public virtual Khachhang? MakhNavigation { get; set; }
        public virtual ICollection<Chitietphieudathang> Chitietphieudathangs { get; set; }
        public virtual ICollection<Phieugiaohang> Phieugiaohangs { get; set; }
    }
}
