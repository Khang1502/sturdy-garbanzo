using System;
using System.Collections.Generic;

namespace WebTest.Models
{
    public partial class Phieugiaohang
    {
        public Phieugiaohang()
        {
            Chitietphieugiaohangs = new HashSet<Chitietphieugiaohang>();
        }

        public string Mapgh { get; set; } = null!;
        public DateTime? Ngaygh { get; set; }
        public string? Diachigh { get; set; }
        public string? Tennguoinhan { get; set; }
        public string? Mapdh { get; set; }
        public string? Manv { get; set; }

        public virtual Nhanvien? ManvNavigation { get; set; }
        public virtual Phieudathang? MapdhNavigation { get; set; }
        public virtual ICollection<Chitietphieugiaohang> Chitietphieugiaohangs { get; set; }
    }
}
