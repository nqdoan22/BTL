using System.Data;

namespace BTLWinForms;

public class Form1 : Form
{
    // ==========================================
    // BẢNG MÀU HIỆN ĐẠI (MODERN COLOR PALETTE)
    // ==========================================
    private static readonly Color ColorPrimaryDark   = Color.FromArgb(18, 30, 49);     // Deep Navy
    private static readonly Color ColorAccent        = Color.FromArgb(0, 140, 148);    // Teal
    private static readonly Color ColorBlue          = Color.FromArgb(41, 105, 176);   // Royal Slate Blue
    private static readonly Color ColorAmber         = Color.FromArgb(217, 119, 6);    // Warm Amber
    private static readonly Color ColorPurple        = Color.FromArgb(109, 76, 153);   // Soft Purple
    private static readonly Color ColorGreen         = Color.FromArgb(16, 149, 93);    // Emerald
    private static readonly Color ColorRed           = Color.FromArgb(220, 38, 38);    // Crimson
    private static readonly Color ColorCanvas        = Color.FromArgb(246, 248, 252);  // Soft Gray Canvas
    private static readonly Color ColorBorder        = Color.FromArgb(226, 232, 240);  // Subtle Border
    private static readonly Color ColorCardBg        = Color.White;
    private static readonly Color ColorTextPrimary   = Color.FromArgb(15, 23, 42);     // Slate 900
    private static readonly Color ColorTextMuted     = Color.FromArgb(100, 116, 139);  // Slate 500

    // ==========================================
    // CÁC CONTROL DÙNG CHUNG & HEADER
    // ==========================================
    private readonly TextBox connectionTextBox = new();
    private readonly Label statusBadgeLabel = new();
    private readonly Panel contentContainer = new();
    private readonly Panel logDrawerPanel = new();
    private readonly TextBox logTextBox = new();
    private readonly Label statusInfoLabel = new();
    private readonly Label statusDbLabel = new();
    private readonly Label statusRowCountLabel = new();
    private readonly Button toggleLogButton = new();

    // Tab buttons
    private readonly Dictionary<string, Button> tabButtons = new();
    private readonly Dictionary<string, Control> tabPages = new();
    private string currentTabKey = "math";

    // ==========================================
    // CONTROLS TAB 1: TOÁN HỌC & THUẬT TOÁN (BÀI 1, 2, 4)
    // ==========================================
    private readonly CheckBox chkRunOnMysql = new();
    private readonly TextBox txtA = new();
    private readonly TextBox txtB = new();
    private readonly TextBox txtC = new();
    private readonly TextBox txtYear = new();
    private readonly Label lblMathFormula = new();
    private readonly Label lblMathSteps = new();
    private readonly Label lblMathResult = new();
    private readonly Label lblMathTime = new();

    // ==========================================
    // CONTROLS TAB 2: QUẢN LÝ THƯ VIỆN (BÀI 3, 5, 6)
    // ==========================================
    private readonly TextBox txtLibraryIsbn = new();
    private readonly TextBox txtLibraryReaderId = new();
    private readonly DataGridView gridLibrary = new();
    private readonly Label lblLibraryGridTitle = new();
    private readonly Label lblLibraryRowCount = new();

    // ==========================================
    // CONTROLS TAB 3: QUẢN LÝ ĐỀ ÁN (BÀI 7, 8)
    // ==========================================
    private readonly TextBox txtProjectDeptId = new();
    private readonly TextBox txtProjectEmpId = new();
    private readonly TextBox txtProjectId = new();
    private readonly DataGridView gridProject = new();
    private readonly Label lblProjectGridTitle = new();
    private readonly Label lblProjectRowCount = new();

    public Form1()
    {
        InitializeWindow();
        BuildLayout();
        ShowTab("math");
    }

    private void InitializeWindow()
    {
        Text = "Hệ Cơ Sở Dữ Liệu & Thuật Toán - BTLWinForms (Bài 1 đến Bài 8)";
        Width = 1320;
        Height = 840;
        MinimumSize = new Size(1080, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorCanvas;
        Font = new Font("Segoe UI", 9.5F);
        DoubleBuffered = true;
    }

    private void BuildLayout()
    {
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            ColumnCount = 1,
            BackColor = ColorCanvas
        };

        // Row 0: Top Header (Connection Bar) - Height 72px
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        // Row 1: Tab Navigation Bar - Height 46px
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        // Row 2: Main Content Area - Fill 100%
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        // Row 3: Collapsible Log Drawer - Default Height 0 (hidden)
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        // Row 4: Status Bar - Height 32px
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        Controls.Add(mainLayout);

        // 1. Header
        mainLayout.Controls.Add(CreateHeaderBar(), 0, 0);

        // 2. Tab Navigation
        mainLayout.Controls.Add(CreateTabBar(), 0, 1);

        // 3. Content Panel
        contentContainer.Dock = DockStyle.Fill;
        contentContainer.BackColor = ColorCanvas;
        mainLayout.Controls.Add(contentContainer, 0, 2);

        // 4. Collapsible Log Drawer
        mainLayout.Controls.Add(CreateLogDrawer(), 0, 3);

        // 5. Status Bar
        mainLayout.Controls.Add(CreateStatusBar(mainLayout), 0, 4);

        // Build Pages
        tabPages["math"] = CreateMathPage();
        tabPages["library"] = CreateLibraryPage();
        tabPages["project"] = CreateProjectPage();
    }

    // =========================================================================
    // 1. HEADER BAR: LOGO & THANH KẾT NỐI MYSQL TINH GỌN
    // =========================================================================
    private Control CreateHeaderBar()
    {
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ColorPrimaryDark,
            Padding = new Padding(20, 8, 20, 8)
        };

        // Logo & Title (Sử dụng FlowLayoutPanel TopDown để không bị đè hay cắt chữ khi phóng to màn hình / DPI scale)
        var titleBox = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 420,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true
        };
        var mainTitle = new Label
        {
            Text = "CƠ SỞ DỮ LIỆU & THUẬT TOÁN",
            Font = new Font("Segoe UI Semibold", 12.5F),
            ForeColor = Color.White,
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 3, 0, 2)
        };
        var subTitle = new Label
        {
            Text = "Bài tập lớn • Bài 1 - Bài 8 • .NET 8 & MySQL",
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 0)
        };
        titleBox.Controls.Add(mainTitle);
        titleBox.Controls.Add(subTitle);
        header.Controls.Add(titleBox);

        // Connection Bar (Right aligned)
        var connPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };

        var lblConn = new Label
        {
            Text = "MySQL Host:",
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = new Font("Segoe UI Semibold", 9F),
            AutoSize = true,
            Margin = new Padding(0, 7, 8, 0)
        };
        connPanel.Controls.Add(lblConn);

        connectionTextBox.Text = @"Server=localhost;Port=3306;Database=BTL_ThuVien;User ID=root;Password=root;SslMode=None";
        connectionTextBox.Width = 430;
        connectionTextBox.Height = 28;
        connectionTextBox.Font = new Font("Consolas", 8.8F);
        connectionTextBox.BorderStyle = BorderStyle.FixedSingle;
        connectionTextBox.Margin = new Padding(0, 4, 8, 0);
        connPanel.Controls.Add(connectionTextBox);

        var btnConnect = CreateStyledButton("Kết nối CSDL", ColorAccent, Color.White, 105, 30);
        btnConnect.Click += (_, _) => TestConnection();
        btnConnect.Margin = new Padding(0, 3, 10, 0);
        connPanel.Controls.Add(btnConnect);

        statusBadgeLabel.Text = "● Chưa kết nối";
        statusBadgeLabel.Font = new Font("Segoe UI Semibold", 8.5F);
        statusBadgeLabel.ForeColor = Color.FromArgb(226, 232, 240);
        statusBadgeLabel.BackColor = Color.FromArgb(47, 63, 86);
        statusBadgeLabel.AutoSize = true;
        statusBadgeLabel.Padding = new Padding(8, 5, 8, 5);
        statusBadgeLabel.Margin = new Padding(0, 3, 0, 0);
        connPanel.Controls.Add(statusBadgeLabel);

        header.Controls.Add(connPanel);
        return header;
    }

    // =========================================================================
    // 2. TAB NAVIGATION BAR (TOP TABS)
    // =========================================================================
    private Control CreateTabBar()
    {
        var tabBar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(28, 43, 67),
            Padding = new Padding(16, 0, 16, 0)
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        flow.Controls.Add(CreateTabButton("math", "📐  1. Toán học & Thuật toán (Bài 1, 2, 4)"));
        flow.Controls.Add(CreateTabButton("library", "📚  2. Quản lý Thư viện (Bài 3, 5, 6)"));
        flow.Controls.Add(CreateTabButton("project", "💼  3. Quản lý Đề án (Bài 7, 8)"));

        tabBar.Controls.Add(flow);
        return tabBar;
    }

    private Button CreateTabButton(string key, string text)
    {
        var btn = new Button
        {
            Text = text,
            Tag = key,
            AutoSize = true,
            Height = 46,
            UseMnemonic = false,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.5F),
            ForeColor = Color.FromArgb(203, 213, 225),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 6, 0),
            Padding = new Padding(14, 0, 14, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += (_, _) => ShowTab(key);
        tabButtons[key] = btn;
        return btn;
    }

    private void ShowTab(string key)
    {
        currentTabKey = key;
        foreach (var (k, btn) in tabButtons)
        {
            if (k == key)
            {
                btn.BackColor = ColorCanvas;
                btn.ForeColor = ColorPrimaryDark;
                btn.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(203, 213, 225);
                btn.Font = new Font("Segoe UI Semibold", 9.5F);
            }
        }

        contentContainer.Controls.Clear();
        if (tabPages.TryGetValue(key, out var page))
        {
            page.Dock = DockStyle.Fill;
            contentContainer.Controls.Add(page);
        }

        UpdateStatusBarContext();
    }

    // =========================================================================
    // 3. TAB 1: TOÁN HỌC & THUẬT TOÁN (BÀI 1, 2, 4)
    // =========================================================================
    private Control CreateMathPage()
    {
        var page = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 16), AutoScroll = true };

        var split = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = ColorCanvas
        };
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 490));
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // LEFT: Form nhập liệu giải phương trình & tính tuổi
        var leftPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(0, 0, 16, 0) };

        // Card 1: Giải phương trình bậc 1 & 2
        var cardEquation = CreateCard("GIẢI PHƯƠNG TRÌNH (BÀI 1 & BÀI 2)", "Hỗ trợ phương trình bậc 1: ax + b = 0 và phương trình bậc 2: ax² + bx + c = 0");

        chkRunOnMysql.Text = "⚡ Thực thi qua Stored Procedure & Function MySQL (sp_GiaiPTBac1, sp_GiaiPTBac2, sp_TinhTuoi)";
        chkRunOnMysql.Font = new Font("Segoe UI Semibold", 8.8F);
        chkRunOnMysql.ForeColor = ColorAccent;
        chkRunOnMysql.AutoSize = true;
        chkRunOnMysql.Cursor = Cursors.Hand;
        chkRunOnMysql.Dock = DockStyle.Top;
        chkRunOnMysql.Padding = new Padding(16, 4, 16, 4);

        var eqBody = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(16, 8, 16, 16)
        };
        eqBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        eqBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddInputRow(eqBody, "Hệ số a:", txtA, "Nhập số thực a (vd: 2)", 0);
        AddInputRow(eqBody, "Hệ số b:", txtB, "Nhập số thực b (vd: -4)", 1);
        AddInputRow(eqBody, "Hệ số c:", txtC, "Nhập c (chỉ cần cho bậc 2)", 2);

        var eqBtnFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 10, 0, 0)
        };

        var btnLinear = CreateStyledButton("Giải Bậc 1 (ax + b = 0)", ColorAccent, Color.White, 165, 34);
        btnLinear.Click += (_, _) => SolveLinear();

        var btnQuadratic = CreateStyledButton("Giải Bậc 2 (ax² + bx + c = 0)", ColorBlue, Color.White, 185, 34);
        btnQuadratic.Click += (_, _) => SolveQuadratic();

        var btnClearEq = CreateStyledButton("Xóa trắng", Color.FromArgb(241, 245, 249), ColorTextPrimary, 80, 34);
        btnClearEq.Click += (_, _) => { txtA.Clear(); txtB.Clear(); txtC.Clear(); txtA.Focus(); };

        eqBtnFlow.Controls.Add(btnLinear);
        eqBtnFlow.Controls.Add(btnQuadratic);
        eqBtnFlow.Controls.Add(btnClearEq);
        eqBody.Controls.Add(eqBtnFlow, 1, 3);

        cardEquation.Controls.Add(eqBody);
        cardEquation.Controls.Add(chkRunOnMysql);
        leftPanel.Controls.Add(cardEquation);

        // Card 2: Tính tuổi (Bài 4)
        var cardAge = CreateCard("TÍNH TUỔI THEO NĂM SINH (BÀI 4)", "Tính toán tuổi hiện tại dựa trên năm sinh nhập vào");
        cardAge.Margin = new Padding(0, 16, 0, 0);

        var ageBody = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(16, 12, 16, 16)
        };
        ageBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        ageBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddInputRow(ageBody, "Năm sinh:", txtYear, $"Nhập năm sinh (1900 - {DateTime.Now.Year})", 0);

        var ageBtnFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 0)
        };
        var btnAge = CreateStyledButton("Tính tuổi", ColorAmber, Color.White, 110, 34);
        btnAge.Click += (_, _) => CalculateAge();
        ageBtnFlow.Controls.Add(btnAge);
        ageBody.Controls.Add(ageBtnFlow, 1, 1);

        cardAge.Controls.Add(ageBody);
        leftPanel.Controls.Add(cardAge);

        split.Controls.Add(leftPanel, 0, 0);

        // RIGHT: THẺ KẾT QUẢ CHUYÊN DỤNG (RESULT CARD)
        var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 0, 0, 0) };
        var cardResult = CreateCard("KẾT QUẢ THỰC THI THUẬT TOÁN", "Hiển thị công thức, các bước phân tích và kết luận nghiệm chi tiết");

        var resultContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 20) };

        var lblHeaderFormula = new Label
        {
            Text = "CÔNG THỨC / PHƯƠNG TRÌNH:",
            Font = new Font("Segoe UI Semibold", 8.5F),
            ForeColor = ColorTextMuted,
            Dock = DockStyle.Top,
            Height = 20
        };

        lblMathFormula.Text = "Chưa có phương trình nào được thực thi.";
        lblMathFormula.Font = new Font("Segoe UI Semibold", 13F);
        lblMathFormula.ForeColor = ColorPrimaryDark;
        lblMathFormula.Dock = DockStyle.Top;
        lblMathFormula.Height = 36;

        var lblHeaderSteps = new Label
        {
            Text = "QUÁ TRÌNH PHÂN TÍCH / BIỆN LUẬN:",
            Font = new Font("Segoe UI Semibold", 8.5F),
            ForeColor = ColorTextMuted,
            Dock = DockStyle.Top,
            Height = 22,
            Margin = new Padding(0, 10, 0, 0)
        };

        lblMathSteps.Text = "Nhập các hệ số bên trái và chọn 'Giải Bậc 1', 'Giải Bậc 2' hoặc 'Tính tuổi'.";
        lblMathSteps.Font = new Font("Segoe UI", 10F);
        lblMathSteps.ForeColor = Color.FromArgb(71, 85, 105);
        lblMathSteps.Dock = DockStyle.Top;
        lblMathSteps.Height = 50;

        var lblHeaderConclusion = new Label
        {
            Text = "KẾT QUẢ / NGHIỆM:",
            Font = new Font("Segoe UI Semibold", 8.5F),
            ForeColor = ColorTextMuted,
            Dock = DockStyle.Top,
            Height = 22
        };

        var resultBox = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(240, 253, 244),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(16, 12, 16, 12)
        };

        lblMathResult.Text = "Sẵn sàng";
        lblMathResult.Font = new Font("Segoe UI Semibold", 14F);
        lblMathResult.ForeColor = ColorGreen;
        lblMathResult.Dock = DockStyle.Fill;
        lblMathResult.TextAlign = ContentAlignment.MiddleLeft;
        resultBox.Controls.Add(lblMathResult);

        lblMathTime.Text = "Thời gian thực thi: --";
        lblMathTime.Font = new Font("Segoe UI", 8.5F);
        lblMathTime.ForeColor = ColorTextMuted;
        lblMathTime.Dock = DockStyle.Bottom;
        lblMathTime.Height = 24;

        resultContainer.Controls.Add(lblMathTime);
        resultContainer.Controls.Add(resultBox);
        resultContainer.Controls.Add(lblHeaderConclusion);
        resultContainer.Controls.Add(lblMathSteps);
        resultContainer.Controls.Add(lblHeaderSteps);
        resultContainer.Controls.Add(lblMathFormula);
        resultContainer.Controls.Add(lblHeaderFormula);

        cardResult.Controls.Add(resultContainer);
        rightPanel.Controls.Add(cardResult);
        split.Controls.Add(rightPanel, 1, 0);

        page.Controls.Add(split);
        return page;
    }

    // =========================================================================
    // 4. TAB 2: QUẢN LÝ THƯ VIỆN (BÀI 3, 5, 6)
    // =========================================================================
    private Control CreateLibraryPage()
    {
        var page = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 14, 20, 14) };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = ColorCanvas
        };
        // Top Toolbar: Height 240px
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 240));
        // Bottom DataGrid: 100%
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // 1. TOP TOOLBAR CARD
        var cardFilter = CreateCard("BỘ LỌC TRA CỨU & BÁO CÁO THƯ VIỆN (CSDL: BTL_ThuVien)", "Thực hiện Bài 3 (Thủ tục đầu sách), Bài 5 (Tra cứu độc giả, quá hạn) và Bài 6 (Trigger)");
        var filterLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14, 8, 14, 8)
        };
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));

        // Group Tra cứu theo mã (ISBN, Mã Độc Giả)
        var pnlInputs = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        // Row ISBN
        var rowIsbn = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 6) };
        rowIsbn.Controls.Add(new Label { Text = "Mã ISBN:", Font = new Font("Segoe UI Semibold", 9F), ForeColor = ColorTextPrimary, Width = 85, Margin = new Padding(0, 6, 0, 0) });
        txtLibraryIsbn.Width = 140;
        txtLibraryIsbn.Height = 28;
        txtLibraryIsbn.BorderStyle = BorderStyle.FixedSingle;
        txtLibraryIsbn.Text = "ISBN001";
        rowIsbn.Controls.Add(txtLibraryIsbn);

        var btnSearchIsbn = CreateStyledButton("🔍 Bài 3 & 5b: Xem đầu sách", ColorAccent, Color.White, 185, 29);
        btnSearchIsbn.Click += (_, _) => SearchBookByIsbn();
        rowIsbn.Controls.Add(btnSearchIsbn);
        pnlInputs.Controls.Add(rowIsbn);

        // Row Reader
        var rowReader = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        rowReader.Controls.Add(new Label { Text = "Mã độc giả:", Font = new Font("Segoe UI Semibold", 9F), ForeColor = ColorTextPrimary, Width = 85, Margin = new Padding(0, 6, 0, 0) });
        txtLibraryReaderId.Width = 140;
        txtLibraryReaderId.Height = 28;
        txtLibraryReaderId.BorderStyle = BorderStyle.FixedSingle;
        txtLibraryReaderId.Text = "DG01";
        rowReader.Controls.Add(txtLibraryReaderId);

        var btnSearchReader = CreateStyledButton("👤 Bài 5a: Xem độc giả", ColorBlue, Color.White, 185, 29);
        btnSearchReader.Click += (_, _) => SearchReaderById();
        rowReader.Controls.Add(btnSearchReader);
        pnlInputs.Controls.Add(rowReader);

        filterLayout.Controls.Add(pnlInputs, 0, 0);

        // Group Báo cáo nhanh & Trigger
        var pnlReports = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(10, 0, 0, 0)
        };

        var btnBorrowing = CreateStyledButton("Bài 5c: Đang mượn", Color.FromArgb(41, 105, 176), Color.White, 145, 29);
        btnBorrowing.Click += (_, _) => ExecuteLibraryRoutine("sp_ThongtinNguoilonDangmuon", Array.Empty<(string, object)>(), "Độc giả người lớn đang mượn (Bài 5c)");

        var btnOverdue = CreateStyledButton("Bài 5d: Quá hạn > 14 ngày", ColorAmber, Color.White, 170, 29);
        btnOverdue.Click += (_, _) => ExecuteLibraryRoutine("sp_ThongtinNguoilonQuahan", Array.Empty<(string, object)>(), "Độc giả người lớn quá hạn > 14 ngày (Bài 5d)");

        var btnChildBorrow = CreateStyledButton("Bài 5e: Có trẻ em mượn", ColorPurple, Color.White, 160, 29);
        btnChildBorrow.Click += (_, _) => ExecuteLibraryRoutine("sp_DocGiaCoTreEmMuon", Array.Empty<(string, object)>(), "Độc giả có trẻ em cùng mượn (Bài 5e)");

        var btnTriggers = CreateStyledButton("⚡ Bài 6: DS Trigger", ColorGreen, Color.White, 135, 29);
        btnTriggers.Click += (_, _) => ExecuteLibraryRoutine("sp_KiemTraTrigger", Array.Empty<(string, object)>(), "Danh sách Triggers CSDL Thư viện (Bài 6.1 - 6.4)");

        pnlReports.Controls.Add(btnBorrowing);
        pnlReports.Controls.Add(btnOverdue);
        pnlReports.Controls.Add(btnChildBorrow);
        pnlReports.Controls.Add(btnTriggers);

        // Bài 6: chạy thử từng trigger trong transaction rồi ROLLBACK (dữ liệu không bị thay đổi)
        pnlReports.Controls.Add(CreateTriggerTestButton("6.1", "6.1 Xóa mượn", "tg_delMuon", 115));
        pnlReports.Controls.Add(CreateTriggerTestButton("6.2", "6.2 Thêm mượn", "tg_insMuon", 120));
        pnlReports.Controls.Add(CreateTriggerTestButton("6.3", "6.3 Sửa cuốn sách", "tg_updCuonSach", 140));
        pnlReports.Controls.Add(CreateTriggerTestButton("6.4", "6.4 Thêm/sửa tựa sách", "tg_InfThongBao", 165));

        // Quick sample chips
        var chipPanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 8, 0, 0) };
        chipPanel.Controls.Add(new Label { Text = "Mẫu thử:", Font = new Font("Segoe UI", 8.5F), ForeColor = ColorTextMuted, AutoSize = true, Margin = new Padding(0, 4, 6, 0) });
        chipPanel.Controls.Add(CreateChip("ISBN001", () => txtLibraryIsbn.Text = "ISBN001"));
        chipPanel.Controls.Add(CreateChip("ISBN002", () => txtLibraryIsbn.Text = "ISBN002"));
        chipPanel.Controls.Add(CreateChip("ISBN003", () => txtLibraryIsbn.Text = "ISBN003"));
        chipPanel.Controls.Add(CreateChip("DG01 (Người lớn)", () => txtLibraryReaderId.Text = "DG01"));
        chipPanel.Controls.Add(CreateChip("DG02 (Trẻ em)", () => txtLibraryReaderId.Text = "DG02"));
        chipPanel.Controls.Add(CreateChip("DG06 (Trẻ em)", () => txtLibraryReaderId.Text = "DG06"));
        pnlReports.Controls.Add(chipPanel);

        filterLayout.Controls.Add(pnlReports, 1, 0);

        cardFilter.Controls.Add(filterLayout);
        layout.Controls.Add(cardFilter, 0, 0);

        // 2. BOTTOM DATAGRID CARD
        var cardGrid = CreateCard();
        cardGrid.Padding = new Padding(16, 12, 16, 12);
        cardGrid.Margin = new Padding(0, 10, 0, 0);

        var gridHeader = new TableLayoutPanel { Dock = DockStyle.Top, Height = 30, ColumnCount = 2, RowCount = 1 };
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        lblLibraryGridTitle.Text = "KẾT QUẢ TRUY VẤN - BTL_ThuVien";
        lblLibraryGridTitle.Font = new Font("Segoe UI Semibold", 9.5F);
        lblLibraryGridTitle.ForeColor = ColorPrimaryDark;
        lblLibraryGridTitle.Dock = DockStyle.Fill;
        gridHeader.Controls.Add(lblLibraryGridTitle, 0, 0);

        lblLibraryRowCount.Text = "0 bản ghi";
        lblLibraryRowCount.Font = new Font("Segoe UI Semibold", 9F);
        lblLibraryRowCount.ForeColor = ColorAccent;
        lblLibraryRowCount.TextAlign = ContentAlignment.MiddleRight;
        lblLibraryRowCount.Dock = DockStyle.Fill;
        gridHeader.Controls.Add(lblLibraryRowCount, 1, 0);

        cardGrid.Controls.Add(gridHeader);
        ConfigureGrid(gridLibrary);
        cardGrid.Controls.Add(gridLibrary);

        layout.Controls.Add(cardGrid, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    // =========================================================================
    // 5. TAB 3: QUẢN LÝ ĐỀ ÁN (BÀI 7, 8)
    // =========================================================================
    private Control CreateProjectPage()
    {
        var page = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 14, 20, 14) };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = ColorCanvas
        };
        // Top Toolbar: Height 200px
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        // Bottom DataGrid: 100%
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // 1. TOP TOOLBAR CARD
        var cardFilter = CreateCard("BỘ LỌC TRA CỨU & BÁO CÁO ĐỀ ÁN (CSDL: BTL_DeAn)", "Thực hiện Bài 7 (Lương, Tiền thưởng, Báo cáo theo phòng) và Bài 8 (Thống kê dự án & phòng ban)");
        var filterLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14, 6, 14, 6)
        };
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));

        // LEFT: Parametric Procedures (Bài 7.1, 7.2, 7.4)
        var pnlLeft = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        // Row Dept
        var rowDept = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 5) };
        rowDept.Controls.Add(new Label { Text = "Mã phòng:", Font = new Font("Segoe UI Semibold", 8.8F), ForeColor = ColorTextPrimary, Width = 80, Margin = new Padding(0, 5, 0, 0) });
        txtProjectDeptId.Width = 65;
        txtProjectDeptId.Height = 27;
        txtProjectDeptId.BorderStyle = BorderStyle.FixedSingle;
        txtProjectDeptId.Text = "5";
        rowDept.Controls.Add(txtProjectDeptId);

        var btnSalaryDept = CreateStyledButton("Bài 7.1 Lương TB phòng", ColorAccent, Color.White, 160, 28);
        btnSalaryDept.Click += (_, _) => SearchDeptSalary();
        rowDept.Controls.Add(btnSalaryDept);
        pnlLeft.Controls.Add(rowDept);

        // Row Emp & Project
        var rowEmpProj = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 5) };
        rowEmpProj.Controls.Add(new Label { Text = "Mã NV / DA:", Font = new Font("Segoe UI Semibold", 8.8F), ForeColor = ColorTextPrimary, Width = 80, Margin = new Padding(0, 5, 0, 0) });
        txtProjectEmpId.Width = 65;
        txtProjectEmpId.Height = 27;
        txtProjectEmpId.BorderStyle = BorderStyle.FixedSingle;
        txtProjectEmpId.Text = "NV001";
        rowEmpProj.Controls.Add(txtProjectEmpId);

        txtProjectId.Width = 50;
        txtProjectId.Height = 27;
        txtProjectId.BorderStyle = BorderStyle.FixedSingle;
        txtProjectId.Text = "1";
        rowEmpProj.Controls.Add(txtProjectId);

        var btnSalaryProject = CreateStyledButton("Bài 7.2 Lương theo DA", ColorBlue, Color.White, 150, 28);
        btnSalaryProject.Click += (_, _) => SearchEmployeeProjectSalary();
        rowEmpProj.Controls.Add(btnSalaryProject);

        var btnBonus = CreateStyledButton("Bài 7.4 Tiền thưởng", ColorAmber, Color.White, 135, 28);
        btnBonus.Click += (_, _) => SearchEmployeeBonus();
        rowEmpProj.Controls.Add(btnBonus);
        pnlLeft.Controls.Add(rowEmpProj);

        // Sample chips
        var chipPanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 2, 0, 0) };
        chipPanel.Controls.Add(new Label { Text = "Mẫu thử:", Font = new Font("Segoe UI", 8.2F), ForeColor = ColorTextMuted, AutoSize = true, Margin = new Padding(0, 4, 4, 0) });
        chipPanel.Controls.Add(CreateChip("NV001", () => txtProjectEmpId.Text = "NV001"));
        chipPanel.Controls.Add(CreateChip("NV002", () => txtProjectEmpId.Text = "NV002"));
        chipPanel.Controls.Add(CreateChip("DA 1", () => txtProjectId.Text = "1"));
        chipPanel.Controls.Add(CreateChip("Phòng 5", () => txtProjectDeptId.Text = "5"));
        pnlLeft.Controls.Add(chipPanel);

        filterLayout.Controls.Add(pnlLeft, 0, 0);

        // RIGHT: Reports Bài 7.3 - 7.6 & Bài 8.1 - 8.5
        var pnlReports = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(8, 0, 0, 0)
        };

        pnlReports.Controls.Add(CreateStyledButton("Bài 7.3 Lương TB các phòng", Color.FromArgb(56, 102, 165), Color.White, 175, 28, () => ExecuteProjectRoutine("sp_fn_LuongTrungBinhCacPhong", Array.Empty<(string, object)>(), "Bài 7.3: Lương TB các phòng ban")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 7.5 Số DA / phòng", Color.FromArgb(56, 102, 165), Color.White, 150, 28, () => ExecuteProjectRoutine("sp_fn_SoDuAnTheoPhong", Array.Empty<(string, object)>(), "Bài 7.5: Số đề án theo từng phòng")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 7.6 NV (Inline)", Color.FromArgb(56, 102, 165), Color.White, 135, 28, () => ExecuteProjectRoutine("sp_fn_ThongTinNhanVien_Inline", Array.Empty<(string, object)>(), "Bài 7.6 (Cách 1 - Inline): Thông tin nhân viên")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 7.6 NV (Multi-statement)", Color.FromArgb(56, 102, 165), Color.White, 185, 28, () => ExecuteProjectRoutine("sp_fn_ThongTinNhanVien_Multi", Array.Empty<(string, object)>(), "Bài 7.6 (Cách 2 - Multi-statement): Thông tin nhân viên")));

        pnlReports.Controls.Add(CreateStyledButton("Bài 8.1 Dự án > 2 NV", ColorPurple, Color.White, 150, 28, () => ExecuteProjectRoutine("sp_fn_DuAnNhieuNhanVien", Array.Empty<(string, object)>(), "Bài 8.1: Các dự án có trên 2 nhân viên")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 8.2 Phòng > 2 NV lương > 25k", ColorPurple, Color.White, 205, 28, () => ExecuteProjectRoutine("sp_fn_PhongNhieuNhanVienLuongCao", Array.Empty<(string, object)>(), "Bài 8.2: Phòng có trên 2 NV và có lương > 25000")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 8.3 Phòng lương TB > 30k", ColorPurple, Color.White, 180, 28, () => ExecuteProjectRoutine("sp_fn_PhongLuongCao", Array.Empty<(string, object)>(), "Bài 8.3: Phòng có lương TB > 30.000")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 8.4 NV Nam phòng TB > 30k", ColorPurple, Color.White, 195, 28, () => ExecuteProjectRoutine("sp_fn_PhongNhieuNhanVienNam", Array.Empty<(string, object)>(), "Bài 8.4: Số NV nam ở phòng lương TB > 30.000")));
        pnlReports.Controls.Add(CreateStyledButton("Bài 8.5 Dự án phòng 5", ColorPurple, Color.White, 150, 28, () => ExecuteProjectRoutine("sp_fn_DuAnNhanVienPhong5", Array.Empty<(string, object)>(), "Bài 8.5: Dự án và số NV thuộc phòng 5")));

        filterLayout.Controls.Add(pnlReports, 1, 0);

        cardFilter.Controls.Add(filterLayout);
        layout.Controls.Add(cardFilter, 0, 0);

        // 2. BOTTOM DATAGRID CARD
        var cardGrid = CreateCard();
        cardGrid.Padding = new Padding(16, 12, 16, 12);
        cardGrid.Margin = new Padding(0, 10, 0, 0);

        var gridHeader = new TableLayoutPanel { Dock = DockStyle.Top, Height = 30, ColumnCount = 2, RowCount = 1 };
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        lblProjectGridTitle.Text = "KẾT QUẢ TRUY VẤN - BTL_DeAn";
        lblProjectGridTitle.Font = new Font("Segoe UI Semibold", 9.5F);
        lblProjectGridTitle.ForeColor = ColorPrimaryDark;
        lblProjectGridTitle.Dock = DockStyle.Fill;
        gridHeader.Controls.Add(lblProjectGridTitle, 0, 0);

        lblProjectRowCount.Text = "0 bản ghi";
        lblProjectRowCount.Font = new Font("Segoe UI Semibold", 9F);
        lblProjectRowCount.ForeColor = ColorAccent;
        lblProjectRowCount.TextAlign = ContentAlignment.MiddleRight;
        lblProjectRowCount.Dock = DockStyle.Fill;
        gridHeader.Controls.Add(lblProjectRowCount, 1, 0);

        cardGrid.Controls.Add(gridHeader);
        ConfigureGrid(gridProject);
        cardGrid.Controls.Add(gridProject);

        layout.Controls.Add(cardGrid, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    // =========================================================================
    // 6. COLLAPSIBLE LOG DRAWER & STATUS BAR
    // =========================================================================
    private Control CreateLogDrawer()
    {
        logDrawerPanel.Dock = DockStyle.Fill;
        logDrawerPanel.BackColor = Color.White;
        logDrawerPanel.BorderStyle = BorderStyle.FixedSingle;
        logDrawerPanel.Padding = new Padding(12, 8, 12, 8);

        var title = new Label
        {
            Text = "NHẬT KÝ HOẠT ĐỘNG (ACTIVITY LOG):",
            Font = new Font("Segoe UI Semibold", 8.5F),
            ForeColor = ColorTextMuted,
            Dock = DockStyle.Top,
            Height = 22
        };

        logTextBox.Multiline = true;
        logTextBox.Dock = DockStyle.Fill;
        logTextBox.ScrollBars = ScrollBars.Vertical;
        logTextBox.ReadOnly = true;
        logTextBox.BorderStyle = BorderStyle.None;
        logTextBox.BackColor = Color.FromArgb(248, 250, 252);
        logTextBox.Font = new Font("Consolas", 9F);

        logDrawerPanel.Controls.Add(logTextBox);
        logDrawerPanel.Controls.Add(title);
        return logDrawerPanel;
    }

    private Control CreateStatusBar(TableLayoutPanel parentLayout)
    {
        var bar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(16, 4, 16, 4)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));

        statusDbLabel.Text = "CSDL: Chưa chọn";
        statusDbLabel.Font = new Font("Segoe UI Semibold", 8.8F);
        statusDbLabel.ForeColor = ColorTextMuted;
        statusDbLabel.Dock = DockStyle.Fill;
        statusDbLabel.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(statusDbLabel, 0, 0);

        statusInfoLabel.Text = "Sẵn sàng thực thi.";
        statusInfoLabel.Font = new Font("Segoe UI", 8.8F);
        statusInfoLabel.ForeColor = ColorTextPrimary;
        statusInfoLabel.Dock = DockStyle.Fill;
        statusInfoLabel.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(statusInfoLabel, 1, 0);

        statusRowCountLabel.Text = "";
        statusRowCountLabel.Font = new Font("Segoe UI Semibold", 8.8F);
        statusRowCountLabel.ForeColor = ColorAccent;
        statusRowCountLabel.Dock = DockStyle.Fill;
        statusRowCountLabel.TextAlign = ContentAlignment.MiddleRight;
        layout.Controls.Add(statusRowCountLabel, 2, 0);

        toggleLogButton.Text = "📋 Xem Log";
        toggleLogButton.Font = new Font("Segoe UI Semibold", 8.5F);
        toggleLogButton.FlatStyle = FlatStyle.Flat;
        toggleLogButton.ForeColor = ColorTextMuted;
        toggleLogButton.BackColor = Color.FromArgb(241, 245, 249);
        toggleLogButton.FlatAppearance.BorderSize = 0;
        toggleLogButton.Dock = DockStyle.Fill;
        toggleLogButton.Cursor = Cursors.Hand;
        toggleLogButton.Click += (_, _) =>
        {
            var isHidden = parentLayout.RowStyles[3].Height == 0;
            parentLayout.RowStyles[3].Height = isHidden ? 130 : 0;
            toggleLogButton.Text = isHidden ? "✖ Đóng Log" : "📋 Xem Log";
            toggleLogButton.ForeColor = isHidden ? ColorRed : ColorTextMuted;
        };
        layout.Controls.Add(toggleLogButton, 3, 0);

        bar.Controls.Add(layout);
        return bar;
    }

    private void UpdateStatusBarContext()
    {
        switch (currentTabKey)
        {
            case "math":
                statusDbLabel.Text = "Module: Thuật toán C# thuần";
                break;
            case "library":
                statusDbLabel.Text = "CSDL: BTL_ThuVien";
                break;
            case "project":
                statusDbLabel.Text = "CSDL: BTL_DeAn";
                break;
        }
    }

    // =========================================================================
    // 7. LOGIC TOÁN HỌC (BÀI 1, BÀI 2, BÀI 4) - ĐÃ SỬA LỖI VALIDATION
    // =========================================================================
    private void SolveLinear()
    {
        // Sửa lỗi: Chỉ đọc a và b, KHÔNG yêu cầu c
        if (!double.TryParse(txtA.Text.Trim(), out var a) || !double.TryParse(txtB.Text.Trim(), out var b))
        {
            ShowMathError("Bài 1: Phương trình bậc 1 (ax + b = 0)", "Vui lòng nhập đúng giá trị số thực cho hệ số a và b.");
            txtA.Focus();
            return;
        }

        var formulaStr = $"{FormatTerm(a, "x", true)} {(b >= 0 ? "+ " + b : "- " + Math.Abs(b))} = 0";

        if (chkRunOnMysql.Checked)
        {
            try
            {
                var table = DatabaseService.ExecuteRoutine(connectionTextBox.Text, "sp_GiaiPTBac1", new[] {
                    ("p_a", (object)a),
                    ("p_b", (object)b)
                }, "BTL_ThuVien");

                var ketLuan = table.Rows.Count > 0 ? table.Rows[0]["KetLuan"]?.ToString() ?? "" : "";
                var nghiem = table.Rows.Count > 0 && table.Rows[0]["Nghiem"] != DBNull.Value ? table.Rows[0]["Nghiem"]?.ToString() : null;
                var finalRes = nghiem != null ? $"{ketLuan}: x = {nghiem}" : ketLuan;

                DisplayMathResult("Bài 1: " + formulaStr, "Thực thi qua Stored Procedure: sp_GiaiPTBac1(a, b) trong CSDL BTL_ThuVien", finalRes);
                WriteLog($"[Bài 1 - SP MySQL] Giải {formulaStr}  ⇒  {finalRes}");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gọi thủ tục 'sp_GiaiPTBac1' trên MySQL thất bại:\n{ex.Message}\n\nỨng dụng sẽ chuyển sang giải bằng thuật toán C# nội bộ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        string steps;
        string result;

        if (Math.Abs(a) < double.Epsilon)
        {
            steps = "Hệ số a = 0. Phương trình có dạng: 0x + " + b + " = 0.";
            result = Math.Abs(b) < double.Epsilon ? "Phương trình có Vô số nghiệm (x ∈ ℝ)" : "Phương trình Vô nghiệm";
        }
        else
        {
            var x = -b / a;
            steps = $"Chuyển vế: {a}x = {-b}  ⇒  x = {-b} / {a}";
            result = $"Nghiệm: x = {x:0.####}";
        }

        DisplayMathResult("Bài 1: " + formulaStr, steps + " (Tính toán thuật toán C#)", result);
        WriteLog($"[Bài 1 - C#] Giải {formulaStr}  ⇒  {result}");
    }

    private void SolveQuadratic()
    {
        if (!double.TryParse(txtA.Text.Trim(), out var a) ||
            !double.TryParse(txtB.Text.Trim(), out var b) ||
            !double.TryParse(txtC.Text.Trim(), out var c))
        {
            ShowMathError("Bài 2: Phương trình bậc 2 (ax² + bx + c = 0)", "Vui lòng nhập đầy đủ hệ số a, b và c (số thực).");
            return;
        }

        var formulaStr = $"{FormatTerm(a, "x²", true)} {FormatTerm(b, "x", false)} {(c >= 0 ? "+ " + c : "- " + Math.Abs(c))} = 0";

        if (chkRunOnMysql.Checked)
        {
            try
            {
                var table = DatabaseService.ExecuteRoutine(connectionTextBox.Text, "sp_GiaiPTBac2", new[] {
                    ("p_a", (object)a),
                    ("p_b", (object)b),
                    ("p_c", (object)c)
                }, "BTL_ThuVien");

                var ketQua = table.Rows.Count > 0 ? table.Rows[0]["KetQua"]?.ToString() ?? "" : "";
                DisplayMathResult("Bài 2: " + formulaStr, "Thực thi qua Stored Procedure: sp_GiaiPTBac2 (gọi Hàm fn_GiaiPTBac2 trong CSDL BTL_ThuVien)", ketQua);
                WriteLog($"[Bài 2 - SP MySQL] Giải {formulaStr}  ⇒  {ketQua}");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gọi thủ tục 'sp_GiaiPTBac2' trên MySQL thất bại:\n{ex.Message}\n\nỨng dụng sẽ chuyển sang giải bằng thuật toán C# nội bộ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        if (Math.Abs(a) < double.Epsilon)
        {
            var steps = "Vì hệ số a = 0 nên phương trình trở thành bậc 1: " + $"{b}x + {c} = 0";
            string res;
            if (Math.Abs(b) < double.Epsilon)
            {
                res = Math.Abs(c) < double.Epsilon ? "Vô số nghiệm (x ∈ ℝ)" : "Vô nghiệm";
            }
            else
            {
                var x = -c / b;
                res = $"Nghiệm: x = {x:0.####}";
            }
            DisplayMathResult("Bài 2: " + formulaStr, steps, res);
            WriteLog($"[Bài 2 - C#] Giải (a=0) {formulaStr}  ⇒  {res}");
            return;
        }

        var delta = b * b - 4 * a * c;
        string analysis = $"Biệt thức Δ = b² - 4ac = ({b})² - 4*({a})*({c}) = {delta:0.####}";
        string finalResult;

        if (delta < 0)
        {
            analysis += " < 0  ⇒  Phương trình không có nghiệm thực.";
            finalResult = "Phương trình Vô nghiệm thực";
        }
        else if (Math.Abs(delta) < double.Epsilon)
        {
            var x = -b / (2 * a);
            analysis += " = 0  ⇒  Phương trình có nghiệm kép x = -b / (2a).";
            finalResult = $"Nghiệm kép: x₁ = x₂ = {x:0.####}";
        }
        else
        {
            var sqrtDelta = Math.Sqrt(delta);
            var x1 = (-b + sqrtDelta) / (2 * a);
            var x2 = (-b - sqrtDelta) / (2 * a);
            analysis += $" > 0  ⇒  Phương trình có 2 nghiệm phân biệt (√Δ = {sqrtDelta:0.####}).";
            finalResult = $"x₁ = {x1:0.####}   và   x₂ = {x2:0.####}";
        }

        DisplayMathResult("Bài 2: " + formulaStr, analysis + " (Tính toán thuật toán C#)", finalResult);
        WriteLog($"[Bài 2 - C#] Giải {formulaStr}  ⇒  {finalResult}");
    }

    private void CalculateAge()
    {
        if (!int.TryParse(txtYear.Text.Trim(), out var year) || year < 1900 || year > DateTime.Now.Year)
        {
            ShowMathError("Bài 4: Tính tuổi", $"Năm sinh không hợp lệ. Vui lòng nhập năm trong khoảng 1900 đến {DateTime.Now.Year}.");
            txtYear.Focus();
            return;
        }

        if (chkRunOnMysql.Checked)
        {
            try
            {
                var table = DatabaseService.ExecuteRoutine(connectionTextBox.Text, "sp_TinhTuoi", new[] {
                    ("p_namsinh", (object)year)
                }, "BTL_ThuVien");

                var tuoi = table.Rows.Count > 0 ? table.Rows[0]["Tuoi"]?.ToString() : (DateTime.Now.Year - year).ToString();
                var resultSql = $"Tuổi hiện tại: {tuoi} tuổi";
                DisplayMathResult($"Bài 4: Năm sinh {year}  •  Năm hiện tại: {DateTime.Now.Year}", "Thực thi qua Stored Procedure: sp_TinhTuoi (gọi Hàm fn_TinhTuoi trong CSDL BTL_ThuVien)", resultSql);
                WriteLog($"[Bài 4 - SP MySQL] Sinh năm {year}  ⇒  {tuoi} tuổi");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gọi thủ tục 'sp_TinhTuoi' trên MySQL thất bại:\n{ex.Message}\n\nỨng dụng sẽ chuyển sang tính bằng C#.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        var age = DateTime.Now.Year - year;
        var formula = $"Bài 4: Năm sinh: {year}  •  Năm hiện tại: {DateTime.Now.Year}";
        var steps = $"Công thức: Tuổi = Năm hiện tại - Năm sinh = {DateTime.Now.Year} - {year} (Tính toán C#)";
        var result = $"Tuổi hiện tại: {age} tuổi";

        DisplayMathResult(formula, steps, result);
        WriteLog($"[Bài 4 - C#] Sinh năm {year}  ⇒  {age} tuổi");
    }

    private static string FormatTerm(double coef, string varName, bool isFirst)
    {
        if (isFirst)
        {
            if (Math.Abs(coef - 1) < double.Epsilon) return varName;
            if (Math.Abs(coef + 1) < double.Epsilon) return "-" + varName;
            return $"{coef}{varName}";
        }

        if (coef >= 0)
        {
            if (Math.Abs(coef - 1) < double.Epsilon) return $"+ {varName}";
            return $"+ {coef}{varName}";
        }
        else
        {
            if (Math.Abs(coef + 1) < double.Epsilon) return $"- {varName}";
            return $"- {Math.Abs(coef)}{varName}";
        }
    }

    private void DisplayMathResult(string formula, string steps, string result)
    {
        lblMathFormula.Text = formula;
        lblMathSteps.Text = steps;
        lblMathResult.Text = result;
        lblMathResult.ForeColor = result.Contains("Vô nghiệm") ? ColorRed : ColorGreen;
        lblMathTime.Text = $"Thời gian tính: {DateTime.Now:HH:mm:ss dd/MM/yyyy}";
        statusInfoLabel.Text = $"[Thành công] Đã tính xong: {result}";
    }

    private void ShowMathError(string title, string message)
    {
        lblMathFormula.Text = title;
        lblMathSteps.Text = message;
        lblMathResult.Text = "Lỗi nhập liệu";
        lblMathResult.ForeColor = ColorRed;
        statusInfoLabel.Text = "[Lỗi] " + message;
        WriteLog($"[Lỗi nhập liệu] {message}");
    }

    // =========================================================================
    // 8. LOGIC THƯ VIỆN & ĐỀ ÁN (BÀI 3, 5, 6, 7, 8) - ĐÃ CÓ VALIDATION
    // =========================================================================
    private void SearchBookByIsbn()
    {
        var isbn = txtLibraryIsbn.Text.Trim();
        if (string.IsNullOrEmpty(isbn))
        {
            MessageBox.Show("Vui lòng nhập mã ISBN cần tra cứu (Ví dụ: ISBN001).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtLibraryIsbn.Focus();
            return;
        }

        ExecuteLibraryRoutine("sp_ThongtinDausach", new[] { ("p_isbn", (object)isbn) }, $"Thông tin đầu sách '{isbn}'");
    }

    private void SearchReaderById()
    {
        var readerId = txtLibraryReaderId.Text.Trim();
        if (string.IsNullOrEmpty(readerId))
        {
            MessageBox.Show("Vui lòng nhập mã độc giả cần tra cứu (Ví dụ: DG01).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtLibraryReaderId.Focus();
            return;
        }

        ExecuteLibraryRoutine("sp_ThongtinDocGia", new[] { ("p_maDocGia", (object)readerId) }, $"Thông tin độc giả '{readerId}'");
    }

    private void ExecuteLibraryRoutine(string routine, (string Name, object Value)[] parameters, string displayName)
    {
        try
        {
            var table = DatabaseService.ExecuteRoutine(connectionTextBox.Text, routine, parameters, "BTL_ThuVien");
            gridLibrary.DataSource = table;
            lblLibraryGridTitle.Text = $"KẾT QUẢ: {displayName.ToUpper()} ({routine})";
            lblLibraryRowCount.Text = $"{table.Rows.Count} bản ghi";
            statusInfoLabel.Text = $"Đã thực thi {routine}: nhận {table.Rows.Count} dòng dữ liệu.";
            statusRowCountLabel.Text = $"Tổng số: {table.Rows.Count} dòng";
            WriteLog($"[Thư viện] {routine} ({displayName}) thành công  ⇒  {table.Rows.Count} dòng.");
        }
        catch (Exception ex)
        {
            HandleQueryError("BTL_ThuVien", routine, ex, lblLibraryRowCount);
        }
    }

    private void SearchDeptSalary()
    {
        var dept = txtProjectDeptId.Text.Trim();
        if (string.IsNullOrEmpty(dept))
        {
            MessageBox.Show("Vui lòng nhập mã phòng ban (Ví dụ: 5).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtProjectDeptId.Focus();
            return;
        }

        ExecuteProjectRoutine("sp_fn_LuongTrungBinhPhong", new[] { ("p_maPhong", (object)dept) }, $"Bài 7.1: Lương trung bình phòng {dept}");
    }

    private void SearchEmployeeProjectSalary()
    {
        var empId = txtProjectEmpId.Text.Trim();
        var projId = txtProjectId.Text.Trim();
        if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(projId))
        {
            MessageBox.Show("Vui lòng nhập cả Mã nhân viên (vd: NV001) và Mã dự án (vd: 1).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (string.IsNullOrEmpty(empId)) txtProjectEmpId.Focus(); else txtProjectId.Focus();
            return;
        }

        ExecuteProjectRoutine("sp_fn_LuongNhanVienDuAn", new[] { ("p_maNv", (object)empId), ("p_maDa", (object)projId) }, $"Bài 7.2: Lương NV '{empId}' trong DA '{projId}'");
    }

    private void SearchEmployeeBonus()
    {
        var empId = txtProjectEmpId.Text.Trim();
        if (string.IsNullOrEmpty(empId))
        {
            MessageBox.Show("Vui lòng nhập Mã nhân viên (Ví dụ: NV001).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtProjectEmpId.Focus();
            return;
        }

        ExecuteProjectRoutine("sp_fn_TienThuongNhanVien", new[] { ("p_maNv", (object)empId) }, $"Bài 7.4: Tiền thưởng của nhân viên '{empId}'");
    }

    private void ExecuteProjectRoutine(string routine, (string Name, object Value)[] parameters, string displayName)
    {
        try
        {
            var table = DatabaseService.ExecuteRoutine(connectionTextBox.Text, routine, parameters, "BTL_DeAn");
            gridProject.DataSource = table;
            lblProjectGridTitle.Text = $"KẾT QUẢ: {displayName.ToUpper()} ({routine})";
            lblProjectRowCount.Text = $"{table.Rows.Count} bản ghi";
            statusInfoLabel.Text = $"Đã thực thi {routine}: nhận {table.Rows.Count} dòng dữ liệu.";
            statusRowCountLabel.Text = $"Tổng số: {table.Rows.Count} dòng";
            WriteLog($"[Đề án] {routine} ({displayName}) thành công  ⇒  {table.Rows.Count} dòng.");
        }
        catch (Exception ex)
        {
            HandleQueryError("BTL_DeAn", routine, ex, lblProjectRowCount);
        }
    }

    private void HandleQueryError(string db, string routine, Exception ex, Label rowCountLabel)
    {
        rowCountLabel.Text = "Lỗi truy vấn";
        statusInfoLabel.Text = $"[Thất bại] {routine}: {ex.Message}";
        statusRowCountLabel.Text = "0 dòng";
        WriteLog($"[Lỗi {db} - {routine}] {ex.Message}");
        MessageBox.Show($"Thực thi thủ tục '{routine}' trên CSDL '{db}' thất bại:\n\n{ex.Message}", "Lỗi truy vấn CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void TestConnection()
    {
        try
        {
            DatabaseService.TestConnection(connectionTextBox.Text);
            statusBadgeLabel.Text = "● Đã kết nối";
            statusBadgeLabel.ForeColor = Color.White;
            statusBadgeLabel.BackColor = ColorGreen;
            statusInfoLabel.Text = "Kết nối MySQL thành công.";
            WriteLog("Kết nối máy chủ MySQL thành công.");
            MessageBox.Show("Kết nối đến MySQL Server thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            statusBadgeLabel.Text = "● Lỗi kết nối";
            statusBadgeLabel.ForeColor = Color.White;
            statusBadgeLabel.BackColor = ColorRed;
            statusInfoLabel.Text = "Kết nối thất bại. Xem chi tiết trong log.";
            WriteLog($"Lỗi kết nối MySQL: {ex.Message}");
            MessageBox.Show(ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void WriteLog(string message)
    {
        logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    // =========================================================================
    // 9. HELPER UI FACTORY METHODS
    // =========================================================================
    private static Panel CreateCard(string? title = null, string? subtitle = null)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ColorCardBg,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18, 14, 18, 14)
        };

        if (!string.IsNullOrEmpty(title))
        {
            var header = new Panel { Dock = DockStyle.Top, Height = subtitle != null ? 48 : 30 };
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 10.5F),
                ForeColor = ColorPrimaryDark,
                Dock = DockStyle.Top,
                Height = 24
            };
            header.Controls.Add(lblTitle);

            if (!string.IsNullOrEmpty(subtitle))
            {
                var lblSub = new Label
                {
                    Text = subtitle,
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = ColorTextMuted,
                    Dock = DockStyle.Top,
                    Height = 20
                };
                header.Controls.Add(lblSub);
            }
            card.Controls.Add(header);
        }

        return card;
    }

    private static Button CreateStyledButton(string text, Color backColor, Color foreColor, int width, int height, Action? onClick = null)
    {
        var btn = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 8.8F),
            Width = width,
            Height = height,
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false,
            Margin = new Padding(0, 0, 8, 6)
        };
        btn.FlatAppearance.BorderSize = 0;
        if (onClick != null) btn.Click += (_, _) => onClick();
        return btn;
    }

    private Button CreateTriggerTestButton(string bai, string text, string triggerName, int width)
    {
        return CreateStyledButton(text, ColorGreen, Color.White, width, 29, () =>
            ExecuteLibraryRoutine("sp_KiemThuTrigger", new[] { ("p_bai", (object)bai) }, $"Bài {bai}: Kiểm thử trigger {triggerName} (ROLLBACK sau khi chạy)"));
    }

    private static Button CreateChip(string text, Action onClick)
    {
        var chip = new Button
        {
            Text = text,
            AutoSize = true,
            Height = 24,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = ColorTextPrimary,
            Font = new Font("Segoe UI", 8F),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 6, 0),
            Padding = new Padding(6, 0, 6, 0)
        };
        chip.FlatAppearance.BorderColor = ColorBorder;
        chip.Click += (_, _) => onClick();
        return chip;
    }

    private static void AddInputRow(TableLayoutPanel parent, string labelText, TextBox input, string placeholder, int row)
    {
        var lbl = new Label
        {
            Text = labelText,
            Font = new Font("Segoe UI Semibold", 9F),
            ForeColor = ColorTextPrimary,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        input.Dock = DockStyle.Fill;
        input.Height = 28;
        input.BorderStyle = BorderStyle.FixedSingle;
        input.PlaceholderText = placeholder;
        input.Margin = new Padding(0, 3, 0, 8);

        parent.Controls.Add(lbl, 0, row);
        parent.Controls.Add(input, 1, row);
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 34;

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = ColorPrimaryDark,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F),
            Padding = new Padding(8, 6, 8, 6),
            Alignment = DataGridViewContentAlignment.MiddleLeft
        };

        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White,
            ForeColor = ColorTextPrimary,
            SelectionBackColor = Color.FromArgb(204, 251, 241),
            SelectionForeColor = ColorTextPrimary,
            Padding = new Padding(8, 6, 8, 6),
            Font = new Font("Segoe UI", 9F)
        };

        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(248, 250, 252)
        };
    }
}
