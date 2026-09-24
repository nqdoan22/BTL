# BTLWinForms - Bài tập lớn Hệ Cơ sở dữ liệu & Thuật toán (Bài 1 đến Bài 8)

Ứng dụng Windows Forms (.NET 8) thực hiện đầy đủ các yêu cầu từ **Bài 1 đến Bài 8** theo đúng tài liệu đề bài `De_bai_BTL_cuoi_ky.md` (đáp ứng vượt mức yêu cầu tối thiểu 7/10 bài).

---

## 1. Cấu trúc dự án

| Đường dẫn | Nội dung |
|---|---|
| `Program.cs` | Điểm khởi động, bắt lỗi không mong muốn và mở màn hình chính |
| `UI/MainForm.cs` | Giao diện chính: mỗi nút mở một bài tập (Bài 1 - 8) |
| `UI/ExerciseForm.cs` | Giao diện con của một bài: nút **Kết nối CSDL**, danh sách câu nhỏ và kết quả |
| `UI/ConnectionDialog.cs` | Cài đặt máy chủ, cổng, tài khoản, mật khẩu MySQL |
| `UI/Controls.cs`, `UI/Theme.cs` | Các control và bảng màu, font dùng chung |
| `Exercises/ExerciseCatalog.cs` | Nội dung đề, tham số, thủ tục và mã nguồn SQL tương ứng của từng câu |
| `Data/DatabaseService.cs` | Kết nối MySQL (bất đồng bộ), gọi thủ tục, đọc mã nguồn hàm / thủ tục / trigger |
| `Data/ConnectionSettings.cs` | Lưu cài đặt kết nối tại `%AppData%\BTLWinForms\settings.json` (mật khẩu mã hóa bằng Windows DPAPI) |
| `Database/BTL_Install.sql` | Tạo 2 CSDL `BTL_ThuVien`, `BTL_DeAn`, dữ liệu mẫu và toàn bộ Function, Stored Procedure, Trigger |
| `De_bai_BTL_cuoi_ky.md` | Đề bài gốc dạng Markdown |

### Giao diện theo yêu cầu đề bài

- **Giao diện chính** có 8 nút, mỗi nút thực hiện 1 bài tập.
- Mỗi bài mở một **giao diện con**; bài có nhiều câu nhỏ (5, 6, 7, 8) có danh sách câu ở cột trái.
- Mỗi giao diện con có nút **Kết nối CSDL** của bài; nút **Thực hiện** chỉ bật sau khi kết nối thành công.
- Mỗi câu hiển thị đủ 5 nội dung đề yêu cầu:
  1. **Câu hỏi** (phía trên)
  2. **Code kết nối CSDL** và 4. **Code lời gọi hàm / thủ tục** (tab *Code kết nối & lời gọi*, cập nhật theo tham số đang nhập)
  3. **Hàm / thủ tục / trigger** (tab *Hàm / Thủ tục / Trigger*, đọc trực tiếp từ CSDL bằng `SHOW CREATE ...`)
  5. **Kết quả trả về** (tab *Kết quả*)

---

## 2. Bảng Đối Chiếu Routine SQL & Chức Năng Theo Đề Bài

| Bài tập | Yêu cầu đề bài | Tên Procedure / Function trong SQL | Ghi chú triển khai |
|---|---|---|---|
| **Bài 1** | Viết Stored Procedure giải PT bậc 1 ($ax + b = 0$) | `sp_GiaiPTBac1(a, b)` | Có tùy chọn chạy qua SP MySQL hoặc chạy thuật toán C# nội bộ |
| **Bài 2** | Viết Hàm giải PT bậc 2 ($ax^2 + bx + c = 0$) | `fn_GiaiPTBac2(a, b, c)` & `sp_GiaiPTBac2(a, b, c)` | Có tùy chọn chạy qua Function/SP MySQL hoặc chạy thuật toán C# |
| **Bài 3** | Liệt kê thông tin đầu sách & số lượng chưa mượn | `sp_ThongtinDausach(isbn)` | Trả về thông tin tựa sách, ngôn ngữ, bìa, số lượng còn lại |
| **Bài 4** | Viết Hàm tính tuổi theo năm sinh | `fn_TinhTuoi(namsinh)` & `sp_TinhTuoi(namsinh)` | Tính tuổi theo năm hiện tại qua SQL hoặc C# |
| **Bài 5a** | Xem thông tin độc giả (người lớn / trẻ em) | `sp_ThongtinDocGia(maDocGia)` | Kiểm tra loại độc giả theo 3 bước, hiển thị thông tin kèm người bảo lãnh nếu là trẻ em |
| **Bài 5b** | Thông tin đầu sách (tương tự bài 3) | `sp_ThongtinDausach(isbn)` | Dùng chung thủ tục đầu sách |
| **Bài 5c** | Liệt kê độc giả người lớn đang mượn sách | `sp_ThongtinNguoilonDangmuon()` | Trả về danh sách độc giả người lớn và sách đang mượn |
| **Bài 5d** | Liệt kê độc giả mượn quá hạn > 14 ngày | `sp_ThongtinNguoilonQuahan()` | Tính số ngày quá hạn theo `CURDATE()` và ngày hết hạn |
| **Bài 5e** | Độc giả mượn sách có trẻ em bảo lãnh cũng đang mượn | `sp_DocGiaCoTreEmMuon()` | Nối bảng độc giả, người lớn, trẻ em và phiếu mượn |
| **Bài 6.1** | Trigger khi xóa mượn sách (`tg_delMuon`) | `tg_delMuon` | Cập nhật tình trạng cuốn sách thành `yes` |
| **Bài 6.2** | Trigger khi thêm mượn sách (`tg_insMuon`) | `tg_insMuon` | Cập nhật tình trạng cuốn sách thành `no` |
| **Bài 6.3** | Trigger cập nhật cuốn sách (`tg_updCuonSach`) | `tg_updCuonSach` | Cập nhật trạng thái đầu sách thành `yes`/`no` tương ứng |
| **Bài 6.4** | Trigger thông báo thêm/sửa tựa sách (`tg_InfThongBao`) | `tg_InfThongBao` & `tg_InfThongBao_Update` | Ghi câu `'Đã thêm mới tựa sách'` vào bảng `ThongBao` (MySQL không có `PRINT` và không cho 1 trigger bắt nhiều sự kiện) |
| **Bài 6 (kiểm thử)** | Kiểm tra trigger bằng `BEGIN TRAN ... ROLLBACK` | `sp_KiemThuTrigger(p_bai)` | Chạy thao tác trong transaction, hiển thị trạng thái trước/sau rồi `ROLLBACK` nên dữ liệu không đổi |
| **Bài 7.1** | Hàm tính lương trung bình phòng ban | `fn_LuongTrungBinhPhong(maPhong)` | Thủ tục bọc: `sp_fn_LuongTrungBinhPhong` |
| **Bài 7.2** | Hàm tính tổng lương NV theo dự án | `fn_LuongNhanVienDuAn(maNv, maDa)` | Thủ tục bọc: `sp_fn_LuongNhanVienDuAn` |
| **Bài 7.3** | Hàm tính lương trung bình của các phòng ban | `fn_LuongTrungBinhCacPhong()` | Thủ tục bọc `sp_fn_LuongTrungBinhCacPhong` hiển thị lương TB từng phòng + dòng tổng hợp |
| **Bài 7.4** | Hàm tính tiền thưởng theo tổng số giờ tham gia | `fn_TienThuongNhanVien(maNv)` | Thủ tục bọc: `sp_fn_TienThuongNhanVien` |
| **Bài 7.5** | Hàm trả ra tổng số dự án theo mỗi phòng | `fn_SoDuAnCuaPhong(maPhong)` | Thủ tục bọc `sp_fn_SoDuAnTheoPhong` trả về bảng mã phòng, tên phòng, số dự án |
| **Bài 7.6** | Hàm trả về bảng thông tin nhân viên (2 cách) | `sp_fn_ThongTinNhanVien_Inline()` & `sp_fn_ThongTinNhanVien_Multi()` | Mô phỏng Inline TVF (1 câu SELECT) và Multi-statement TVF (bảng tạm + nhiều bước). Cột: `MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB` (lương TB của phòng) |
| **Bài 8.1** | Dự án có nhiều hơn 2 nhân viên tham gia | `sp_fn_DuAnNhieuNhanVien()` | Dùng hàm `fn_SoNhanVienDuAn(maDa) > 2` |
| **Bài 8.2** | Phòng có > 2 nhân viên: số NV có lương > 25000 | `sp_fn_PhongNhieuNhanVienLuongCao()` | Lọc `fn_SoNhanVienPhong(maPhong) > 2`, đếm bằng `fn_SoNhanVienLuongTren(maPhong, 25000)` |
| **Bài 8.3** | Phòng có mức lương trung bình > 30000 | `sp_fn_PhongLuongCao()` | Lọc bằng `fn_LuongTrungBinhPhong > 30000`, trả về mã phòng, tên phòng, số NV |
| **Bài 8.4** | Phòng lương TB > 30000: số lượng nhân viên nam | `sp_fn_PhongNhieuNhanVienNam()` | Dùng `fn_SoNhanVienNam(maPhong)` |
| **Bài 8.5** | Số lượng nhân viên phòng 5 tham gia từng dự án | `sp_fn_DuAnNhanVienPhong5()` | Dùng `fn_SoNhanVienPhongThamGiaDuAn(maDa, '5')` |

> **Lưu ý về MySQL:** MySQL chỉ hỗ trợ hàm trả về 1 giá trị (scalar), không có hàm trả về bảng như SQL Server. Vì vậy các yêu cầu trả về nhiều dòng (7.5, 7.6, 8.1 - 8.5) được viết thành hàm scalar + thủ tục `sp_fn_...` gọi hàm đó để trả về bảng.

---

## 3. Hướng dẫn cài đặt và chạy ứng dụng

### Bước 1: Cài đặt CSDL MySQL
1. Khởi động MySQL Server 8.0+ (qua XAMPP, MySQL Installer hoặc Docker).
2. Mở MySQL Workbench hoặc HeidiSQL.
3. Mở và chạy toàn bộ nội dung file **`Database/BTL_Install.sql`** để tự động tạo 2 cơ sở dữ liệu `BTL_ThuVien` và `BTL_DeAn` cùng đầy đủ dữ liệu mẫu và các routine.

### Bước 2: Chạy ứng dụng WinForms
1. Mở `BTLWinForms.csproj` bằng Visual Studio 2022 hoặc chạy lệnh:
   ```powershell
   dotnet run
   ```
2. Bấm **Cài đặt kết nối** ở góc phải màn hình chính, nhập máy chủ, cổng, tài khoản, mật khẩu MySQL, bấm **Kiểm tra kết nối** rồi **Lưu**.
3. Bấm vào một bài tập. Trong giao diện con, bấm **Kết nối CSDL**, chọn câu (nếu có) rồi bấm **Thực hiện** (hoặc nhấn Enter). Nhấn Esc để đóng.

> Các câu Bài 6 chạy thao tác kiểm thử trong transaction rồi `ROLLBACK`, nên có thể chạy nhiều lần mà dữ liệu không đổi.
