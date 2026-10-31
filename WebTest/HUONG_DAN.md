# WebTest — Bài thực hành 1–8

Bản chỉnh từ HGK.zip, giữ tên solution WebTest.sln, namespace WebTest,
.NET 6 và EF Core SQL Server 6.0.36. Bài 3 QLHV được tích hợp thành các
mục riêng, dùng QLHVContext và cơ sở dữ liệu riêng, không trộn với QLBH.

## Chạy trên Windows / Visual Studio 2022

1. Giải nén toàn bộ ZIP. Cài Visual Studio 2022 với workload ASP.NET and web
   development, .NET 6 SDK và SQL Server Express LocalDB (hoặc SQL Server).
2. Dùng SQL Server Management Studio kết nối `(localdb)\MSSQLLocalDB` bằng
   Windows Authentication. Chạy lần lượt `Database/QLBH.sql` và `Database/QLHV.sql`.
   Hai script tạo CSDL mới `QLBH_WebTestLab` và `QLHV_WebTestLab`, có dữ liệu mẫu.
   Nếu CSDL cùng tên đã tồn tại, script thông báo và không thay đổi dữ liệu.
   Chưa có bản sao lưu dữ liệu gốc trong HGK.zip; dữ liệu đi kèm là mẫu mới.
3. Mở `WebTest.sln`. Nếu dùng instance SQL khác, sửa hai connection strings
   trong `WebTest/appsettings.json`. Có thể trỏ QLBH về CSDL cũ đúng schema thay
   vì dùng CSDL mẫu. Không cần chạy migration hay xóa CSDL cũ.
4. Đặt **WebTest** làm Startup Project (không chọn WebTest.Tests), chọn profile
   WebTest, restore NuGet và nhấn F5. Truy cập trang chủ qua trình duyệt được mở.

Tài khoản khách hàng mẫu: **KH01 / Demo123!**. Có thể tự đăng ký tài khoản mới.
Để chạy bằng dòng lệnh, tại thư mục chứa solution:

```sh
dotnet restore WebTest.sln
dotnet run --project WebTest/WebTest.csproj
```

LocalDB chỉ chạy trên Windows. Trên Linux/macOS cần SQL Server riêng và thay
connection strings, hoặc chạy riêng bộ test SQLite dưới đây.
Ứng dụng không tự tạo hay thay đổi CSDL khi khởi động.

## Đối chiếu bài lab

| Bài | Chức năng trong bản sửa |
| --- | --- |
| 1–2 | Xem/thêm/sửa/xóa nhà sản xuất, loại hàng, nhân viên, khách hàng, hàng hóa; sửa thiếu action GET thêm nhà sản xuất; kiểm tra trùng khóa chính. |
| 3 | QLHV: CRUD lý lịch học viên, chọn lớp/phái bằng Tag Helpers; thêm CRUD lớp, môn học, điểm thi với khóa ghép học viên–môn học và kiểm tra khóa ngoại. Mở menu “Bài 3: Lý lịch học viên”. |
| 4 | Layout Bootstrap dùng chung: header, thanh điều hướng, menu trái, nội dung và footer; liên kết đến năm nhóm quản lý. Dùng Bootstrap/jQuery có sẵn trong ZIP, không phụ thuộc CDN. |
| 5 | DataAnnotations, thông báo lỗi tại form, kiểm tra ngày/năm sinh, đơn giá, điện thoại, độ dài trường; kiểm tra quan hệ trước khi hiện nút xóa và kiểm tra lại phía server khi POST. |
| 6 | Session 60 phút: đăng ký, đăng nhập, đăng xuất về quản lý nhân viên, nhấn tên để sửa hồ sơ; ID tài khoản lấy từ session. Dùng System.Text.Json có sẵn cho giỏ hàng, không cần Newtonsoft.Json. |
| 7 | Hàng hóa có tên loại/nhà sản xuất, trang chi tiết; upload ảnh, xem trước, jQuery Ajax hiện thông tin loại/nhà sản xuất khi chọn dropdown; kiểm tra trùng mã và FK. |
| 8 | Trang chủ dạng lưới 4 cột trên màn hình lớn; chọn số lượng, cộng dồn vào giỏ, xem/xóa/trở về, phân biệt đăng nhập và giỏ trống; lập phiếu đặt hàng + chi tiết trong một SaveChanges có transaction. |

Mẫu layout giữ bố cục của giáo trình, dùng Bootstrap đi kèm project; không sao
chép từng pixel ảnh minh họa. Bài làm thêm tải hai template bên ngoài và tạo
project rỗng riêng (Bài 4) không nằm trong bản sửa này. Không mở rộng thành phần
quản lý phiếu giao hàng riêng hoặc Bài 10.

## Các điểm cần biết khi xem mã

- Bản gốc HGK.zip không kèm SQL: schema QLBH được sinh từ scaffold gốc. Fluent
  mapping giữ tính nullable và quan hệ gốc dù form có thêm Required validation.
- Tên trường/khóa giữ nguyên. Các thao tác thay đổi dùng POST + antiforgery.
- Không đưa mật khẩu vào session hay vào trang danh sách/sửa; ô mật khẩu để
  trống khi sửa nghĩa là giữ nguyên. Cách đối chiếu mật khẩu dạng rõ giữ tương
  thích với dữ liệu và ví dụ lab; đây là bài thực hành, chưa phải hệ thống
  đăng nhập/phân quyền để triển khai thực tế. Các menu quản lý vẫn mở như lab.
- Upload ở `WebTest/wwwroot/Hinh`, tối đa 2 MB, tên ngẫu nhiên, kiểm tra phần
  mở rộng và chữ ký tệp. Giữ thư mục ảnh khi chuyển máy. Hàng chưa có ảnh dùng
  ảnh giữ chỗ. Thư mục phải cho phép ứng dụng ghi.
- Giỏ hàng lưu ID/số lượng ở session. Giá lấy lại từ CSDL khi đặt; không nhận
  giá từ client. Session/giỏ mất khi khởi động lại server hoặc hết hạn.
- Không kèm bin, obj, .vs và csproj.user của máy cũ. ZIP nguồn ban đầu không
  bị thay đổi; bản sửa nằm độc lập.

## Kiểm tra

```sh
dotnet test WebTest.sln
```

Đã biên dịch solution và chạy **20 ca kiểm thử: 20 đạt, 0 lỗi, 0 bỏ qua** trên
.NET SDK 6.0.428 / ASP.NET Core 6.0.36. Bộ test sử dụng MVC TestServer và hai
SQLite in-memory riêng, không kết nối hoặc sửa SQL Server thật.
Kiểm tra các trang CRUD và layout, CRUD cả năm nhóm, khóa chính/khóa ngoại,
validation, CSRF, tài khoản/hồ sơ/session, Ajax JSON, ảnh hợp lệ/không hợp lệ,
đơn giá âm, giỏ hàng, đơn hàng/chi tiết và QLHV. Báo cáo: `Validation/lab-tests.trx`.
Schema SQL Server được sinh và kiểm tra cấu trúc bằng provider SqlServer.

Chưa chạy hai script trên một SQL Server thật và chưa kiểm tra tương tác
JavaScript bằng trình duyệt; cần kiểm tra bước này khi mở project trên máy bạn.

## Thử nhanh sau khi F5

1. Xem 2 hàng mẫu ở trang chủ, chọn mua cùng hàng hai lần để thấy cộng số lượng.
2. Mở giỏ khi chưa đăng nhập: có thông báo cần đăng nhập và chưa có nút lập đơn.
3. Đăng nhập KH01, trở lại giỏ, nhập địa chỉ/ngày giao và lập đơn.
4. Vào quản lý hàng hóa: thêm hàng, chọn loại/NSX, chọn ảnh để xem trước, lưu,
   xem chi tiết, sửa. Thử mã trùng và đơn giá âm để xem validation.
5. Thử xóa NSX01 đang có hàng: phải bị chặn. Tạo nhà sản xuất mới chưa có hàng
   rồi xóa: phải thành công.
6. Mở QLHV: thêm lớp → thêm học viên → thêm môn → thêm điểm. Xóa học viên có
   điểm bị chặn; xóa điểm trước rồi mới xóa học viên.
