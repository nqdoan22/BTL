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
-- Các độc giả:
-- DG01, DG03, DG05: người lớn | DG02 (DG01 bảo lãnh), DG04 (DG03 bảo lãnh), DG06 (DG05 bảo lãnh): trẻ em
INSERT INTO DocGia VALUES 
('DG01','Nguyen','Van','An','1995-01-10'),
('DG02','Tran','Thi','Binh','2012-04-12'),
('DG03','Le','Minh','Chau','1998-08-20'),
('DG04','Pham','Hoang','Nam','2014-06-15'),
('DG05','Hoang','Thi','Em','1990-03-05'),
('DG06','Vu','Duc','Phong','2016-09-09');

INSERT INTO Nguoilon VALUES 
('DG01','12','Le Loi','Quan 1','0900000001','2028-12-31'),
('DG03','20','Nguyen Hue','Quan 1','0900000003','2028-12-31'),
('DG05','8','Tran Hung Dao','Quan 5','0900000005','2027-06-30');

INSERT INTO Treem VALUES 
('DG02','DG01'),
('DG04','DG03'),
('DG06','DG05');

INSERT INTO Tuasach (tuasach,tacgia,tomtat) VALUES 
('SQL Server can ban','Pham Van A','Giao trinh thuc hanh SQL Server'),
('Lap trinh C#','Tran Van B','WinForms va ADO.NET'),
('Cau truc du lieu','Nguyen Van C','Giai thuat va lap trinh');

-- Ban đầu mọi cuốn sách và đầu sách đều sẵn sàng ('yes').
-- Tình trạng thực tế sẽ do trigger Bài 6 cập nhật khi nhập dữ liệu mượn (cuối Phần 1).
INSERT INTO Dausach VALUES 
('ISBN001',1,'Tieng Viet','Cung','yes'),
('ISBN002',2,'Tieng Viet','Mem','yes'),
('ISBN003',3,'Tieng Anh','Cung','yes');

INSERT INTO Cuonsach VALUES 
('ISBN001',1,'yes'),
('ISBN001',2,'yes'),
('ISBN001',3,'yes'),
('ISBN002',1,'yes'),
('ISBN002',2,'yes'),
('ISBN003',1,'yes');

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

-- 6.4. tg_InfThongBao: thêm mới, sửa tên tác giả, thêm/sửa tựa sách -> in câu thông báo 'Đã thêm mới tựa sách'.
-- MySQL không cho 1 trigger bắt nhiều sự kiện (INSERT OR UPDATE) như SQL Server, nên tách thành 2 trigger
-- dùng chung nội dung. MySQL không có lệnh PRINT trong trigger, nên câu thông báo được ghi vào bảng ThongBao.
DROP TRIGGER IF EXISTS tg_updTuasachThongBao$$
DROP TRIGGER IF EXISTS tg_InfThongBao$$
CREATE TRIGGER tg_InfThongBao AFTER INSERT ON Tuasach FOR EACH ROW 
BEGIN 
    INSERT INTO ThongBao(noi_dung) VALUES ('Đã thêm mới tựa sách'); 
END$$

DROP TRIGGER IF EXISTS tg_InfThongBao_Update$$
CREATE TRIGGER tg_InfThongBao_Update AFTER UPDATE ON Tuasach FOR EACH ROW 
BEGIN 
    -- Chỉ thông báo khi tên tựa sách hoặc tên tác giả thực sự thay đổi
    IF NOT (OLD.tuasach <=> NEW.tuasach) OR NOT (OLD.tacgia <=> NEW.tacgia) THEN
        INSERT INTO ThongBao(noi_dung) VALUES ('Đã thêm mới tựa sách'); 
    END IF;
END$$

-- Thủ tục liệt kê các trigger đã tạo
DROP PROCEDURE IF EXISTS sp_KiemTraTrigger$$
CREATE PROCEDURE sp_KiemTraTrigger()
BEGIN 
    SELECT TRIGGER_NAME AS ten_trigger, EVENT_MANIPULATION AS su_kien, EVENT_OBJECT_TABLE AS bang, ACTION_TIMING AS thoi_diem
    FROM information_schema.TRIGGERS 
    WHERE TRIGGER_SCHEMA = DATABASE(); 
END$$

-- Thủ tục chạy thử từng trigger 6.1 - 6.4 theo gợi ý của đề (BEGIN TRAN ... ROLLBACK):
-- ghi lại trạng thái TRƯỚC và SAU khi thao tác, sau đó ROLLBACK để dữ liệu không bị thay đổi.
-- Bảng kết quả dùng ENGINE=MEMORY (không tham gia transaction) nên vẫn còn sau khi ROLLBACK.
DROP PROCEDURE IF EXISTS sp_KiemThuTrigger$$
CREATE PROCEDURE sp_KiemThuTrigger(IN p_bai VARCHAR(5))
BEGIN
    DECLARE v_ma_tuasach INT;
    DECLARE v_isbn VARCHAR(20);
    DECLARE v_ma_cuonsach INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        DROP TEMPORARY TABLE IF EXISTS tmp_KiemThu;
        RESIGNAL;
    END;

    DROP TEMPORARY TABLE IF EXISTS tmp_KiemThu;
    CREATE TEMPORARY TABLE tmp_KiemThu (
        buoc INT AUTO_INCREMENT PRIMARY KEY,
        thoi_diem VARCHAR(60),
        doi_tuong VARCHAR(120),
        gia_tri VARCHAR(255)
    ) ENGINE = MEMORY;

    START TRANSACTION;

    IF p_bai = '6.1' THEN
        -- Xóa 1 phiếu mượn -> cuốn sách phải chuyển sang 'yes'
        SELECT isbn, ma_cuonsach INTO v_isbn, v_ma_cuonsach FROM Muon ORDER BY isbn, ma_cuonsach LIMIT 1;
        IF v_isbn IS NULL THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không có phiếu mượn nào để kiểm thử tg_delMuon';
        END IF;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Trước', CONCAT('Cuonsach ', isbn, ' #', ma_cuonsach, ' .tinhtrang'), tinhtrang
            FROM Cuonsach WHERE isbn = v_isbn AND ma_cuonsach = v_ma_cuonsach;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            VALUES ('Thao tác', 'DELETE FROM Muon', CONCAT('isbn = ', v_isbn, ', ma_cuonsach = ', v_ma_cuonsach));
        DELETE FROM Muon WHERE isbn = v_isbn AND ma_cuonsach = v_ma_cuonsach;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau (tg_delMuon)', CONCAT('Cuonsach ', isbn, ' #', ma_cuonsach, ' .tinhtrang'), tinhtrang
            FROM Cuonsach WHERE isbn = v_isbn AND ma_cuonsach = v_ma_cuonsach;

    ELSEIF p_bai = '6.2' THEN
        -- Thêm 1 phiếu mượn cho cuốn sách đang rảnh -> cuốn sách phải chuyển sang 'no'
        SELECT isbn, ma_cuonsach INTO v_isbn, v_ma_cuonsach FROM Cuonsach WHERE tinhtrang = 'yes' ORDER BY isbn, ma_cuonsach LIMIT 1;
        IF v_isbn IS NULL THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Không còn cuốn sách rảnh để kiểm thử tg_insMuon';
        END IF;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Trước', CONCAT('Cuonsach ', isbn, ' #', ma_cuonsach, ' .tinhtrang'), tinhtrang
            FROM Cuonsach WHERE isbn = v_isbn AND ma_cuonsach = v_ma_cuonsach;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            VALUES ('Thao tác', 'INSERT INTO Muon', CONCAT('isbn = ', v_isbn, ', ma_cuonsach = ', v_ma_cuonsach, ', ma_DocGia = DG05'));
        INSERT INTO Muon VALUES (v_isbn, v_ma_cuonsach, 'DG05', CURDATE(), DATE_ADD(CURDATE(), INTERVAL 14 DAY));
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau (tg_insMuon)', CONCAT('Cuonsach ', isbn, ' #', ma_cuonsach, ' .tinhtrang'), tinhtrang
            FROM Cuonsach WHERE isbn = v_isbn AND ma_cuonsach = v_ma_cuonsach;

    ELSEIF p_bai = '6.3' THEN
        -- ISBN003 chỉ có 1 cuốn và đang được mượn -> đầu sách 'no'.
        -- Cập nhật cuốn sách thành 'yes' -> đầu sách 'yes'; cập nhật lại 'no' -> đầu sách 'no'.
        SET v_isbn = 'ISBN003';
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Trước', CONCAT('Dausach ', isbn, ' .trangthai'), trangthai FROM Dausach WHERE isbn = v_isbn;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            VALUES ('Thao tác', 'UPDATE Cuonsach', 'SET tinhtrang = ''yes'' WHERE isbn = ''ISBN003''');
        UPDATE Cuonsach SET tinhtrang = 'yes' WHERE isbn = v_isbn;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau (tg_updCuonSach)', CONCAT('Dausach ', isbn, ' .trangthai'), trangthai FROM Dausach WHERE isbn = v_isbn;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            VALUES ('Thao tác', 'UPDATE Cuonsach', 'SET tinhtrang = ''no'' WHERE isbn = ''ISBN003''');
        UPDATE Cuonsach SET tinhtrang = 'no' WHERE isbn = v_isbn;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau (tg_updCuonSach)', CONCAT('Dausach ', isbn, ' .trangthai'), trangthai FROM Dausach WHERE isbn = v_isbn;

    ELSEIF p_bai = '6.4' THEN
        -- Thêm mới tựa sách, sửa tên tác giả, sửa tên tựa sách -> mỗi thao tác sinh 1 thông báo
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Trước', 'Số dòng trong ThongBao', COUNT(*) FROM ThongBao;

        INSERT INTO Tuasach(tuasach, tacgia, tomtat) VALUES ('Co so du lieu nang cao', 'Le Van D', 'Sach kiem thu trigger');
        SET v_ma_tuasach = LAST_INSERT_ID();
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau INSERT Tuasach (tg_InfThongBao)', 'ThongBao mới nhất', noi_dung FROM ThongBao ORDER BY id DESC LIMIT 1;

        UPDATE Tuasach SET tacgia = 'Le Van E' WHERE ma_tuasach = v_ma_tuasach;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau UPDATE tacgia (tg_InfThongBao_Update)', 'ThongBao mới nhất', noi_dung FROM ThongBao ORDER BY id DESC LIMIT 1;

        UPDATE Tuasach SET tuasach = 'Co so du lieu nang cao (tai ban)' WHERE ma_tuasach = v_ma_tuasach;
        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau UPDATE tuasach (tg_InfThongBao_Update)', 'ThongBao mới nhất', noi_dung FROM ThongBao ORDER BY id DESC LIMIT 1;

        INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
            SELECT 'Sau', 'Số dòng trong ThongBao', COUNT(*) FROM ThongBao;
    ELSE
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tham số p_bai phải là 6.1, 6.2, 6.3 hoặc 6.4';
    END IF;

    ROLLBACK;

    INSERT INTO tmp_KiemThu(thoi_diem, doi_tuong, gia_tri)
        VALUES ('ROLLBACK', 'Dữ liệu', 'Đã hoàn tác, CSDL không bị thay đổi');

    SELECT buoc AS Buoc, thoi_diem AS ThoiDiem, doi_tuong AS DoiTuong, gia_tri AS GiaTri FROM tmp_KiemThu ORDER BY buoc;
    DROP TEMPORARY TABLE IF EXISTS tmp_KiemThu;
END$$

DELIMITER ;

-- -------------------------------------------------------------------------
-- DỮ LIỆU MƯỢN SÁCH (nhập SAU khi tạo trigger để tình trạng sách được cập nhật đúng)
-- -------------------------------------------------------------------------
-- DG01 (người lớn) mượn quá hạn 30 ngày            -> có trong 5c, 5d
-- DG03 (người lớn) mượn quá hạn 5 ngày (<= 14)     -> có trong 5c, KHÔNG có trong 5d
-- DG02 (trẻ em của DG01) đang mượn                 -> DG01 có trong 5e
-- DG04 (trẻ em của DG03) không mượn                -> DG03 KHÔNG có trong 5e
-- DG06 (trẻ em của DG05) đang mượn, DG05 không mượn -> DG05 KHÔNG có trong 5c, 5e
-- Sau khi nhập: ISBN001 còn 2/3 cuốn (đầu sách 'yes'); ISBN002 và ISBN003 hết sách (đầu sách 'no').
INSERT INTO Muon VALUES 
('ISBN001',2,'DG01', DATE_SUB(CURDATE(), INTERVAL 45 DAY), DATE_SUB(CURDATE(), INTERVAL 30 DAY)),
('ISBN002',1,'DG02', DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_ADD(CURDATE(), INTERVAL 10 DAY)),
('ISBN003',1,'DG03', DATE_SUB(CURDATE(), INTERVAL 19 DAY), DATE_SUB(CURDATE(), INTERVAL 5 DAY)),
('ISBN002',2,'DG06', DATE_SUB(CURDATE(), INTERVAL 3 DAY), DATE_ADD(CURDATE(), INTERVAL 11 DAY));

-- Xóa các thông báo sinh ra trong quá trình cài đặt (nếu có)
DELETE FROM ThongBao;


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
-- Phòng 1: 3 NV, lương TB 32333 (> 30000), 2 NV lương > 25000, 2 NV nam
-- Phòng 4: 3 NV, lương TB 23500 (<= 30000), 0 NV lương > 25000   -> 8.2 vẫn liệt kê (số lượng = 0); 8.3, 8.4 loại
-- Phòng 5: 3 NV, lương TB 30667 (> 30000), 3 NV lương > 25000, 1 NV nam
-- Phòng 6: 1 NV (<= 2 NV)                                         -> 8.2 loại; lương TB 32000 -> 8.3, 8.4 có
-- Phòng 7: chưa có nhân viên, chưa có đề án                       -> 7.1 trả NULL, 7.5 trả 0
INSERT INTO PHONGBAN VALUES 
('1', 'Ke toan', NULL, '2020-01-01'),
('4', 'Kinh doanh', NULL, '2021-04-01'),
('5', 'Cong nghe', NULL, '2020-02-01'),
('6', 'Nhan su', NULL, '2020-03-01'),
('7', 'Nghien cuu', NULL, '2023-07-01');

INSERT INTO NHANVIEN VALUES 
('NV001','Nguyen','Van','An','1985-01-10','Ha Noi','Nam',42000,NULL,'1'),
('NV002','Tran','Thi','Binh','1988-04-12','Ha Noi','Nu',38000,'NV001','5'),
('NV003','Le','Minh','Chau','1990-08-20','Da Nang','Nu',28000,'NV002','5'),
('NV004','Pham','Quoc','Dung','1982-02-15','Can Tho','Nam',32000,'NV001','6'),
('NV005','Vo','Thanh','Hai','1992-11-03','Hue','Nam',26000,'NV002','5'),
('NV006','Dang','Thu','Ha','1991-05-21','Ha Noi','Nu',35000,'NV001','1'),
('NV007','Bui','Van','Khoa','1995-12-02','Hai Phong','Nam',20000,'NV001','1'),
('NV008','Do','Thanh','Long','1987-07-17','Ha Noi','Nam',22000,'NV001','4'),
('NV009','Ngo','Thi','Mai','1993-03-30','Nam Dinh','Nu',24000,'NV008','4'),
('NV010','Ly','Van','Nghia','1996-10-11','Vinh','Nam',24500,'NV008','4');

UPDATE PHONGBAN SET TRPHG='NV001' WHERE MAPHG='1';
UPDATE PHONGBAN SET TRPHG='NV008' WHERE MAPHG='4';
UPDATE PHONGBAN SET TRPHG='NV002' WHERE MAPHG='5';
UPDATE PHONGBAN SET TRPHG='NV004' WHERE MAPHG='6';

INSERT INTO DIADIEM_PHG VALUES 
('1','Ha Noi'),
('4','Ha Noi'),
('4','Hai Phong'),
('5','Da Nang'),
('6','Can Tho');

-- DA 1: 4 NV (> 2) | DA 2: 2 NV (<= 2) | DA 3: 1 NV | DA 4: 3 NV (> 2) | DA 5: chưa có NV
INSERT INTO DEAN VALUES 
('1','He thong thu vien','Ha Noi','5'),
('2','Chuyen doi so','Da Nang','5'),
('3','Tuyen dung','Can Tho','6'),
('4','Mo rong thi truong','Hai Phong','4'),
('5','Kiem toan noi bo','Ha Noi','1');

-- Tổng giờ tham gia dự án để test Bài 7.4 (tiền thưởng), đủ mọi mức và các giá trị biên:
-- NV001: 40         -> 500    | NV002: 70 + 40 = 110 -> 1200 | NV003: 30 (biên)  -> 500
-- NV004: 160        -> 1600   | NV005: 100 (biên)    -> 1200 | NV006: 60 (biên)  -> 500
-- NV007: 80         -> 1000   | NV008: 20            -> 0    | NV009: 0          -> 0
-- NV010: 15         -> 0
INSERT INTO PHANCONG VALUES 
('NV001','1',40),
('NV002','1',70),
('NV003','1',30),
('NV010','1',15),
('NV002','2',40),
('NV005','2',100),
('NV004','3',160),
('NV006','4',60),
('NV007','4',80),
('NV008','4',20);

INSERT INTO THANNHAN VALUES 
('NV001','Hoa','1987-02-14','Nu','Vo'),
('NV001','Tuan','2012-06-01','Nam','Con'),
('NV002','Lan','2015-01-01','Nu','Con'),
('NV004','Minh','2010-05-01','Nam','Con');

DELIMITER $$

-- =========================================================================
-- BÀI 7: Các Function CSDL Đề án
-- Lưu ý: MySQL chỉ hỗ trợ hàm trả về 1 giá trị (scalar), không có hàm trả về bảng như SQL Server.
-- Vì vậy các yêu cầu trả về nhiều dòng được viết thành hàm scalar + thủ tục gọi hàm đó để trả bảng.
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
-- Hàm trả về trung bình của lương trung bình các phòng (chỉ tính phòng có nhân viên).
DROP FUNCTION IF EXISTS fn_LuongTrungBinhCacPhong$$
CREATE FUNCTION fn_LuongTrungBinhCacPhong() 
RETURNS DECIMAL(18,2) 
DETERMINISTIC READS SQL DATA 
BEGIN 
    DECLARE v_result DECIMAL(18,2); 
    SELECT AVG(LuongTB) INTO v_result 
    FROM (SELECT AVG(LUONG) AS LuongTB FROM NHANVIEN WHERE PHG IS NOT NULL GROUP BY PHG) t; 
    RETURN v_result; 
END$$

-- Thủ tục hiển thị: lương TB từng phòng (gọi hàm 7.1) + dòng tổng hợp (gọi hàm 7.3)
DROP PROCEDURE IF EXISTS sp_fn_LuongTrungBinhCacPhong$$
CREATE PROCEDURE sp_fn_LuongTrungBinhCacPhong() 
BEGIN 
    SELECT MaPhong, TenPhong, LuongTrungBinh FROM (
        SELECT 0 AS thu_tu, MAPHG AS MaPhong, TENPHG AS TenPhong, fn_LuongTrungBinhPhong(MAPHG) AS LuongTrungBinh 
        FROM PHONGBAN 
        UNION ALL 
        SELECT 1, '*', 'Trung bình các phòng', fn_LuongTrungBinhCacPhong()
    ) kq 
    ORDER BY thu_tu, MaPhong; 
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
DROP FUNCTION IF EXISTS fn_SoDuAnCuaPhong$$
CREATE FUNCTION fn_SoDuAnCuaPhong(p_maPhong VARCHAR(2)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM DEAN WHERE PHONG = p_maPhong); 
END$$

DROP PROCEDURE IF EXISTS sp_fn_SoDuAnTheoPhong$$
CREATE PROCEDURE sp_fn_SoDuAnTheoPhong() 
BEGIN 
    SELECT MAPHG AS MaPhong, TENPHG AS TenPhong, fn_SoDuAnCuaPhong(MAPHG) AS SoDuAn 
    FROM PHONGBAN 
    ORDER BY MAPHG; 
END$$

-- 7.6. Hàm trả về bảng: MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB
--   NguoiThan   : danh sách người thân của nhân viên (tên + quan hệ), 'Không có' nếu không có người thân
--   TongLuongTB : lương trung bình của phòng ban mà nhân viên đang làm việc (dùng hàm 7.1)
-- SQL Server yêu cầu viết 2 cách: Inline Table-Valued Function và Multi-statement Table-Valued Function.
-- MySQL không có hàm trả về bảng, nên mô phỏng 2 cách bằng 2 thủ tục:

-- Cách 1 (mô phỏng Inline TVF): toàn bộ kết quả là MỘT câu lệnh SELECT duy nhất.
DROP PROCEDURE IF EXISTS sp_fn_ThongTinNhanVien$$
DROP PROCEDURE IF EXISTS sp_fn_ThongTinNhanVien_Inline$$
CREATE PROCEDURE sp_fn_ThongTinNhanVien_Inline() 
BEGIN 
    SELECT n.MANV AS MaNV, 
           CONCAT_WS(' ', n.HONV, n.TENLOT, n.TENNV) AS HoTen, 
           n.NGSINH AS NgaySinh, 
           COALESCE((SELECT GROUP_CONCAT(CONCAT(t.TENTN, ' (', t.QUANHE, ')') ORDER BY t.TENTN SEPARATOR ', ') 
                     FROM THANNHAN t WHERE t.MA_NVIEN = n.MANV), 'Không có') AS NguoiThan, 
           fn_LuongTrungBinhPhong(n.PHG) AS TongLuongTB 
    FROM NHANVIEN n 
    ORDER BY n.MANV; 
END$$

-- Cách 2 (mô phỏng Multi-statement TVF): khai báo bảng kết quả, điền dữ liệu qua nhiều câu lệnh rồi trả về.
DROP PROCEDURE IF EXISTS sp_fn_ThongTinNhanVien_Multi$$
CREATE PROCEDURE sp_fn_ThongTinNhanVien_Multi() 
BEGIN 
    -- Bước 1: khai báo bảng kết quả (tương đương RETURNS @KetQua TABLE (...))
    DROP TEMPORARY TABLE IF EXISTS tmp_ThongTinNhanVien;
    CREATE TEMPORARY TABLE tmp_ThongTinNhanVien (
        MaNV VARCHAR(9) PRIMARY KEY,
        HoTen VARCHAR(80),
        NgaySinh DATE,
        NguoiThan VARCHAR(255),
        PHG VARCHAR(2),
        TongLuongTB DECIMAL(18,2)
    );

    -- Bước 2: thêm thông tin cơ bản của nhân viên
    INSERT INTO tmp_ThongTinNhanVien(MaNV, HoTen, NgaySinh, PHG)
    SELECT MANV, CONCAT_WS(' ', HONV, TENLOT, TENNV), NGSINH, PHG FROM NHANVIEN;

    -- Bước 3: cập nhật danh sách người thân
    UPDATE tmp_ThongTinNhanVien k
    JOIN (SELECT MA_NVIEN, GROUP_CONCAT(CONCAT(TENTN, ' (', QUANHE, ')') ORDER BY TENTN SEPARATOR ', ') AS DanhSach 
          FROM THANNHAN GROUP BY MA_NVIEN) t ON t.MA_NVIEN = k.MaNV
    SET k.NguoiThan = t.DanhSach;
    UPDATE tmp_ThongTinNhanVien SET NguoiThan = 'Không có' WHERE NguoiThan IS NULL;

    -- Bước 4: cập nhật lương trung bình của phòng ban
    UPDATE tmp_ThongTinNhanVien SET TongLuongTB = fn_LuongTrungBinhPhong(PHG);

    -- Bước 5: trả về bảng kết quả (tương đương RETURN)
    SELECT MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB FROM tmp_ThongTinNhanVien ORDER BY MaNV;
    DROP TEMPORARY TABLE IF EXISTS tmp_ThongTinNhanVien;
END$$

-- =========================================================================
-- BÀI 8: Các hàm thống kê CSDL Đề án (hàm scalar + thủ tục trả bảng)
-- =========================================================================

-- Các hàm dùng chung
DROP FUNCTION IF EXISTS fn_SoNhanVienDuAn$$
CREATE FUNCTION fn_SoNhanVienDuAn(p_maDa VARCHAR(2)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM PHANCONG WHERE SODA = p_maDa); 
END$$

DROP FUNCTION IF EXISTS fn_SoNhanVienPhong$$
CREATE FUNCTION fn_SoNhanVienPhong(p_maPhong VARCHAR(2)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM NHANVIEN WHERE PHG = p_maPhong); 
END$$

DROP FUNCTION IF EXISTS fn_SoNhanVienLuongTren$$
CREATE FUNCTION fn_SoNhanVienLuongTren(p_maPhong VARCHAR(2), p_mucLuong DECIMAL(18,0)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM NHANVIEN WHERE PHG = p_maPhong AND LUONG > p_mucLuong); 
END$$

DROP FUNCTION IF EXISTS fn_SoNhanVienNam$$
CREATE FUNCTION fn_SoNhanVienNam(p_maPhong VARCHAR(2)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM NHANVIEN WHERE PHG = p_maPhong AND PHAI = 'Nam'); 
END$$

DROP FUNCTION IF EXISTS fn_SoNhanVienPhongThamGiaDuAn$$
CREATE FUNCTION fn_SoNhanVienPhongThamGiaDuAn(p_maDa VARCHAR(2), p_maPhong VARCHAR(2)) 
RETURNS INT 
DETERMINISTIC READS SQL DATA 
BEGIN 
    RETURN (SELECT COUNT(*) FROM PHANCONG pc JOIN NHANVIEN n ON n.MANV = pc.MA_NVIEN 
            WHERE pc.SODA = p_maDa AND n.PHG = p_maPhong); 
END$$

-- 8.1. Dự án có nhiều hơn 2 nhân viên tham gia: mã DA, tên DA, số lượng NV
DROP PROCEDURE IF EXISTS sp_fn_DuAnNhieuNhanVien$$
CREATE PROCEDURE sp_fn_DuAnNhieuNhanVien() 
BEGIN 
    SELECT MADA AS MaDuAn, TENDA AS TenDuAn, fn_SoNhanVienDuAn(MADA) AS SoNhanVien 
    FROM DEAN 
    WHERE fn_SoNhanVienDuAn(MADA) > 2 
    ORDER BY MADA; 
END$$

-- 8.2. Phòng có nhiều hơn 2 nhân viên: mã phòng, số lượng NV có lương > 25000
DROP PROCEDURE IF EXISTS sp_fn_PhongNhieuNhanVienLuongCao$$
CREATE PROCEDURE sp_fn_PhongNhieuNhanVienLuongCao() 
BEGIN 
    SELECT MAPHG AS MaPhong, TENPHG AS TenPhong, 
           fn_SoNhanVienPhong(MAPHG) AS TongSoNV, 
           fn_SoNhanVienLuongTren(MAPHG, 25000) AS SoNhanVienLuongTren25000 
    FROM PHONGBAN 
    WHERE fn_SoNhanVienPhong(MAPHG) > 2 
    ORDER BY MAPHG; 
END$$

-- 8.3. Phòng có lương trung bình > 30000: mã phòng, tên phòng, số lượng NV
DROP PROCEDURE IF EXISTS sp_fn_PhongLuongCao$$
CREATE PROCEDURE sp_fn_PhongLuongCao() 
BEGIN 
    SELECT MAPHG AS MaPhong, TENPHG AS TenPhong, 
           fn_SoNhanVienPhong(MAPHG) AS SoNhanVien, 
           fn_LuongTrungBinhPhong(MAPHG) AS LuongTrungBinh 
    FROM PHONGBAN 
    WHERE fn_LuongTrungBinhPhong(MAPHG) > 30000 
    ORDER BY MAPHG; 
END$$

-- 8.4. Phòng có lương trung bình > 30000: mã phòng, tên phòng, số lượng NV nam
DROP PROCEDURE IF EXISTS sp_fn_PhongNhieuNhanVienNam$$
CREATE PROCEDURE sp_fn_PhongNhieuNhanVienNam() 
BEGIN 
    SELECT MAPHG AS MaPhong, TENPHG AS TenPhong, 
           fn_SoNhanVienNam(MAPHG) AS SoNhanVienNam, 
           fn_LuongTrungBinhPhong(MAPHG) AS LuongTrungBinh 
    FROM PHONGBAN 
    WHERE fn_LuongTrungBinhPhong(MAPHG) > 30000 
    ORDER BY MAPHG; 
END$$

-- 8.5. Với mỗi dự án: mã DA, tên DA, số lượng NV phòng số 5 tham gia
DROP PROCEDURE IF EXISTS sp_fn_DuAnNhanVienPhong5$$
CREATE PROCEDURE sp_fn_DuAnNhanVienPhong5() 
BEGIN 
    SELECT MADA AS MaDuAn, TENDA AS TenDuAn, fn_SoNhanVienPhongThamGiaDuAn(MADA, '5') AS SoNhanVienPhong5 
    FROM DEAN 
    ORDER BY MADA; 
END$$

DELIMITER ;
