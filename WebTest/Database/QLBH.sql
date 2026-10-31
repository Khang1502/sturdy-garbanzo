-- Chỉ tạo cơ sở dữ liệu mới; không thay đổi CSDL đã tồn tại.
USE [master];
GO
IF DB_ID(N'QLBH_WebTestLab') IS NOT NULL
BEGIN
    PRINT N'CSDL QLBH_WebTestLab đã tồn tại; không có dữ liệu nào bị thay đổi.';
END
ELSE
BEGIN
    EXEC(N'CREATE DATABASE [QLBH_WebTestLab] COLLATE Vietnamese_CI_AS;');
    EXEC(N'USE [QLBH_WebTestLab];
SET XACT_ABORT ON;
BEGIN TRANSACTION;
CREATE TABLE [khachhang] (
    [makh] nvarchar(50) NOT NULL,
    [tenkh] nvarchar(50) NULL,
    [namsinh] int NULL,
    [phai] bit NULL,
    [dienthoai] nvarchar(50) NULL,
    [diachi] nvarchar(50) NULL,
    [password] nvarchar(50) NULL,
    CONSTRAINT [PK_khachhang] PRIMARY KEY ([makh])
);

CREATE TABLE [loaihanghoa] (
    [maloai] nvarchar(50) NOT NULL,
    [tenloai] nvarchar(50) NULL,
    CONSTRAINT [PK_loaihanghoa] PRIMARY KEY ([maloai])
);

CREATE TABLE [nhanvien] (
    [manv] nvarchar(50) NOT NULL,
    [tennv] nvarchar(50) NULL,
    [ngaysinh] datetime NULL,
    [phai] bit NULL,
    [diachi] nvarchar(50) NULL,
    [password] nvarchar(50) NULL,
    CONSTRAINT [PK_nhanvien] PRIMARY KEY ([manv])
);

CREATE TABLE [nhasanxuat] (
    [mansx] nvarchar(50) NOT NULL,
    [tennsx] nvarchar(50) NULL,
    [diachi] nvarchar(50) NULL,
    CONSTRAINT [PK_nhasanxuat] PRIMARY KEY ([mansx])
);

CREATE TABLE [phieudathang] (
    [mapdh] nvarchar(50) NOT NULL,
    [ngaydh] datetime NULL,
    [ngaygh] datetime NULL,
    [diachigh] nvarchar(50) NULL,
    [makh] nvarchar(50) NULL,
    CONSTRAINT [PK_phieudathang] PRIMARY KEY ([mapdh]),
    CONSTRAINT [FK_phieudathang_khachhang] FOREIGN KEY ([makh]) REFERENCES [khachhang] ([makh])
);

CREATE TABLE [hanghoa] (
    [mahang] nvarchar(50) NOT NULL,
    [tenhang] nvarchar(50) NULL,
    [donvitinh] nvarchar(50) NULL,
    [dongia] float NULL,
    [hinh] nvarchar(50) NULL,
    [maloai] nvarchar(50) NULL,
    [mansx] nvarchar(50) NULL,
    CONSTRAINT [PK_hanghoa] PRIMARY KEY ([mahang]),
    CONSTRAINT [FK_hanghoa_loaihanghoa] FOREIGN KEY ([maloai]) REFERENCES [loaihanghoa] ([maloai]),
    CONSTRAINT [FK_hanghoa_nhasanxuat] FOREIGN KEY ([mansx]) REFERENCES [nhasanxuat] ([mansx])
);

CREATE TABLE [phieugiaohang] (
    [mapgh] nvarchar(50) NOT NULL,
    [ngaygh] datetime NULL,
    [diachigh] nvarchar(50) NULL,
    [tennguoinhan] nvarchar(50) NULL,
    [mapdh] nvarchar(50) NULL,
    [manv] nvarchar(50) NULL,
    CONSTRAINT [PK_phieugiaohang] PRIMARY KEY ([mapgh]),
    CONSTRAINT [FK_phieugiaohang_nhanvien] FOREIGN KEY ([manv]) REFERENCES [nhanvien] ([manv]),
    CONSTRAINT [FK_phieugiaohang_phieudathang] FOREIGN KEY ([mapdh]) REFERENCES [phieudathang] ([mapdh])
);

CREATE TABLE [chitietphieudathang] (
    [mapdh] nvarchar(50) NOT NULL,
    [mahang] nvarchar(50) NOT NULL,
    [dongia] float NULL,
    [soluong] float NULL,
    CONSTRAINT [PK_chitietphieudathang] PRIMARY KEY ([mapdh], [mahang]),
    CONSTRAINT [FK_chitietphieudathang_hanghoa] FOREIGN KEY ([mahang]) REFERENCES [hanghoa] ([mahang]),
    CONSTRAINT [FK_chitietphieudathang_phieudathang] FOREIGN KEY ([mapdh]) REFERENCES [phieudathang] ([mapdh])
);

CREATE TABLE [chitietphieugiaohang] (
    [mapgh] nvarchar(50) NOT NULL,
    [mahang] nvarchar(50) NOT NULL,
    [donvitinh] nvarchar(50) NULL,
    [soluong] float NULL,
    CONSTRAINT [PK_chitietphieugiaohang] PRIMARY KEY ([mapgh], [mahang]),
    CONSTRAINT [FK_chitietphieugiaohang_hanghoa] FOREIGN KEY ([mahang]) REFERENCES [hanghoa] ([mahang]),
    CONSTRAINT [FK_chitietphieugiaohang_phieugiaohang] FOREIGN KEY ([mapgh]) REFERENCES [phieugiaohang] ([mapgh])
);

CREATE INDEX [IX_chitietphieudathang_mahang] ON [chitietphieudathang] ([mahang]);

CREATE INDEX [IX_chitietphieugiaohang_mahang] ON [chitietphieugiaohang] ([mahang]);

CREATE INDEX [IX_hanghoa_maloai] ON [hanghoa] ([maloai]);

CREATE INDEX [IX_hanghoa_mansx] ON [hanghoa] ([mansx]);

CREATE INDEX [IX_phieudathang_makh] ON [phieudathang] ([makh]);

CREATE INDEX [IX_phieugiaohang_manv] ON [phieugiaohang] ([manv]);

CREATE INDEX [IX_phieugiaohang_mapdh] ON [phieugiaohang] ([mapdh]);

INSERT INTO nhasanxuat (mansx, tennsx, diachi) VALUES (N''NSX01'', N''Nhà sản xuất mẫu'', N''TP.HCM'');
INSERT INTO loaihanghoa (maloai, tenloai) VALUES (N''L01'', N''Đồ dùng học tập'');
INSERT INTO nhanvien (manv, tennv, ngaysinh, phai, diachi, password) VALUES (N''NV01'', N''Nhân viên mẫu'', ''20000101'', 1, N''TP.HCM'', N''Demo123!'');
INSERT INTO khachhang (makh, tenkh, namsinh, phai, dienthoai, diachi, password) VALUES (N''KH01'', N''Khách hàng mẫu'', 2000, 1, N''0901234567'', N''TP.HCM'', N''Demo123!'');
INSERT INTO hanghoa (mahang, tenhang, donvitinh, dongia, maloai, mansx) VALUES
(N''HH01'', N''Vở học sinh'', N''Quyển'', 15000, N''L01'', N''NSX01''),
(N''HH02'', N''Bút bi'', N''Cây'', 5000, N''L01'', N''NSX01'');
COMMIT;');
END;
GO
