namespace BTLWinForms.Exercises;

/// <summary>Danh sách bài tập 1 - 8 theo đề bài (De_bai_BTL_cuoi_ky.md).</summary>
public static class ExerciseCatalog
{
    public const string LibraryDatabase = "BTL_ThuVien";
    public const string ProjectDatabase = "BTL_DeAn";

    private static SqlObject Proc(string name) => new(SqlObjectType.Procedure, name);
    private static SqlObject Func(string name) => new(SqlObjectType.Function, name);
    private static SqlObject Trigger(string name) => new(SqlObjectType.Trigger, name);

    private const string LibrarySchema =
        "DocGia (ma_DocGia, ho, tenlot, ten, ngaysinh)\n" +
        "Nguoilon (ma_DocGia, sonha, duong, quan, dienthoai, han_sd)\n" +
        "Treem (ma_DocGia, ma_DocGia_nguoilon)\n" +
        "Tuasach (ma_tuasach, tuasach, tacgia, tomtat)\n" +
        "Dausach (isbn, ma_tuasach, ngonngu, bia, trangthai)\n" +
        "Cuonsach (isbn, ma_cuonsach, tinhtrang)\n" +
        "DangKy (isbn, ma_DocGia, ngay_dk, ghichu)\n" +
        "Muon (isbn, ma_cuonsach, ma_DocGia, ngay_muon, ngay_hethan)\n" +
        "QuaTrinhMuon (isbn, ma_cuonsach, ngay_muon, ma_DocGia, ngay_hethan, ngay_tra, tien_muon, tien_datra, tien_datcoc, ghichu)";

    private const string ProjectSchema =
        "NHANVIEN (MANV, HONV, TENLOT, TENNV, NGSINH, DCHI, PHAI, LUONG, MA_NQL, PHG)\n" +
        "PHONGBAN (MAPHG, TENPHG, TRPHG, NG_NHANCHUC)\n" +
        "DIADIEM_PHG (MAPHG, DIADIEM)\n" +
        "DEAN (MADA, TENDA, DDIEM_DA, PHONG)\n" +
        "PHANCONG (MA_NVIEN, SODA, THOIGIAN)\n" +
        "THANNHAN (MA_NVIEN, TENTN, NGSINH, PHAI, QUANHE)";

    private const string ReaderHint = "Mẫu: DG01, DG03, DG05 (người lớn) · DG02, DG04, DG06 (trẻ em)";
    private const string IsbnHint = "Mẫu: ISBN001 (còn 2 cuốn) · ISBN002, ISBN003 (đã mượn hết)";
    private const string DepartmentHint = "Mẫu: phòng 1, 4, 5, 6 · phòng 7 chưa có nhân viên";
    private const string EmployeeHint = "Mẫu: NV001 - NV010";
    private const string MySqlTableFunctionNote =
        "MySQL không có hàm trả về bảng, nên kết quả được trả về qua thủ tục gọi các hàm scalar tương ứng.";

    public static IReadOnlyList<Exercise> All { get; } =
    [
        new Exercise
        {
            Number = 1,
            Title = "Giải phương trình bậc 1",
            Database = LibraryDatabase,
            Items =
            [
                new ExerciseItem
                {
                    Code = "1",
                    Title = "Giải phương trình bậc 1",
                    Question = "Viết stored procedure giải phương trình bậc 1: ax + b = 0 với a, b bất kỳ.",
                    Routine = "sp_GiaiPTBac1",
                    Parameters =
                    [
                        new("p_a", "Hệ số a", ParameterKind.Number, "2"),
                        new("p_b", "Hệ số b", ParameterKind.Number, "-4")
                    ],
                    Sources = [Proc("sp_GiaiPTBac1")],
                    Hint = "Thử a = 0 để kiểm tra trường hợp vô nghiệm và vô số nghiệm."
                }
            ]
        },
        new Exercise
        {
            Number = 2,
            Title = "Giải phương trình bậc 2",
            Database = LibraryDatabase,
            Items =
            [
                new ExerciseItem
                {
                    Code = "2",
                    Title = "Giải phương trình bậc 2",
                    Question = "Viết 1 hàm trả về kết quả giải phương trình bậc 2: ax² + bx + c = 0 với a, b, c bất kỳ.",
                    Routine = "sp_GiaiPTBac2",
                    Parameters =
                    [
                        new("p_a", "Hệ số a", ParameterKind.Number, "1"),
                        new("p_b", "Hệ số b", ParameterKind.Number, "-3"),
                        new("p_c", "Hệ số c", ParameterKind.Number, "2")
                    ],
                    Sources = [Func("fn_GiaiPTBac2"), Proc("sp_GiaiPTBac2")],
                    Hint = "Thử Δ < 0 (1, 0, 1), Δ = 0 (1, -2, 1) hoặc a = 0.",
                    Note = "Kết quả được tính bởi hàm fn_GiaiPTBac2; thủ tục sp_GiaiPTBac2 chỉ gọi hàm và trả kết quả về ứng dụng."
                }
            ]
        },
        new Exercise
        {
            Number = 3,
            Title = "Thông tin đầu sách",
            Database = LibraryDatabase,
            Items =
            [
                new ExerciseItem
                {
                    Code = "3",
                    Title = "Thông tin đầu sách",
                    Question = "Viết stored procedure liệt kê những thông tin của đầu sách, thông tin tựa sách và số lượng sách " +
                               "hiện chưa được mượn của một đầu sách cụ thể (ISBN).\n\n" +
                               "Tuasach (ma_tuasach, tuasach, tacgia, tomtat)\n" +
                               "Dausach (isbn, ma_tuasach, ngonngu, bia, trangthai)\n" +
                               "Cuonsach (isbn, ma_cuonsach, tinhtrang)",
                    Routine = "sp_ThongtinDausach",
                    Parameters = [new("p_isbn", "ISBN", ParameterKind.Text, "ISBN001")],
                    Sources = [Proc("sp_ThongtinDausach")],
                    Hint = IsbnHint
                }
            ]
        },
        new Exercise
        {
            Number = 4,
            Title = "Tính tuổi theo năm sinh",
            Database = LibraryDatabase,
            Items =
            [
                new ExerciseItem
                {
                    Code = "4",
                    Title = "Tính tuổi theo năm sinh",
                    Question = "Viết hàm tính tuổi của người có năm sinh được nhập vào như một tham số của hàm.",
                    Routine = "sp_TinhTuoi",
                    Parameters = [new("p_namsinh", "Năm sinh", ParameterKind.Integer, "2000")],
                    Sources = [Func("fn_TinhTuoi"), Proc("sp_TinhTuoi")],
                    Note = "Tuổi được tính bởi hàm fn_TinhTuoi; thủ tục sp_TinhTuoi gọi hàm và trả kết quả về ứng dụng."
                }
            ]
        },
        new Exercise
        {
            Number = 5,
            Title = "Tra cứu CSDL Thư viện",
            Database = LibraryDatabase,
            Schema = LibrarySchema,
            Items =
            [
                new ExerciseItem
                {
                    Code = "5a",
                    Title = "Xem thông tin độc giả",
                    Question = "sp_ThongtinDocGia: liệt kê những thông tin của độc giả tương ứng với mã độc giả.\n" +
                               "1. Kiểm tra độc giả này thuộc loại người lớn hay trẻ em.\n" +
                               "2. Nếu là người lớn: thông tin độc giả + thông tin người lớn.\n" +
                               "3. Nếu là trẻ em: thông tin độc giả + thông tin trẻ em.",
                    Routine = "sp_ThongtinDocGia",
                    Parameters = [new("p_maDocGia", "Mã độc giả", ParameterKind.Text, "DG01")],
                    Sources = [Proc("sp_ThongtinDocGia")],
                    Hint = ReaderHint
                },
                new ExerciseItem
                {
                    Code = "5b",
                    Title = "Thông tin đầu sách",
                    Question = "sp_ThongtinDausach: liệt kê những thông tin của đầu sách, thông tin tựa sách và số lượng sách " +
                               "hiện chưa được mượn của một đầu sách cụ thể (ISBN).",
                    Routine = "sp_ThongtinDausach",
                    Parameters = [new("p_isbn", "ISBN", ParameterKind.Text, "ISBN002")],
                    Sources = [Proc("sp_ThongtinDausach")],
                    Hint = IsbnHint
                },
                new ExerciseItem
                {
                    Code = "5c",
                    Title = "Người lớn đang mượn sách",
                    Question = "sp_ThongtinNguoilonDangmuon: liệt kê những thông tin của tất cả độc giả người lớn đang mượn sách của thư viện.",
                    Routine = "sp_ThongtinNguoilonDangmuon",
                    Sources = [Proc("sp_ThongtinNguoilonDangmuon")]
                },
                new ExerciseItem
                {
                    Code = "5d",
                    Title = "Người lớn mượn quá hạn",
                    Question = "sp_ThongtinNguoilonQuahan: liệt kê những thông tin của tất cả độc giả người lớn đang mượn sách " +
                               "trong tình trạng quá hạn 14 ngày.",
                    Routine = "sp_ThongtinNguoilonQuahan",
                    Sources = [Proc("sp_ThongtinNguoilonQuahan")],
                    Note = "Dữ liệu mẫu: DG01 quá hạn 30 ngày (có trong kết quả), DG03 quá hạn 5 ngày (không có)."
                },
                new ExerciseItem
                {
                    Code = "5e",
                    Title = "Người lớn có trẻ em cùng mượn",
                    Question = "sp_DocGiaCoTreEmMuon: liệt kê những độc giả đang mượn sách và những trẻ em do độc giả này " +
                               "bảo lãnh cũng đang mượn sách.",
                    Routine = "sp_DocGiaCoTreEmMuon",
                    Sources = [Proc("sp_DocGiaCoTreEmMuon")]
                }
            ]
        },
        new Exercise
        {
            Number = 6,
            Title = "Trigger CSDL Thư viện",
            Database = LibraryDatabase,
            Context = "Mỗi câu chạy thao tác kiểm thử trong một transaction, ghi lại trạng thái trước và sau khi trigger chạy, " +
                      "sau đó ROLLBACK để dữ liệu không bị thay đổi (tương đương BEGIN TRAN ... ROLLBACK trong đề).",
            Items =
            [
                new ExerciseItem
                {
                    Code = "6.1",
                    Title = "tg_delMuon",
                    Question = "tg_delMuon: khi xóa một phiếu mượn, cập nhật tình trạng của cuốn sách là 'yes'.",
                    Routine = "sp_KiemThuTrigger",
                    Parameters = [new("p_bai", "Câu", ParameterKind.Text, "6.1", Visible: false)],
                    Sources = [Trigger("tg_delMuon")]
                },
                new ExerciseItem
                {
                    Code = "6.2",
                    Title = "tg_insMuon",
                    Question = "tg_insMuon: khi thêm một phiếu mượn, cập nhật tình trạng của cuốn sách là 'no'.",
                    Routine = "sp_KiemThuTrigger",
                    Parameters = [new("p_bai", "Câu", ParameterKind.Text, "6.2", Visible: false)],
                    Sources = [Trigger("tg_insMuon")]
                },
                new ExerciseItem
                {
                    Code = "6.3",
                    Title = "tg_updCuonSach",
                    Question = "tg_updCuonSach: khi thuộc tính tình trạng trên bảng cuốn sách được cập nhật thì trạng thái " +
                               "của đầu sách cũng được cập nhật theo.",
                    Routine = "sp_KiemThuTrigger",
                    Parameters = [new("p_bai", "Câu", ParameterKind.Text, "6.3", Visible: false)],
                    Sources = [Trigger("tg_updCuonSach")]
                },
                new ExerciseItem
                {
                    Code = "6.4",
                    Title = "tg_InfThongBao",
                    Question = "tg_InfThongBao: khi thêm mới, sửa tên tác giả, thêm/sửa một tựa sách thì in ra câu thông báo " +
                               "bằng tiếng Việt: 'Đã thêm mới tựa sách'.",
                    Routine = "sp_KiemThuTrigger",
                    Parameters = [new("p_bai", "Câu", ParameterKind.Text, "6.4", Visible: false)],
                    Sources = [Trigger("tg_InfThongBao"), Trigger("tg_InfThongBao_Update")],
                    Note = "MySQL không cho một trigger bắt nhiều sự kiện và không có lệnh PRINT, nên dùng 2 trigger " +
                           "(INSERT, UPDATE) và ghi câu thông báo vào bảng ThongBao."
                }
            ]
        },
        new Exercise
        {
            Number = 7,
            Title = "Hàm CSDL Đề án",
            Database = ProjectDatabase,
            Schema = ProjectSchema,
            Items =
            [
                new ExerciseItem
                {
                    Code = "7.1",
                    Title = "Lương trung bình một phòng",
                    Question = "Viết hàm trả về tổng tiền lương trung bình của một phòng ban tùy ý (truyền vào MaPB).",
                    Routine = "sp_fn_LuongTrungBinhPhong",
                    Parameters = [new("p_maPhong", "Mã phòng", ParameterKind.Text, "5")],
                    Sources = [Func("fn_LuongTrungBinhPhong"), Proc("sp_fn_LuongTrungBinhPhong")],
                    Hint = DepartmentHint
                },
                new ExerciseItem
                {
                    Code = "7.2",
                    Title = "Lương nhân viên theo dự án",
                    Question = "Viết hàm trả về tổng lương nhận được của nhân viên theo dự án (truyền vào MaNV và MaDA).",
                    Routine = "sp_fn_LuongNhanVienDuAn",
                    Parameters =
                    [
                        new("p_maNv", "Mã nhân viên", ParameterKind.Text, "NV002"),
                        new("p_maDa", "Mã dự án", ParameterKind.Text, "1")
                    ],
                    Sources = [Func("fn_LuongNhanVienDuAn"), Proc("sp_fn_LuongNhanVienDuAn")],
                    Hint = "Mẫu: NV002 tham gia dự án 1 và 2 · dự án 1 - 5",
                    Note = "Tổng lương theo dự án = LUONG × THOIGIAN (số giờ tham gia dự án)."
                },
                new ExerciseItem
                {
                    Code = "7.3",
                    Title = "Lương trung bình các phòng",
                    Question = "Viết hàm trả về tổng tiền lương trung bình của các phòng ban.",
                    Routine = "sp_fn_LuongTrungBinhCacPhong",
                    Sources = [Func("fn_LuongTrungBinhCacPhong"), Func("fn_LuongTrungBinhPhong"), Proc("sp_fn_LuongTrungBinhCacPhong")],
                    Note = "Hàm fn_LuongTrungBinhCacPhong trả về trung bình của lương trung bình các phòng có nhân viên " +
                           "(dòng cuối); các dòng trên là lương trung bình từng phòng."
                },
                new ExerciseItem
                {
                    Code = "7.4",
                    Title = "Tiền thưởng theo giờ tham gia",
                    Question = "Viết hàm trả về tổng tiền thưởng cho nhân viên dựa vào tổng số giờ tham gia dự án (Time_Total):\n" +
                               ">= 30 và <= 60: 500$ · > 60 và < 100: 1000$ · >= 100 và < 150: 1200$ · >= 150: 1600$",
                    Routine = "sp_fn_TienThuongNhanVien",
                    Parameters = [new("p_maNv", "Mã nhân viên", ParameterKind.Text, "NV007")],
                    Sources = [Func("fn_TienThuongNhanVien"), Proc("sp_fn_TienThuongNhanVien")],
                    Hint = "Mẫu: NV008 (20 giờ) · NV006 (60) · NV007 (80) · NV005 (100) · NV004 (160)"
                },
                new ExerciseItem
                {
                    Code = "7.5",
                    Title = "Số dự án theo phòng",
                    Question = "Viết hàm trả ra tổng số dự án theo mỗi phòng ban.",
                    Routine = "sp_fn_SoDuAnTheoPhong",
                    Sources = [Func("fn_SoDuAnCuaPhong"), Proc("sp_fn_SoDuAnTheoPhong")],
                    Note = MySqlTableFunctionNote
                },
                new ExerciseItem
                {
                    Code = "7.6a",
                    Title = "Bảng nhân viên (Inline)",
                    Question = "Viết hàm trả về kết quả là một bảng gồm MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB. " +
                               "Cách 1: Inline Table-Valued Function.",
                    Routine = "sp_fn_ThongTinNhanVien_Inline",
                    Sources = [Proc("sp_fn_ThongTinNhanVien_Inline"), Func("fn_LuongTrungBinhPhong")],
                    Note = "Mô phỏng Inline TVF: kết quả là một câu SELECT duy nhất. " +
                           "TongLuongTB là lương trung bình của phòng mà nhân viên đang làm việc."
                },
                new ExerciseItem
                {
                    Code = "7.6b",
                    Title = "Bảng nhân viên (Multi-statement)",
                    Question = "Viết hàm trả về kết quả là một bảng gồm MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB. " +
                               "Cách 2: Multi-statement Table-Valued Function.",
                    Routine = "sp_fn_ThongTinNhanVien_Multi",
                    Sources = [Proc("sp_fn_ThongTinNhanVien_Multi"), Func("fn_LuongTrungBinhPhong")],
                    Note = "Mô phỏng Multi-statement TVF: khai báo bảng kết quả, điền dữ liệu qua nhiều câu lệnh rồi trả về."
                }
            ]
        },
        new Exercise
        {
            Number = 8,
            Title = "Thống kê CSDL Đề án",
            Database = ProjectDatabase,
            Schema = ProjectSchema,
            Items =
            [
                new ExerciseItem
                {
                    Code = "8.1",
                    Title = "Dự án có hơn 2 nhân viên",
                    Question = "Với mỗi dự án có nhiều hơn 2 nhân viên tham gia, trả về danh sách mã dự án, tên dự án và " +
                               "số lượng nhân viên tham gia.",
                    Routine = "sp_fn_DuAnNhieuNhanVien",
                    Sources = [Func("fn_SoNhanVienDuAn"), Proc("sp_fn_DuAnNhieuNhanVien")],
                    Note = MySqlTableFunctionNote
                },
                new ExerciseItem
                {
                    Code = "8.2",
                    Title = "Phòng có hơn 2 nhân viên",
                    Question = "Với mỗi phòng có nhiều hơn 2 nhân viên, trả về mã phòng và số lượng nhân viên có lương lớn hơn 25000.",
                    Routine = "sp_fn_PhongNhieuNhanVienLuongCao",
                    Sources = [Func("fn_SoNhanVienPhong"), Func("fn_SoNhanVienLuongTren"), Proc("sp_fn_PhongNhieuNhanVienLuongCao")],
                    Note = MySqlTableFunctionNote
                },
                new ExerciseItem
                {
                    Code = "8.3",
                    Title = "Phòng lương TB > 30000",
                    Question = "Với mỗi phòng có mức lương trung bình lớn hơn 30000, trả về mã phòng, tên phòng, số lượng nhân viên của phòng đó.",
                    Routine = "sp_fn_PhongLuongCao",
                    Sources = [Func("fn_LuongTrungBinhPhong"), Func("fn_SoNhanVienPhong"), Proc("sp_fn_PhongLuongCao")],
                    Note = MySqlTableFunctionNote
                },
                new ExerciseItem
                {
                    Code = "8.4",
                    Title = "Nhân viên nam, phòng lương TB > 30000",
                    Question = "Với mỗi phòng có mức lương trung bình lớn hơn 30000, trả về mã phòng, tên phòng, số lượng nhân viên nam của phòng đó.",
                    Routine = "sp_fn_PhongNhieuNhanVienNam",
                    Sources = [Func("fn_LuongTrungBinhPhong"), Func("fn_SoNhanVienNam"), Proc("sp_fn_PhongNhieuNhanVienNam")],
                    Note = MySqlTableFunctionNote
                },
                new ExerciseItem
                {
                    Code = "8.5",
                    Title = "Nhân viên phòng 5 theo dự án",
                    Question = "Với mỗi dự án, trả về mã số dự án, tên dự án và số lượng nhân viên phòng số 5 tham gia.",
                    Routine = "sp_fn_DuAnNhanVienPhong5",
                    Sources = [Func("fn_SoNhanVienPhongThamGiaDuAn"), Proc("sp_fn_DuAnNhanVienPhong5")],
                    Note = MySqlTableFunctionNote
                }
            ]
        }
    ];
}
