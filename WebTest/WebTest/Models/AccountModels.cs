using System.ComponentModel.DataAnnotations;
namespace WebTest.Models;
public class LoginModel {
    [Required(ErrorMessage = "Vui lòng nhập mã khách hàng."), StringLength(50), Display(Name = "Mã khách hàng")]
    public string Makh { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu."), StringLength(50), DataType(DataType.Password), Display(Name = "Mật khẩu")]
    public string Password { get; set; } = "";
}
public class CartItem {
    public string Mahang { get; set; } = "";
    public int Soluong { get; set; }
}
public class CartLine {
    public Hanghoa Hang { get; set; } = null!;
    public int Soluong { get; set; }
    public decimal ThanhTien => (decimal)(Hang.Dongia ?? 0) * Soluong;
}
public class CartModel {
    public List<CartLine> Lines { get; set; } = new();
    public decimal TongTien => Lines.Sum(x => x.ThanhTien);
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng."), StringLength(50), Display(Name = "Địa chỉ giao hàng")]
    public string? Diachigh { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn ngày giao hàng."), DataType(DataType.Date), Display(Name = "Ngày giao hàng")]
    public DateTime? Ngaygh { get; set; } = DateTime.Today.AddDays(1);
}
