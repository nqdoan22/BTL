# BÀI TẬP LỚN CUỐI KỲ

> Tài liệu này được chuyển từ file Word đề bài gốc sang Markdown để dễ đọc bằng AI.
> Nội dung giữ nguyên theo đề. Chỉ sửa lỗi định dạng và lỗi gõ dấu (xem mục "Ghi chú chuyển đổi" ở cuối).
> Sơ đồ CSDL Đề án (bài 7, 8) trong đề gốc là hình ảnh, đã được chép lại thành bảng.

## Yêu cầu chung

- Làm theo mẫu báo cáo KLTN cuối kỳ của khoa (có đầy đủ bìa, mục lục, …).
- Học theo nhóm, nhưng mỗi SV làm riêng 1 báo cáo.
- Phải trung thực: nội dung trong báo cáo là những bài tập mình hiểu, giải thích được code và có thể tự code lại được.
- Để đạt yêu cầu phải làm được tối thiểu **7/10** bài tập trong danh sách.
- Tạo 1 giao diện chính dạng **WinForm** có nhiều nút command, mỗi nút thực hiện 1 bài tập dưới đây.
- Nếu bài tập có nhiều câu nhỏ thì tạo tiếp 1 giao diện con để thực hiện các câu nhỏ khi người dùng click vào nút tương ứng.
- Nếu bài tập có CSDL thì tạo CSDL, nhập dữ liệu và tạo 1 nút trên giao diện để thực hiện kết nối với CSDL của bài tập trước khi thực hiện các câu khác.
- Data trong CSDL phải đủ để test mọi trường hợp có thể xảy ra.
- Mỗi bài tập phải có đầy đủ các nội dung:
  1. Câu hỏi
  2. Code kết nối CSDL
  3. Hàm / thủ tục / trigger
  4. Code lời gọi hàm / thủ tục
  5. Kết quả trả về

---

## Bài 1

Viết **stored procedure** giải phương trình bậc 1: `ax + b = 0` với a, b bất kỳ.

---

## Bài 2

Viết 1 **hàm** trả về kết quả giải phương trình bậc 2: `ax² + bx + c = 0` với a, b, c bất kỳ.

---

## Bài 3

Viết **stored procedure** liệt kê những thông tin của đầu sách, thông tin tựa sách và số lượng sách hiện chưa được mượn của một đầu sách cụ thể (ISBN).

Với các bảng:

```
Tuasach  (ma_tuasach, tuasach, tacgia, tomtat)
Dausach  (isbn, ma_tuasach, ngonngu, bia, trangthai)
Cuonsach (isbn, ma_cuonsach, tinhtrang)
```

---

## Bài 4

Viết **hàm** tính tuổi của người có năm sinh được nhập vào như một tham số của hàm.

---

## Bài 5 — CSDL Thư viện

Cho CSDL như sau:

```
DocGia        (ma_DocGia, ho, tenlot, ten, ngaysinh)
Nguoilon      (ma_DocGia, sonha, duong, quan, dienthoai, han_sd)
Treem         (ma_DocGia, ma_DocGia_nguoilon)
Tuasach       (ma_tuasach, tuasach, tacgia, tomtat)
Dausach       (isbn, ma_tuasach, ngonngu, bia, trangthai)
Cuonsach      (isbn, ma_cuonsach, tinhtrang)
DangKy        (isbn, ma_DocGia, ngay_dk, ghichu)
Muon          (isbn, ma_cuonsach, ma_DocGia, ngay_muon, ngay_hethan)
QuaTrinhMuon  (isbn, ma_cuonsach, ngay_muon, ma_DocGia, ngay_hethan, ngay_tra, tien_muon, tien_datra, tien_datcoc, ghichu)
```

Viết các **stored procedure** thực hiện các việc sau:

### 5a. Xem thông tin độc giả

- **Tên:** `sp_ThongtinDocGia`
- **Nội dung:** Liệt kê những thông tin của độc giả tương ứng với mã độc giả.
- **Thực hiện:**
  1. Kiểm tra độc giả này thuộc loại người lớn hay trẻ em.
  2. Nếu là người lớn thì in những thông tin độc giả này, gồm: thông tin độc giả + thông tin người lớn.
  3. Nếu là trẻ em thì in những thông tin liên quan đến độc giả này, gồm: thông tin độc giả + thông tin trẻ em.

### 5b. Thông tin đầu sách

- **Tên:** `sp_ThongtinDausach`
- **Nội dung:** Liệt kê những thông tin của đầu sách, thông tin tựa sách và số lượng sách hiện chưa được mượn của một đầu sách cụ thể (ISBN).

### 5c. Liệt kê những độc giả người lớn đang mượn sách

- **Tên:** `sp_ThongtinNguoilonDangmuon`
- **Nội dung:** Liệt kê những thông tin của tất cả độc giả đang mượn sách của thư viện.

### 5d. Liệt kê những độc giả người lớn đang mượn sách quá hạn

- **Tên:** `sp_ThongtinNguoilonQuahan`
- **Nội dung:** Liệt kê những thông tin của tất cả độc giả đang mượn sách của thư viện đang trong tình trạng mượn quá hạn 14 ngày.

### 5e. Liệt kê những độc giả người lớn đang mượn sách có trẻ em cũng đang mượn sách

- **Tên:** `sp_DocGiaCoTreEmMuon`
- **Nội dung:** Liệt kê những độc giả đang trong tình trạng mượn sách và những trẻ em độc giả này đang bảo lãnh cũng đang trong tình trạng mượn sách.

---

## Bài 6 — Trigger trong CSDL Thư viện

Tạo một số **trigger** như sau trong CSDL Thư viện (bài 5):

### 6.1. `tg_delMuon`

- **Nội dung:** Cập nhật tình trạng của cuốn sách là `yes`.

### 6.2. `tg_insMuon`

- **Nội dung:** Cập nhật tình trạng của cuốn sách là `no`.

### 6.3. `tg_updCuonSach`

- **Nội dung:** Khi thuộc tính tình trạng trên bảng cuốn sách được cập nhật thì trạng thái của đầu sách cũng được cập nhật theo.

### 6.4. `tg_InfThongBao`

- **Nội dung:** Viết trigger khi thêm mới, sửa tên tác giả, thêm/sửa một tựa sách thì in ra câu thông báo bằng tiếng Việt: `'Đã thêm mới tựa sách'`.

**Gợi ý:** Kiểm tra trigger đã tạo bằng khối lệnh sau để dữ liệu không bị thay đổi:

```sql
BEGIN TRAN
    -- khối lệnh thêm, xóa, sửa
ROLLBACK
```

---

## Bài 7 — CSDL Đề án

Cho CSDL Đề án (chép lại từ sơ đồ hình ảnh trong đề gốc; 🔑 = khóa chính):

### NHANVIEN

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MANV 🔑 | varchar(9) | No | |
| HONV | nvarchar(15) | Yes | |
| TENLOT | nvarchar(30) | Yes | |
| TENNV | nvarchar(30) | Yes | |
| NGSINH | smalldatetime | Yes | |
| DCHI | nvarchar(150) | Yes | |
| PHAI | nvarchar(3) | Yes | |
| LUONG | numeric(18, 0) | Yes | |
| MA_NQL | varchar(9) | Yes | FK → NHANVIEN(MANV) (người quản lý) |
| PHG | varchar(2) | Yes | FK → PHONGBAN(MAPHG) |

### PHONGBAN

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MAPHG 🔑 | varchar(2) | No | |
| TENPHG | nvarchar(20) | Yes | |
| TRPHG | varchar(9) | Yes | FK → NHANVIEN(MANV) (trưởng phòng) |
| NG_NHANCHUC | smalldatetime | Yes | |

### DIADIEM_PHG

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MAPHG 🔑 | varchar(2) | No | FK → PHONGBAN(MAPHG) |
| DIADIEM 🔑 | varchar(20) | No | |

### DEAN

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MADA 🔑 | varchar(2) | No | |
| TENDA | nvarchar(50) | Yes | |
| DDIEM_DA | varchar(20) | Yes | |
| PHONG | varchar(2) | Yes | FK → PHONGBAN(MAPHG) |

### PHANCONG

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MA_NVIEN 🔑 | varchar(9) | No | FK → NHANVIEN(MANV) |
| SODA 🔑 | varchar(2) | No | FK → DEAN(MADA) |
| THOIGIAN | numeric(18, 0) | Yes | Số giờ tham gia |

### THANNHAN

| Cột | Kiểu dữ liệu | Null | Ghi chú |
|---|---|---|---|
| MA_NVIEN 🔑 | varchar(9) | No | FK → NHANVIEN(MANV) |
| TENTN 🔑 | varchar(20) | No | |
| NGSINH | smalldatetime | Yes | |
| PHAI | varchar(3) | Yes | |
| QUANHE | varchar(15) | Yes | |

Viết các **function** sau trong CSDL Đề án:

- **7.1.** Viết hàm trả về tổng tiền lương trung bình của một phòng ban tùy ý (truyền vào MaPB).
- **7.2.** Viết hàm trả về tổng lương nhận được của nhân viên theo dự án (truyền vào MaNV và MaDA).
- **7.3.** Viết hàm trả về tổng tiền lương trung bình của các phòng ban.
- **7.4.** Viết hàm trả về tổng tiền thưởng cho nhân viên dựa vào tổng số giờ tham gia dự án (`Time_Total`) như sau:

  | Điều kiện Time_Total | Tổng tiền thưởng |
  |---|---|
  | >= 30 và <= 60 | 500 ($) |
  | > 60 và < 100 | 1000 ($) |
  | >= 100 và < 150 | 1200 ($) |
  | >= 150 | 1600 ($) |

- **7.5.** Viết hàm trả ra tổng số dự án theo mỗi phòng ban.
- **7.6.** Viết hàm trả về kết quả là một bảng (Table), viết bằng hai cách: **Inline Table-Valued Function** và **Multi-statement Table-Valued Function**. Thông tin gồm: `MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB`.

---

## Bài 8 — CSDL Đề án (dùng CSDL của bài 7)

Viết các **hàm** thực hiện các yêu cầu sau:

- **8.1.** Với mỗi dự án có nhiều hơn 2 nhân viên tham gia, trả về danh sách mã dự án, tên dự án và số lượng nhân viên tham gia.
- **8.2.** Với mỗi phòng có nhiều hơn 2 nhân viên, trả về mã phòng và số lượng nhân viên có lương lớn hơn 25000.
- **8.3.** Với mỗi phòng có mức lương trung bình lớn hơn 30000, trả về mã phòng, tên phòng, số lượng nhân viên của phòng đó.
- **8.4.** Với mỗi phòng có mức lương trung bình lớn hơn 30000, trả về mã phòng, tên phòng, số lượng nhân viên nam của phòng đó.
- **8.5.** Với mỗi dự án, trả về mã số dự án, tên dự án và số lượng nhân viên phòng số 5 tham gia.

---

## Bài 9 — CSDL Ga ra sửa xe

Cho một lược đồ CSDL dùng để quản lý hoạt động sửa chữa và bảo trì xe của một ga ra như sau:

### THO (MaTho, TenTho, Nhom, NhomTruong)

**Tân từ:** Mỗi người thợ đều có một mã số (MaTho) để nhận diện, một tên (TenTho) và chỉ thuộc một nhóm. Nhóm trưởng của mỗi nhóm là một trong những người thợ của nhóm đó.

Ràng buộc: `MGT(NhomTruong) ⊆ MGT(MaTho)` *(đề gốc ghi `MGT(MaTho) = MGT(NhomTruong)`)*

### CONGVIEC (MaCV, NoiDungCV)

**Tân từ:** Dịch vụ sửa xe được chia thành nhiều công việc để dễ dàng tính toán chi phí với khách hàng. Mỗi công việc đều có một mã (MaCV) và thuộc tính NoiDungCV mô tả nội dung của công việc.

### HOPDONG (SoHD, NgayHD, MaKH, SoXe, TriGiaHD, NgayGiaoDK, NgayNgThu)

**Tân từ:** Mỗi hợp đồng sửa chữa đều có một mã số phân biệt. NgayHD là ngày ký hợp đồng sửa xe với khách hàng là chủ xe (MaKH). SoXe là số đăng bộ của xe đem đến sửa. Một khách hàng có thể ký nhiều hợp đồng sửa chữa nhiều xe khác nhau hoặc sửa chữa nhiều lần cho cùng một xe, nhưng trong cùng một ngày, những công việc sửa chữa cho một xe chỉ ký hợp đồng một lần. TriGiaHD là tổng trị giá của hóa đơn. NgayGiaoDK là ngày dự kiến phải giao xe cho khách. NgayNgThu là ngày nghiệm thu thật sự sau khi đã sửa chữa xong để thanh lý hợp đồng.

### KHACHHANG (MaKH, TenKH, DiaChi, DienThoai)

**Tân từ:** Mỗi khách hàng có một MaKH để phân biệt, một tên (TenKH), một địa chỉ (DiaChi) và một số điện thoại để theo dõi công nợ.

### CHITIET_HD (SoHD, MaCV, TriGiaCV, MaTho, KhoanTho)

**Tân từ:** Mỗi hợp đồng sửa xe có thể gồm nhiều công việc, MaCV là mã số của từng công việc, TriGiaCV là chi phí của công việc đã tính toán với khách. Mỗi công việc của hợp đồng được giao cho một người thợ (MaTho) phụ trách. Một người thợ có thể được giao một hoặc nhiều công việc của một hay nhiều hợp đồng khác nhau. KhoanTho là số tiền giao khoán cho người thợ về công việc sửa chữa tương ứng.

### PHIEUTHU (SoPT, NgayLapPT, SoHD, MaKH, HoTen, SoTienThu)

**Tân từ:** Khách hàng (MaKH) có thể thanh toán tiền của một hợp đồng (SoHD) làm nhiều lần trước hoặc sau khi nghiệm thu (trong cùng một ngày hoặc khác ngày). Mỗi lần thanh toán đều có số phiếu thu (SoPT) để phân biệt, NgayLapPT là ngày lập phiếu thu. SoTienThu là số tiền thanh toán của lần thu đó. HoTen là họ tên của người mang tiền đến thanh toán (có thể khác với người đứng ra ký hợp đồng).

### Yêu cầu

- Cài đặt các ràng buộc có trong lược đồ CSDL.
- Viết các **hàm** thực hiện các yêu cầu sau:
  - **9.1.** Cho biết danh sách các người thợ hiện không tham gia vào một hợp đồng sửa chữa nào.
  - **9.2.** Cho biết danh sách những hợp đồng đã thanh lý nhưng chưa được thanh toán tiền đầy đủ.
  - **9.3.** Cho biết danh sách những hợp đồng cần phải hoàn tất trước ngày 31/12/2002.
  - **9.4.** Cho biết người thợ nào thực hiện công việc nhiều nhất.
  - **9.5.** Cho biết người thợ nào có tổng trị giá công việc được giao cao nhất.

---

## Bài 10 — CSDL Trường phổ thông

Cho một phần CSDL của một trường phổ thông như sau:

### GV (MAGV, TENGV, MAMH)

**Tân từ:** Một giáo viên (MAGV) chủ nhiệm một bộ môn duy nhất. Đối với những giáo viên không phải là chủ nhiệm bộ môn thì giá trị của thuộc tính MAMH là null.

### MHOC (MAMH, TENMH, SOTIET)

**Tân từ:** Mỗi môn học có một MAMH duy nhất, một TENMH và một số tiết học của môn học đó.

### BUOITHI (HKY, NGAY, GIO, PHG, MAMH, TGTHI)

**Tân từ:** Mỗi buổi thi được xác định bởi một học kỳ (HKY), một ngày, một giờ và một phòng (PHG). Buổi thi liên quan đến một môn duy nhất và có một thời gian thi (TGTHI) duy nhất.

### PC_COI_THI (MAGV, HK, NGAY, GIO, PHG)

**Tân từ:** Một lần phân công coi thi được xác định bởi 1 giáo viên, 1 học kỳ, 1 ngày, 1 giờ và 1 phòng.

### Yêu cầu

**10.1. Cài đặt các ràng buộc sau:**

- **10.1.a.** Một giáo viên có thể được phân công gác thi nhiều buổi trong một học kỳ, với điều kiện các buổi thi đó không liên quan đến môn học do giáo viên đó chủ nhiệm.
- **10.1.b.** Nếu số tiết học là 30 thì thời gian thi là 120 phút.
- **10.1.c.** Nếu số tiết học là 45 tiết trở lên thì thời gian thi là 150 phút.

**10.2. Viết hàm thực hiện các câu hỏi sau:**

- **10.2.a.** Danh sách các giáo viên dạy các môn học có số tiết từ 45 trở lên.
- **10.2.b.** Danh sách giáo viên được phân công gác thi trong học kỳ 1.
- **10.2.c.** Danh sách giáo viên không được phân công gác thi trong học kỳ 1.
- **10.2.d.** Cho biết lịch thi môn văn (`TENMH = 'VĂN HỌC'`).
- **10.2.e.** Cho biết các buổi gác thi của các giáo viên chủ nhiệm môn văn (`TENMH = 'VĂN HỌC'`).

---

## Ghi chú chuyển đổi

Những chỗ đã chỉnh so với file Word gốc (chỉ sửa hình thức, không đổi yêu cầu):

- Bài 2: `ax2 + bx,+ c = 0` → `ax² + bx + c = 0`.
- Bài 5a–5e: đề gốc đánh "Xem thông tin độc giả" bằng dấu gạch đầu dòng, các mục sau là b–e; đã đánh lại thống nhất 5a–5e.
- Bài 6: đề gốc có câu thừa "Cài đặt các thủ tục sau cho CSDL Quản lý thư viện." chen giữa mục 6.3; đã lược bỏ.
- Bài 7: sơ đồ CSDL Đề án trong đề gốc là hình ảnh; đã chép thành bảng. Khóa chính đọc theo biểu tượng chìa khóa trong hình; khóa ngoại suy theo đường nối và tên cột.
- Bài 9: sửa lỗi gõ dấu ("mã so" → "mã số", "nhung trong cung" → "nhưng trong cùng", "ve" → "về", "ho tên" → "họ tên", "MAKH" → "MaKH", "NgaylapPT" → "NgayLapPT", "KhoanTHo" → "KhoanTho").
- Bài 9: ràng buộc nhóm trưởng ghi theo nghĩa đúng (NhomTruong phải là một MaTho), kèm nguyên văn đề gốc.
- Bài 10: "MHỌC" → "MHOC", "BUỔITHI" → "BUOITHI"; "HỌC KỲ (HK)" → "học kỳ (HKY)" cho khớp tên cột. Lưu ý đề gốc dùng `HKY` trong BUOITHI nhưng `HK` trong PC_COI_THI.
- Khóa chính của bài 3, 5, 9, 10 không được đánh dấu trong đề gốc nên không ghi vào tài liệu này.
