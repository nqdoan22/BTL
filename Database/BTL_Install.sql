-- =========================================================================
-- BÀI TẬP LỚN CUỐI KỲ - HỆ CƠ SỞ DỮ LIỆU & THUẬT TOÁN (BÀI 1 ĐẾN BÀI 8)
-- Chạy trên MySQL 8.0+ (MySQL Workbench hoặc mysql client).
-- =========================================================================

CREATE DATABASE IF NOT EXISTS BTL_ThuVien CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS BTL_DeAn CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- =========================================================================
-- PHẦN 1: CSDL BTL_ThuVien (BÀI 1, 2, 3, 4, 5, 6)
-- =========================================================================
USE BTL_ThuVien;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS Muon, QuaTrinhMuon, DangKy, Cuonsach, Dausach, Treem, Nguoilon, Tuasach, DocGia, ThongBao;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE DocGia (
    ma_DocGia VARCHAR(10) PRIMARY KEY,
    ho VARCHAR(30),
    tenlot VARCHAR(30),
    ten VARCHAR(30),
    ngaysinh DATE NOT NULL
);

CREATE TABLE Nguoilon (
    ma_DocGia VARCHAR(10) PRIMARY KEY,
    sonha VARCHAR(20),
    duong VARCHAR(80),
    quan VARCHAR(40),
    dienthoai VARCHAR(20),
    han_sd DATE NOT NULL,
    FOREIGN KEY (ma_DocGia) REFERENCES DocGia(ma_DocGia)
);

CREATE TABLE Treem (
    ma_DocGia VARCHAR(10) PRIMARY KEY,
    ma_DocGia_nguoilon VARCHAR(10) NOT NULL,
    FOREIGN KEY (ma_DocGia) REFERENCES DocGia(ma_DocGia),
    FOREIGN KEY (ma_DocGia_nguoilon) REFERENCES Nguoilon(ma_DocGia)
);

CREATE TABLE Tuasach (
    ma_tuasach INT AUTO_INCREMENT PRIMARY KEY,
    tuasach VARCHAR(120) NOT NULL,
    tacgia VARCHAR(100),
    tomtat VARCHAR(500)
);

CREATE TABLE Dausach (
    isbn VARCHAR(20) PRIMARY KEY,
    ma_tuasach INT NOT NULL,
    ngonngu VARCHAR(30),
    bia VARCHAR(30),
    trangthai ENUM('yes','no') NOT NULL DEFAULT 'yes',
    FOREIGN KEY (ma_tuasach) REFERENCES Tuasach(ma_tuasach)
);

CREATE TABLE Cuonsach (
    isbn VARCHAR(20) NOT NULL,
    ma_cuonsach INT NOT NULL,
    tinhtrang ENUM('yes','no') NOT NULL DEFAULT 'yes',
    PRIMARY KEY (isbn, ma_cuonsach),
    FOREIGN KEY (isbn) REFERENCES Dausach(isbn)
);

CREATE TABLE DangKy (
    isbn VARCHAR(20),
    ma_DocGia VARCHAR(10),
    ngay_dk DATE NOT NULL,
    ghichu VARCHAR(200),
    PRIMARY KEY (isbn, ma_DocGia),
    FOREIGN KEY (isbn) REFERENCES Dausach(isbn),
    FOREIGN KEY (ma_DocGia) REFERENCES DocGia(ma_DocGia)
);

CREATE TABLE Muon (
    isbn VARCHAR(20) NOT NULL,
    ma_cuonsach INT NOT NULL,
    ma_DocGia VARCHAR(10) NOT NULL,
    ngay_muon DATE NOT NULL,
    ngay_hethan DATE NOT NULL,
    PRIMARY KEY (isbn, ma_cuonsach),
    FOREIGN KEY (isbn, ma_cuonsach) REFERENCES Cuonsach(isbn, ma_cuonsach),
    FOREIGN KEY (ma_DocGia) REFERENCES DocGia(ma_DocGia)
);

CREATE TABLE QuaTrinhMuon (
    isbn VARCHAR(20),
    ma_cuonsach INT,
    ngay_muon DATE,
    ma_DocGia VARCHAR(10),
    ngay_hethan DATE,
    ngay_tra DATE,
    tien_muon DECIMAL(12,2),
    tien_datra DECIMAL(12,2),
    tien_datcoc DECIMAL(12,2),
    ghichu VARCHAR(200)
);

CREATE TABLE ThongBao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    noi_dung VARCHAR(255) NOT NULL,
    thoi_gian TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- -------------------------------------------------------------------------
-- DỮ LIỆU MẪU CSDL THƯ VIỆN (Đủ để test mọi trường hợp)
-- -------------------------------------------------------------------------
INSERT INTO DocGia VALUES 
('DG01','Nguyen','Van','An','1995-01-10'),
('DG02','Tran','Thi','Binh','2012-04-12'),
('DG03','Le','Minh','Chau','1998-08-20'),
('DG04','Pham','Hoang','Nam','2014-06-15');

INSERT INTO Nguoilon VALUES 
('DG01','12','Le Loi','Quan 1','0900000001','2028-12-31'),
('DG03','20','Nguyen Hue','Quan 1','0900000003','2028-12-31');

INSERT INTO Treem VALUES 
('DG02','DG01'),
('DG04','DG03');

INSERT INTO Tuasach (tuasach,tacgia,tomtat) VALUES 
('SQL Server can ban','Pham Van A','Giao trinh thuc hanh SQL Server'),
('Lap trinh C#','Tran Van B','WinForms va ADO.NET'),
('Cau truc du lieu','Nguyen Van C','Giai thuat va lap trinh');

INSERT INTO Dausach VALUES 
('ISBN001',1,'Tieng Viet','Cung','yes'),
('ISBN002',2,'Tieng Viet','Mem','yes'),
('ISBN003',3,'Tieng Anh','Cung','no');

INSERT INTO Cuonsach VALUES 
('ISBN001',1,'yes'),
('ISBN001',2,'no'),
('ISBN002',1,'yes'),
('ISBN002',2,'yes'),
('ISBN003',1,'no');

-- Dữ liệu mượn sách:
-- DG01 mượn quá hạn > 14 ngày (hạn trả cách đây 30 ngày) -> Test Bài 5d
-- DG01 mượn sách và DG02 (trẻ em do DG01 bảo lãnh) cũng đang mượn -> Test Bài 5e
INSERT INTO Muon VALUES 
('ISBN001',2,'DG01', DATE_SUB(CURDATE(), INTERVAL 45 DAY), DATE_SUB(CURDATE(), INTERVAL 30 DAY)),
('ISBN002',1,'DG02', DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_ADD(CURDATE(), INTERVAL 10 DAY));

DELIMITER $$

-- =========================================================================
-- BÀI 1: Stored Procedure giải phương trình bậc 1: ax + b = 0
-- =========================================================================
DROP PROCEDURE IF EXISTS sp_GiaiPTBac1$$
CREATE PROCEDURE sp_GiaiPTBac1(IN p_a DOUBLE, IN p_b DOUBLE)
BEGIN
    IF p_a = 0 THEN
        IF p_b = 0 THEN
            SELECT 'Phương trình có vô số nghiệm' AS KetLuan, NULL AS Nghiem;
        ELSE
            SELECT 'Phương trình vô nghiệm' AS KetLuan, NULL AS Nghiem;
        END IF;
    ELSE
        SELECT 'Phương trình có 1 nghiệm duy nhất' AS KetLuan, ROUND(-p_b / p_a, 4) AS Nghiem;
    END IF;
END$$

-- =========================================================================
-- BÀI 2: Hàm giải phương trình bậc 2: ax² + bx + c = 0
-- =========================================================================
DROP FUNCTION IF EXISTS fn_GiaiPTBac2$$
CREATE FUNCTION fn_GiaiPTBac2(p_a DOUBLE, p_b DOUBLE, p_c DOUBLE)
RETURNS VARCHAR(255)
DETERMINISTIC
BEGIN
    DECLARE v_delta DOUBLE;
    DECLARE v_x1 DOUBLE;
    DECLARE v_x2 DOUBLE;

    IF p_a = 0 THEN
        IF p_b = 0 THEN
            IF p_c = 0 THEN
                RETURN 'Phương trình có vô số nghiệm (x ∈ ℝ)';
            ELSE
                RETURN 'Phương trình vô nghiệm';
            END IF;
        ELSE
            RETURN CONCAT('Hạ bậc 1: nghiệm x = ', ROUND(-p_c / p_b, 4));
        END IF;
    ELSE
        SET v_delta = p_b * p_b - 4 * p_a * p_c;
        IF v_delta < 0 THEN
            RETURN 'Phương trình vô nghiệm thực (Δ < 0)';
        ELSEIF v_delta = 0 THEN
            SET v_x1 = -p_b / (2 * p_a);
            RETURN CONCAT('Nghiệm kép: x₁ = x₂ = ', ROUND(v_x1, 4));
        ELSE
            SET v_x1 = (-p_b + SQRT(v_delta)) / (2 * p_a);
            SET v_x2 = (-p_b - SQRT(v_delta)) / (2 * p_a);
            RETURN CONCAT('2 nghiệm: x₁ = ', ROUND(v_x1, 4), ' ; x₂ = ', ROUND(v_x2, 4));
        END IF;
    END IF;
END$$

DROP PROCEDURE IF EXISTS sp_GiaiPTBac2$$
CREATE PROCEDURE sp_GiaiPTBac2(IN p_a DOUBLE, IN p_b DOUBLE, IN p_c DOUBLE)
BEGIN
    SELECT fn_GiaiPTBac2(p_a, p_b, p_c) AS KetQua;
END$$

-- =========================================================================
-- BÀI 3: Stored Procedure thông tin đầu sách và số lượng còn lại
-- =========================================================================
DROP PROCEDURE IF EXISTS sp_ThongtinDausach$$
CREATE PROCEDURE sp_ThongtinDausach(IN p_isbn VARCHAR(20))
BEGIN
    SELECT d.isbn, t.tuasach, t.tacgia, t.tomtat, d.ngonngu, d.bia, d.trangthai,
           COALESCE(SUM(c.tinhtrang = 'yes'), 0) AS so_luong_chua_muon
    FROM Dausach d
    JOIN Tuasach t ON t.ma_tuasach = d.ma_tuasach
    LEFT JOIN Cuonsach c ON c.isbn = d.isbn
    WHERE d.isbn = p_isbn
    GROUP BY d.isbn, t.tuasach, t.tacgia, t.tomtat, d.ngonngu, d.bia, d.trangthai;
END$$

-- =========================================================================
-- BÀI 4: Hàm tính tuổi theo năm sinh
-- =========================================================================
DROP FUNCTION IF EXISTS fn_TinhTuoi$$
CREATE FUNCTION fn_TinhTuoi(p_namsinh INT)
RETURNS INT
DETERMINISTIC
BEGIN
    RETURN YEAR(CURDATE()) - p_namsinh;
END$$

DROP PROCEDURE IF EXISTS sp_TinhTuoi$$
CREATE PROCEDURE sp_TinhTuoi(IN p_namsinh INT)
BEGIN
    SELECT p_namsinh AS NamSinh, YEAR(CURDATE()) AS NamHienTai, fn_TinhTuoi(p_namsinh) AS Tuoi;
END$$

-- =========================================================================
-- BÀI 5: Các Stored Procedure tra cứu CSDL Thư viện
-- =========================================================================

-- 5a. Xem thông tin độc giả (kiểm tra người lớn / trẻ em theo đúng 3 bước của đề)
DROP PROCEDURE IF EXISTS sp_ThongtinDocGia$$
CREATE PROCEDURE sp_ThongtinDocGia(IN p_maDocGia VARCHAR(10))
BEGIN
    IF EXISTS (SELECT 1 FROM Nguoilon WHERE ma_DocGia = p_maDocGia) THEN
        -- Độc giả người lớn
        SELECT d.ma_DocGia, d.ho, d.tenlot, d.ten, d.ngaysinh, 'Người lớn' AS loai_doc_gia,
               n.sonha, n.duong, n.quan, n.dienthoai, n.han_sd,
               NULL AS ma_nguoi_bao_lanh, NULL AS ho_ten_nguoi_bao_lanh
        FROM DocGia d
        JOIN Nguoilon n ON n.ma_DocGia = d.ma_DocGia
        WHERE d.ma_DocGia = p_maDocGia;
    ELSEIF EXISTS (SELECT 1 FROM Treem WHERE ma_DocGia = p_maDocGia) THEN
        -- Độc giả trẻ em
        SELECT d.ma_DocGia, d.ho, d.tenlot, d.ten, d.ngaysinh, 'Trẻ em' AS loai_doc_gia,
               nln.sonha, nln.duong, nln.quan, nln.dienthoai, nln.han_sd,
               tr.ma_DocGia_nguoilon AS ma_nguoi_bao_lanh,
               CONCAT_WS(' ', nl.ho, nl.tenlot, nl.ten) AS ho_ten_nguoi_bao_lanh
        FROM DocGia d
        JOIN Treem tr ON tr.ma_DocGia = d.ma_DocGia
        JOIN DocGia nl ON nl.ma_DocGia = tr.ma_DocGia_nguoilon
        JOIN Nguoilon nln ON nln.ma_DocGia = tr.ma_DocGia_nguoilon
        WHERE d.ma_DocGia = p_maDocGia;
    ELSE
        -- Độc giả chưa phân loại
        SELECT d.ma_DocGia, d.ho, d.tenlot, d.ten, d.ngaysinh, 'Chưa xác định' AS loai_doc_gia,
               NULL AS sonha, NULL AS duong, NULL AS quan, NULL AS dienthoai, NULL AS han_sd,
               NULL AS ma_nguoi_bao_lanh, NULL AS ho_ten_nguoi_bao_lanh
        FROM DocGia d
        WHERE d.ma_DocGia = p_maDocGia;
    END IF;
END$$

-- 5c. Liệt kê những độc giả người lớn đang mượn sách
DROP PROCEDURE IF EXISTS sp_ThongtinNguoilonDangmuon$$
CREATE PROCEDURE sp_ThongtinNguoilonDangmuon()
BEGIN
    SELECT DISTINCT d.ma_DocGia, d.ho, d.tenlot, d.ten, d.ngaysinh, n.dienthoai,
           m.isbn, m.ma_cuonsach, m.ngay_muon, m.ngay_hethan
    FROM DocGia d
    JOIN Nguoilon n ON n.ma_DocGia = d.ma_DocGia
    JOIN Muon m ON m.ma_DocGia = d.ma_DocGia;
END$$

-- 5d. Liệt kê độc giả người lớn mượn quá hạn > 14 ngày
DROP PROCEDURE IF EXISTS sp_ThongtinNguoilonQuahan$$
CREATE PROCEDURE sp_ThongtinNguoilonQuahan()
BEGIN
    SELECT DISTINCT d.ma_DocGia, d.ho, d.tenlot, d.ten, m.isbn, m.ma_cuonsach,
           m.ngay_hethan, DATEDIFF(CURDATE(), m.ngay_hethan) AS so_ngay_qua_han
    FROM DocGia d
    JOIN Nguoilon n ON n.ma_DocGia = d.ma_DocGia
    JOIN Muon m ON m.ma_DocGia = d.ma_DocGia
    WHERE DATEDIFF(CURDATE(), m.ngay_hethan) > 14;
END$$

-- 5e. Liệt kê độc giả người lớn đang mượn sách có trẻ em được bảo lãnh cũng đang mượn
DROP PROCEDURE IF EXISTS sp_DocGiaCoTreEmMuon$$
CREATE PROCEDURE sp_DocGiaCoTreEmMuon()
BEGIN
    SELECT DISTINCT nl.ma_DocGia AS ma_nguoi_lon, CONCAT_WS(' ', nl.ho, nl.tenlot, nl.ten) AS ho_ten_nguoi_lon,
           te.ma_DocGia AS ma_tre_em, CONCAT_WS(' ', te.ho, te.tenlot, te.ten) AS ho_ten_tre_em
    FROM DocGia nl
    JOIN Nguoilon n ON n.ma_DocGia = nl.ma_DocGia
    JOIN Treem tr ON tr.ma_DocGia_nguoilon = nl.ma_DocGia
    JOIN DocGia te ON te.ma_DocGia = tr.ma_DocGia
    JOIN Muon m1 ON m1.ma_DocGia = nl.ma_DocGia
    JOIN Muon m2 ON m2.ma_DocGia = te.ma_DocGia;
END$$

-- =========================================================================
-- BÀI 6: Các Trigger trong CSDL Thư viện
-- =========================================================================

-- 6.1. Xóa mượn sách -> Cập nhật tình trạng cuốn sách là 'yes'
DROP TRIGGER IF EXISTS tg_delMuon$$
CREATE TRIGGER tg_delMuon AFTER DELETE ON Muon FOR EACH ROW 
BEGIN 
    UPDATE Cuonsach SET tinhtrang = 'yes' 
    WHERE isbn = OLD.isbn AND ma_cuonsach = OLD.ma_cuonsach; 
END$$

-- 6.2. Thêm mượn sách -> Cập nhật tình trạng cuốn sách là 'no'
DROP TRIGGER IF EXISTS tg_insMuon$$
CREATE TRIGGER tg_insMuon AFTER INSERT ON Muon FOR EACH ROW 
BEGIN 
    UPDATE Cuonsach SET tinhtrang = 'no' 
    WHERE isbn = NEW.isbn AND ma_cuonsach = NEW.ma_cuonsach; 
END$$

-- 6.3. Cập nhật tình trạng cuốn sách -> Trạng thái đầu sách cập nhật theo
DROP TRIGGER IF EXISTS tg_updCuonSach$$
CREATE TRIGGER tg_updCuonSach AFTER UPDATE ON Cuonsach FOR EACH ROW 
BEGIN 
    UPDATE Dausach 
    SET trangthai = IF(EXISTS (SELECT 1 FROM Cuonsach WHERE isbn = NEW.isbn AND tinhtrang = 'yes'), 'yes', 'no')
    WHERE isbn = NEW.isbn; 
END$$

-- 6.4. Thêm mới tựa sách -> Ghi thông báo
DROP TRIGGER IF EXISTS tg_InfThongBao$$
CREATE TRIGGER tg_InfThongBao AFTER INSERT ON Tuasach FOR EACH ROW 
BEGIN 
    INSERT INTO ThongBao(noi_dung) VALUES ('Đã thêm mới tựa sách'); 
END$$

-- 6.4b. Sửa tựa sách / tên tác giả -> Ghi thông báo
DROP TRIGGER IF EXISTS tg_updTuasachThongBao$$
CREATE TRIGGER tg_updTuasachThongBao AFTER UPDATE ON Tuasach FOR EACH ROW 
BEGIN 
    INSERT INTO ThongBao(noi_dung) VALUES ('Đã cập nhật thông tin tựa sách'); 
END$$

-- Thủ tục kiểm tra các trigger đã tạo
DROP PROCEDURE IF EXISTS sp_KiemTraTrigger$$
CREATE PROCEDURE sp_KiemTraTrigger()
BEGIN 
    SELECT TRIGGER_NAME AS ten_trigger, EVENT_MANIPULATION AS su_kien, EVENT_OBJECT_TABLE AS bang, ACTION_TIMING AS thoi_diem
    FROM information_schema.TRIGGERS 
    WHERE TRIGGER_SCHEMA = DATABASE(); 
END$$

DELIMITER ;


-- =========================================================================
-- PHẦN 2: CSDL BTL_DeAn (BÀI 7 & BÀI 8)
-- =========================================================================
USE BTL_DeAn;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS THANNHAN, PHANCONG, DEAN, DIADIEM_PHG, NHANVIEN, PHONGBAN;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE PHONGBAN (
    MAPHG VARCHAR(2) PRIMARY KEY,
    TENPHG VARCHAR(20),
    TRPHG VARCHAR(9),
    NG_NHANCHUC DATE
);

CREATE TABLE NHANVIEN (
    MANV VARCHAR(9) PRIMARY KEY,
    HONV VARCHAR(15),
    TENLOT VARCHAR(30),
    TENNV VARCHAR(30),
    NGSINH DATE,
    DCHI VARCHAR(150),
    PHAI VARCHAR(3),
    LUONG DECIMAL(18,0),
    MA_NQL VARCHAR(9),
    PHG VARCHAR(2),
    FOREIGN KEY (PHG) REFERENCES PHONGBAN(MAPHG)
);

ALTER TABLE PHONGBAN ADD CONSTRAINT FK_PB_TRPHG FOREIGN KEY (TRPHG) REFERENCES NHANVIEN(MANV);
ALTER TABLE NHANVIEN ADD CONSTRAINT FK_NV_NQL FOREIGN KEY (MA_NQL) REFERENCES NHANVIEN(MANV);

CREATE TABLE DIADIEM_PHG (
    MAPHG VARCHAR(2),
    DIADIEM VARCHAR(20),
    PRIMARY KEY (MAPHG, DIADIEM),
    FOREIGN KEY (MAPHG) REFERENCES PHONGBAN(MAPHG)
);

CREATE TABLE DEAN (
    MADA VARCHAR(2) PRIMARY KEY,
    TENDA VARCHAR(50),
    DDIEM_DA VARCHAR(20),
    PHONG VARCHAR(2),
    FOREIGN KEY (PHONG) REFERENCES PHONGBAN(MAPHG)
);

CREATE TABLE PHANCONG (
    MA_NVIEN VARCHAR(9),
    SODA VARCHAR(2),
    THOIGIAN DECIMAL(18,0),
    PRIMARY KEY (MA_NVIEN, SODA),
    FOREIGN KEY (MA_NVIEN) REFERENCES NHANVIEN(MANV),
    FOREIGN KEY (SODA) REFERENCES DEAN(MADA)
);

CREATE TABLE THANNHAN (
    MA_NVIEN VARCHAR(9),
    TENTN VARCHAR(20),
    NGSINH DATE,
    PHAI VARCHAR(3),
    QUANHE VARCHAR(15),
    PRIMARY KEY (MA_NVIEN, TENTN),
    FOREIGN KEY (MA_NVIEN) REFERENCES NHANVIEN(MANV)
);

-- -------------------------------------------------------------------------
-- DỮ LIỆU MẪU CSDL ĐỀ ÁN (Đủ để test mọi trường hợp)
-- -------------------------------------------------------------------------
INSERT INTO PHONGBAN VALUES 
('1', 'Ke toan', NULL, '2020-01-01'),
('5', 'Cong nghe', NULL, '2020-02-01'),
('6', 'Nhan su', NULL, '2020-03-01');

INSERT INTO NHANVIEN VALUES 
('NV001','Nguyen','Van','An','1985-01-10','Ha Noi','Nam',42000,NULL,'1'),
('NV002','Tran','Thi','Binh','1988-04-12','Ha Noi','Nu',38000,'NV001','5'),
('NV003','Le','Minh','Chau','1990-08-20','Da Nang','Nu',28000,'NV002','5'),
('NV004','Pham','Quoc','Dung','1982-02-15','Can Tho','Nam',32000,'NV001','6'),
('NV005','Vo','Thanh','Hai','1992-11-03','Hue','Nam',26000,'NV002','5');

UPDATE PHONGBAN SET TRPHG='NV001' WHERE MAPHG='1';
UPDATE PHONGBAN SET TRPHG='NV002' WHERE MAPHG='5';
UPDATE PHONGBAN SET TRPHG='NV004' WHERE MAPHG='6';

INSERT INTO DEAN VALUES 
('1','He thong thu vien','Ha Noi','5'),
('2','Chuyen doi so','Da Nang','5'),
('3','Tuyen dung','Can Tho','6');

-- Phân công giờ tham gia dự án để test Bài 7.4 (tiền thưởng theo mức giờ):
-- NV001: 40 giờ (mức 30-60 -> thưởng 500$)
-- NV002: 70 + 40 = 110 giờ (mức 100-150 -> thưởng 1200$)
-- NV003: 30 giờ (mức 30-60 -> thưởng 500$)
-- NV004: 160 giờ (mức >=150 -> thưởng 1600$)
-- NV005: 100 giờ (mức 100-150 -> thưởng 1200$)
INSERT INTO PHANCONG VALUES 
('NV001','1',40),
('NV002','1',70),
('NV003','1',30),
('NV004','3',160),
('NV005','2',100),
('NV002','2',40);

INSERT INTO THANNHAN VALUES 
('NV002','Lan','2015-01-01','Nu','Con'),
('NV004','Minh','2010-05-01','Nam','Con');

DELIMITER $$

-- =========================================================================
-- BÀI 7: Các Function CSDL Đề án
-- =========================================================================

-- 7.1. Lương trung bình của một phòng ban tùy ý
DROP FUNCTION IF EXISTS fn_LuongTrungBinhPhong$$
CREATE FUNCTION fn_LuongTrungBinhPhong(p_maPhong VARCHAR(2)) 
RETURNS DECIMAL(18,2) 
DETERMINISTIC READS SQL DATA 
BEGIN 
    DECLARE v_result DECIMAL(18,2); 
    SELECT AVG(LUONG) INTO v_result FROM NHANVIEN WHERE PHG = p_maPhong; 
    RETURN v_result; 
END$$

DROP PROCEDURE IF EXISTS sp_fn_LuongTrungBinhPhong$$
CREATE PROCEDURE sp_fn_LuongTrungBinhPhong(IN p_maPhong VARCHAR(2)) 
BEGIN 
    SELECT p_maPhong AS MaPhong, fn_LuongTrungBinhPhong(p_maPhong) AS LuongTrungBinh; 
END$$

-- 7.2. Tổng lương nhân viên nhận được theo dự án (Lương * Số giờ)
DROP FUNCTION IF EXISTS fn_LuongNhanVienDuAn$$
CREATE FUNCTION fn_LuongNhanVienDuAn(p_maNv VARCHAR(9), p_maDa VARCHAR(2)) 
RETURNS DECIMAL(18,2) 
DETERMINISTIC READS SQL DATA 
BEGIN 
    DECLARE v_result DECIMAL(18,2); 
    SELECT n.LUONG * p.THOIGIAN INTO v_result 
    FROM NHANVIEN n 
    JOIN PHANCONG p ON p.MA_NVIEN = n.MANV 
    WHERE n.MANV = p_maNv AND p.SODA = p_maDa; 
    RETURN v_result; 
END$$

DROP PROCEDURE IF EXISTS sp_fn_LuongNhanVienDuAn$$
CREATE PROCEDURE sp_fn_LuongNhanVienDuAn(IN p_maNv VARCHAR(9), IN p_maDa VARCHAR(2)) 
BEGIN 
    SELECT p_maNv AS MaNhanVien, p_maDa AS MaDuAn, fn_LuongNhanVienDuAn(p_maNv, p_maDa) AS TongLuong; 
END$$

-- 7.3. Tổng tiền lương trung bình của các phòng ban
DROP PROCEDURE IF EXISTS sp_fn_LuongTrungBinhCacPhong$$
CREATE PROCEDURE sp_fn_LuongTrungBinhCacPhong() 
BEGIN 
    SELECT p.MAPHG, p.TENPHG, ROUND(AVG(n.LUONG), 2) AS LuongTrungBinh 
    FROM PHONGBAN p 
    LEFT JOIN NHANVIEN n ON n.PHG = p.MAPHG 
    GROUP BY p.MAPHG, p.TENPHG; 
END$$

-- 7.4. Tổng tiền thưởng cho nhân viên dựa vào tổng số giờ tham gia dự án
DROP FUNCTION IF EXISTS fn_TienThuongNhanVien$$
CREATE FUNCTION fn_TienThuongNhanVien(p_maNv VARCHAR(9)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    DECLARE v_gio DECIMAL(18,0); 
    SELECT COALESCE(SUM(THOIGIAN), 0) INTO v_gio FROM PHANCONG WHERE MA_NVIEN = p_maNv; 
    RETURN CASE 
        WHEN v_gio BETWEEN 30 AND 60 THEN 500 
        WHEN v_gio > 60 AND v_gio < 100 THEN 1000 
        WHEN v_gio >= 100 AND v_gio < 150 THEN 1200 
        WHEN v_gio >= 150 THEN 1600 
        ELSE 0 
    END; 
END$$

DROP PROCEDURE IF EXISTS sp_fn_TienThuongNhanVien$$
CREATE PROCEDURE sp_fn_TienThuongNhanVien(IN p_maNv VARCHAR(9)) 
BEGIN 
    SELECT p_maNv AS MaNhanVien, 
           (SELECT COALESCE(SUM(THOIGIAN), 0) FROM PHANCONG WHERE MA_NVIEN = p_maNv) AS TongGioThamGia,
           fn_TienThuongNhanVien(p_maNv) AS TienThuong; 
END$$

-- 7.5. Tổng số dự án theo mỗi phòng ban
DROP PROCEDURE IF EXISTS sp_fn_SoDuAnTheoPhong$$
CREATE PROCEDURE sp_fn_SoDuAnTheoPhong() 
BEGIN 
    SELECT p.MAPHG, p.TENPHG, COUNT(DISTINCT d.MADA) AS SoDuAn 
    FROM PHONGBAN p 
    LEFT JOIN DEAN d ON d.PHONG = p.MAPHG 
    GROUP BY p.MAPHG, p.TENPHG; 
END$$

-- 7.6. Thông tin nhân viên gồm: MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB
-- (Trong SQL Server viết bằng Inline Table-Valued Function & Multi-statement Function. Trong MySQL dùng SP trả bảng)
DROP PROCEDURE IF EXISTS sp_fn_ThongTinNhanVien$$
CREATE PROCEDURE sp_fn_ThongTinNhanVien() 
BEGIN 
    SELECT n.MANV, CONCAT_WS(' ', n.HONV, n.TENLOT, n.TENNV) AS HoTen, n.NGSINH, 
           COUNT(t.TENTN) AS SoNguoiThan, n.LUONG AS TongLuongTB 
    FROM NHANVIEN n 
    LEFT JOIN THANNHAN t ON t.MA_NVIEN = n.MANV 
    GROUP BY n.MANV, n.HONV, n.TENLOT, n.TENNV, n.NGSINH, n.LUONG; 
END$$

-- =========================================================================
-- BÀI 8: Các hàm/thủ tục thống kê CSDL Đề án
-- =========================================================================

-- 8.1. Dự án có nhiều hơn 2 nhân viên tham gia
DROP PROCEDURE IF EXISTS sp_fn_DuAnNhieuNhanVien$$
CREATE PROCEDURE sp_fn_DuAnNhieuNhanVien() 
BEGIN 
    SELECT d.MADA, d.TENDA, COUNT(p.MA_NVIEN) AS SoNhanVien 
    FROM DEAN d 
    JOIN PHANCONG p ON p.SODA = d.MADA 
    GROUP BY d.MADA, d.TENDA 
    HAVING COUNT(p.MA_NVIEN) > 2; 
END$$

-- 8.2. Phòng có > 2 nhân viên, số lượng nhân viên có lương > 25000
DROP PROCEDURE IF EXISTS sp_fn_PhongNhieuNhanVienLuongCao$$
CREATE PROCEDURE sp_fn_PhongNhieuNhanVienLuongCao() 
BEGIN 
    SELECT p.MAPHG AS MaPhong, p.TENPHG AS TenPhong,
           COUNT(n.MANV) AS TongSoNV,
           SUM(n.LUONG > 25000) AS SoNhanVienLuongCao
    FROM PHONGBAN p
    JOIN NHANVIEN n ON n.PHG = p.MAPHG
    GROUP BY p.MAPHG, p.TENPHG
    HAVING COUNT(n.MANV) > 2 AND SUM(n.LUONG > 25000) > 0; 
END$$

-- 8.3. Phòng có lương trung bình > 30000 (MaPB, TenPB, SoLuongNV)
DROP PROCEDURE IF EXISTS sp_fn_PhongLuongCao$$
CREATE PROCEDURE sp_fn_PhongLuongCao() 
BEGIN 
    SELECT p.MAPHG, p.TENPHG, COUNT(n.MANV) AS SoNhanVien, ROUND(AVG(n.LUONG), 2) AS LuongTrungBinh 
    FROM PHONGBAN p 
    JOIN NHANVIEN n ON n.PHG = p.MAPHG 
    GROUP BY p.MAPHG, p.TENPHG 
    HAVING AVG(n.LUONG) > 30000; 
END$$

-- 8.4. Phòng có lương trung bình > 30000, số lượng nhân viên nam của phòng đó
DROP PROCEDURE IF EXISTS sp_fn_PhongNhieuNhanVienNam$$
CREATE PROCEDURE sp_fn_PhongNhieuNhanVienNam() 
BEGIN 
    SELECT p.MAPHG, p.TENPHG, SUM(n.PHAI = 'Nam') AS SoNhanVienNam, ROUND(AVG(n.LUONG), 2) AS LuongTrungBinh 
    FROM PHONGBAN p 
    JOIN NHANVIEN n ON n.PHG = p.MAPHG 
    GROUP BY p.MAPHG, p.TENPHG 
    HAVING AVG(n.LUONG) > 30000; 
END$$

-- 8.5. Mã số dự án, tên dự án và số lượng nhân viên phòng số 5 tham gia
DROP PROCEDURE IF EXISTS sp_fn_DuAnNhanVienPhong5$$
CREATE PROCEDURE sp_fn_DuAnNhanVienPhong5() 
BEGIN 
    SELECT d.MADA, d.TENDA, COUNT(n.MANV) AS SoNhanVienPhong5 
    FROM DEAN d 
    LEFT JOIN PHANCONG p ON p.SODA = d.MADA 
    LEFT JOIN NHANVIEN n ON n.MANV = p.MA_NVIEN AND n.PHG = '5' 
    GROUP BY d.MADA, d.TENDA; 
END$$

DELIMITER ;
