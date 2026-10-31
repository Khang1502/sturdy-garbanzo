-- Chỉ tạo cơ sở dữ liệu mới; không thay đổi CSDL đã tồn tại.
USE [master];
GO
IF DB_ID(N'QLHV_WebTestLab') IS NOT NULL
BEGIN
    PRINT N'CSDL QLHV_WebTestLab đã tồn tại; không có dữ liệu nào bị thay đổi.';
END
ELSE
BEGIN
    EXEC(N'CREATE DATABASE [QLHV_WebTestLab] COLLATE Vietnamese_CI_AS;');
    EXEC(N'USE [QLHV_WebTestLab];
SET XACT_ABORT ON;
BEGIN TRANSACTION;
CREATE TABLE [lop] (
    [malop] nvarchar(50) NOT NULL,
    [tenlop] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_lop] PRIMARY KEY ([malop])
);

CREATE TABLE [monhoc] (
    [msmh] nvarchar(50) NOT NULL,
    [tenmh] nvarchar(50) NOT NULL,
    [sotiet] int NOT NULL,
    CONSTRAINT [PK_monhoc] PRIMARY KEY ([msmh])
);

CREATE TABLE [lylich] (
    [mshv] nvarchar(50) NOT NULL,
    [tenhv] nvarchar(50) NOT NULL,
    [ngaysinh] datetime2 NOT NULL,
    [phai] bit NOT NULL,
    [malop] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_lylich] PRIMARY KEY ([mshv]),
    CONSTRAINT [FK_lylich_lop_malop] FOREIGN KEY ([malop]) REFERENCES [lop] ([malop]) ON DELETE NO ACTION
);

CREATE TABLE [diemthi] (
    [mshv] nvarchar(50) NOT NULL,
    [msmh] nvarchar(50) NOT NULL,
    [diem] float NOT NULL,
    CONSTRAINT [PK_diemthi] PRIMARY KEY ([mshv], [msmh]),
    CONSTRAINT [FK_diemthi_lylich_mshv] FOREIGN KEY ([mshv]) REFERENCES [lylich] ([mshv]) ON DELETE NO ACTION,
    CONSTRAINT [FK_diemthi_monhoc_msmh] FOREIGN KEY ([msmh]) REFERENCES [monhoc] ([msmh]) ON DELETE NO ACTION
);

CREATE INDEX [IX_diemthi_msmh] ON [diemthi] ([msmh]);

CREATE INDEX [IX_lylich_malop] ON [lylich] ([malop]);

INSERT INTO lop (malop, tenlop) VALUES (N''CNTT01'', N''Công nghệ thông tin 01'');
INSERT INTO lylich (mshv, tenhv, ngaysinh, phai, malop) VALUES (N''HV01'', N''Học viên mẫu'', ''20040101'', 1, N''CNTT01'');
INSERT INTO monhoc (msmh, tenmh, sotiet) VALUES (N''LTWIN'', N''Lập trình Windows'', 45);
INSERT INTO diemthi (mshv, msmh, diem) VALUES (N''HV01'', N''LTWIN'', 8.5);
COMMIT;');
END;
GO
