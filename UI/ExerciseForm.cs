using System.Data;
using System.Globalization;
using System.Text;
using BTLWinForms.Data;
using BTLWinForms.Exercises;

namespace BTLWinForms.UI;

/// <summary>
/// Giao diện con của một bài tập. Gồm nút kết nối CSDL của bài, danh sách câu nhỏ (nếu có) và với mỗi câu:
/// câu hỏi, kết quả trả về, mã nguồn hàm / thủ tục / trigger, code kết nối và lời gọi.
/// </summary>
internal sealed class ExerciseForm : Form
{
    private const int ResultTab = 0;
    private const int SourceTab = 1;

    private readonly Exercise _exercise;
    private readonly ConnectionSettings _settings;

    private readonly StatusIndicator _connectionStatus = new();
    private readonly FlatButton _connectButton = new("Kết nối CSDL", ButtonVariant.Primary);
    private readonly NavList? _nav;

    private readonly Label _itemHeading = Ui.Label("", Theme.Heading);
    private readonly Label _question = Ui.WrappingLabel("");
    private readonly Label _note = Ui.WrappingLabel("", Theme.Small, Theme.TextMuted);
    private readonly FlowLayoutPanel _parameterPanel = new() { AutoSize = true, WrapContents = true, Dock = DockStyle.Fill, Margin = new Padding(0, 16, 0, 0) };
    private readonly FlatButton _runButton = new("Thực hiện", ButtonVariant.Primary);
    private readonly Label _hint = Ui.WrappingLabel("", Theme.Small, Theme.TextMuted);
    private readonly Label _error = Ui.WrappingLabel("", Theme.Body, Theme.Danger);

    private readonly TabStrip _tabs;
    private readonly List<Control> _tabPages = [];
    private readonly DataGridView _grid = new();
    private readonly Label _emptyState = Ui.Label("", Theme.Body, Theme.TextMuted);
    private readonly TextBox _sourceView = Ui.CodeView();
    private readonly TextBox _callView = Ui.CodeView();
    private readonly Label _resultStatus = Ui.Label("", Theme.Small, Theme.TextMuted);

    private readonly Dictionary<string, TextBox> _inputs = [];
    private readonly Dictionary<string, string> _enteredValues = [];
    private readonly Dictionary<string, string> _sourceCache = [];
    private readonly CancellationTokenSource _lifetime = new();

    private ExerciseItem _current;
    private bool _connected;
    private bool _busy;

    public ExerciseForm(Exercise exercise, ConnectionSettings settings)
    {
        _exercise = exercise;
        _settings = settings;
        _current = exercise.Items[0];

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = $"Bài {exercise.Number} - {exercise.Title}";
        Font = Theme.Body;
        BackColor = Theme.Background;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1120, 740);
        MinimumSize = new Size(820, 600);
        ShowInTaskbar = false;

        var tabNames = new List<string> { "Kết quả", "Hàm / Thủ tục / Trigger", "Code kết nối & lời gọi" };
        if (exercise.Schema is not null) tabNames.Add("Lược đồ CSDL");
        _tabs = new TabStrip([.. tabNames]) { Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 0) };

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0) };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 1));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(BuildHeader(), 0, 0);
        var headerDivider = Ui.Divider(DockStyle.Fill);
        headerDivider.Margin = new Padding(0);
        root.Controls.Add(headerDivider, 0, 1);

        var content = BuildContent();
        if (exercise.HasSubItems)
        {
            _nav = new NavList { Dock = DockStyle.Fill, Margin = new Padding(0) };
            foreach (var item in exercise.Items) _nav.Items.Add(new NavList.NavItem(item.Code, item.Title));
            _nav.SelectedIndexChanged += (_, _) =>
            {
                if (_nav.SelectedIndex >= 0) SelectItem(exercise.Items[_nav.SelectedIndex]);
            };

            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0) };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var navHost = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(0, 12, 0, 12), Margin = new Padding(0) };
            navHost.Controls.Add(_nav);
            body.Controls.Add(navHost, 0, 0);
            var navDivider = Ui.Divider(DockStyle.Fill);
            navDivider.Margin = new Padding(0);
            body.Controls.Add(navDivider, 1, 0);
            body.Controls.Add(content, 2, 0);
            root.Controls.Add(body, 0, 2);
        }
        else
        {
            root.Controls.Add(content, 0, 2);
        }

        Controls.Add(root);
        AcceptButton = _runButton;

        _connectButton.Click += async (_, _) => await ConnectAsync();
        _runButton.Click += async (_, _) => await RunAsync();
        _tabs.SelectedIndexChanged += (_, _) => ShowTab(_tabs.SelectedIndex);

        SetConnectionState(connected: false, "Chưa kết nối", Theme.TextDisabled);
        ShowTab(ResultTab);
        if (_nav is not null) _nav.SelectedIndex = 0; else SelectItem(_current);
    }

    // ---------------------------------------------------------------------
    // Dựng giao diện
    // ---------------------------------------------------------------------

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, RowCount = 1, Padding = new Padding(28, 18, 28, 18), Margin = new Padding(0) };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var titleStack = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0) };
        titleStack.Controls.Add(Ui.Label($"BÀI {_exercise.Number}  ·  CSDL {_exercise.Database}", Theme.SmallSemibold, Theme.TextMuted));
        var title = Ui.Label(_exercise.Title, Theme.Title);
        title.Margin = new Padding(0, 4, 0, 0);
        titleStack.Controls.Add(title);
        header.Controls.Add(titleStack, 0, 0);

        var actions = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Anchor = AnchorStyles.Right, Margin = new Padding(16, 0, 0, 0) };
        _connectionStatus.Margin = new Padding(0, 7, 16, 0);
        actions.Controls.Add(_connectionStatus);
        _connectButton.Margin = new Padding(0);
        actions.Controls.Add(_connectButton);
        header.Controls.Add(actions, 1, 0);
        return header;
    }

    private Control BuildContent()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, Padding = new Padding(28, 22, 28, 14), Margin = new Padding(0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddRow(Control control, bool fill = false)
        {
            layout.RowStyles.Add(fill ? new RowStyle(SizeType.Percent, 100) : new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(control, 0, layout.RowStyles.Count - 1);
        }

        _itemHeading.Margin = new Padding(0, 0, 0, 8);
        AddRow(_itemHeading);
        AddRow(_question);
        _note.Margin = new Padding(0, 8, 0, 0);
        AddRow(_note);
        AddRow(_parameterPanel);
        _hint.Margin = new Padding(0, 8, 0, 0);
        AddRow(_hint);

        _error.BackColor = Theme.DangerSoft;
        _error.Padding = new Padding(10, 8, 10, 8);
        _error.Margin = new Padding(0, 12, 0, 0);
        _error.Visible = false;
        AddRow(_error);

        AddRow(_tabs);
        AddRow(BuildTabPages(), fill: true);

        _resultStatus.Margin = new Padding(0, 10, 0, 0);
        AddRow(_resultStatus);
        return layout;
    }

    private Control BuildTabPages()
    {
        var host = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 12, 0, 0) };

        // Kết quả
        ConfigureGrid();
        var resultPage = new Panel { Dock = DockStyle.Fill };
        _emptyState.AutoSize = false;
        _emptyState.Dock = DockStyle.Fill;
        _emptyState.TextAlign = ContentAlignment.MiddleCenter;
        _emptyState.BackColor = Theme.Surface;
        resultPage.Controls.Add(_grid);
        resultPage.Controls.Add(_emptyState);
        _tabPages.Add(resultPage);

        // Mã nguồn SQL, code C#, lược đồ
        _tabPages.Add(WrapCode(_sourceView));
        _tabPages.Add(WrapCode(_callView));
        if (_exercise.Schema is not null)
        {
            var schemaView = Ui.CodeView();
            schemaView.Text = _exercise.Schema.Replace("\n", Environment.NewLine);
            _tabPages.Add(WrapCode(schemaView));
        }

        foreach (var page in _tabPages) host.Controls.Add(page);
        return host;
    }

    private static Control WrapCode(TextBox view)
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(14, 12, 6, 6) };
        panel.Controls.Add(view);
        return panel;
    }

    private void ConfigureGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.BorderStyle = BorderStyle.None;
        _grid.BackgroundColor = Theme.Background;
        _grid.GridColor = Theme.Border;
        _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _grid.EnableHeadersVisualStyles = false;
        _grid.RowHeadersVisible = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.ReadOnly = true;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = true;
        _grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        _grid.ColumnHeadersHeight = 38;
        _grid.RowTemplate.Height = 34;
        _grid.StandardTab = true;

        _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.Surface,
            ForeColor = Theme.TextMuted,
            SelectionBackColor = Theme.Surface,
            SelectionForeColor = Theme.TextMuted,
            Font = Theme.SmallSemibold,
            Padding = new Padding(8, 0, 8, 0),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            WrapMode = DataGridViewTriState.False
        };
        _grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.Background,
            ForeColor = Theme.Text,
            SelectionBackColor = Theme.AccentSoft,
            SelectionForeColor = Theme.Text,
            Font = Theme.Body,
            Padding = new Padding(8, 0, 8, 0),
            NullValue = "—"
        };

        _grid.DataBindingComplete += (_, _) => FormatGridColumns();
    }

    // ---------------------------------------------------------------------
    // Chọn câu
    // ---------------------------------------------------------------------

    private void SelectItem(ExerciseItem item)
    {
        SaveEnteredValues();
        _current = item;

        _itemHeading.Text = _exercise.HasSubItems ? $"Câu {item.Code}. {item.Title}" : "Câu hỏi";
        _question.Text = item.Question;

        var notes = new[] { _exercise.Context, item.Note }.Where(n => !string.IsNullOrWhiteSpace(n));
        _note.Text = string.Join(Environment.NewLine, notes);
        _note.Visible = _note.Text.Length > 0;

        _hint.Text = item.Hint ?? "";
        _hint.Visible = item.Hint is not null;

        BuildParameterInputs(item);
        HideError();
        ClearResult(_connected ? "Nhấn Thực hiện (Enter) để chạy câu này." : "Kết nối CSDL để thực hiện câu này.");
        UpdateCallCode();
        _ = LoadSourceAsync(item);
    }

    private void BuildParameterInputs(ExerciseItem item)
    {
        _parameterPanel.SuspendLayout();
        foreach (Control old in _parameterPanel.Controls.Cast<Control>().ToList())
        {
            old.Controls.Remove(_runButton); // Nút Thực hiện được dùng lại, không hủy
            _parameterPanel.Controls.Remove(old);
            old.Dispose();
        }
        _inputs.Clear();

        var visible = item.Parameters.Where(p => p.Visible).ToList();
        foreach (var parameter in visible)
        {
            var field = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 12, 0) };
            var caption = Ui.Label(parameter.Label, Theme.SmallSemibold, Theme.TextMuted);
            caption.Margin = new Padding(0, 0, 0, 6);
            var input = Ui.Input(parameter.Kind == ParameterKind.Text ? 170 : 110);
            input.AccessibleName = parameter.Label;
            input.Text = _enteredValues.GetValueOrDefault(Key(item, parameter), parameter.DefaultValue);
            input.TextChanged += (_, _) => { HideError(); UpdateCallCode(); };
            field.Controls.Add(caption);
            field.Controls.Add(input);
            _parameterPanel.Controls.Add(field);
            _inputs[parameter.Name] = input;
        }

        var runField = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0) };
        if (visible.Count > 0)
        {
            // Nhãn trống để nút thẳng hàng với các ô nhập
            var spacer = Ui.Label(" ", Theme.SmallSemibold);
            spacer.Margin = new Padding(0, 0, 0, 2);
            runField.Controls.Add(spacer);
        }
        _runButton.Margin = new Padding(0);
        runField.Controls.Add(_runButton);
        _parameterPanel.Controls.Add(runField);
        _parameterPanel.ResumeLayout();

        UpdateRunButton();
    }

    private static string Key(ExerciseItem item, ExerciseParameter parameter) => $"{item.Code}:{parameter.Name}";

    private void SaveEnteredValues()
    {
        foreach (var parameter in _current.Parameters.Where(p => p.Visible))
        {
            if (_inputs.TryGetValue(parameter.Name, out var input)) _enteredValues[Key(_current, parameter)] = input.Text;
        }
    }

    // ---------------------------------------------------------------------
    // Kết nối, thực thi
    // ---------------------------------------------------------------------

    private async Task ConnectAsync()
    {
        _connectButton.Enabled = false;
        SetConnectionState(connected: false, "Đang kết nối...", Theme.TextDisabled);
        HideError();
        UseWaitCursor = true;
        try
        {
            await DatabaseService.TestConnectionAsync(_settings, _exercise.Database, _lifetime.Token);
            SetConnectionState(connected: true, $"Đã kết nối · {_settings.DisplayName}", Theme.Success);
            ClearResult("Nhấn Thực hiện (Enter) để chạy câu này.");
            _sourceCache.Clear();
            _runButton.Focus();
            await LoadSourceAsync(_current);
        }
        catch (DatabaseException ex)
        {
            SetConnectionState(connected: false, "Kết nối thất bại", Theme.Danger);
            ShowError(ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Form đã đóng.
        }
        finally
        {
            if (!IsDisposed)
            {
                UseWaitCursor = false;
                _connectButton.Enabled = true;
            }
        }
    }

    private void SetConnectionState(bool connected, string text, Color dotColor)
    {
        _connected = connected;
        _connectionStatus.SetState(text, dotColor);
        _connectButton.Text = connected ? "Kết nối lại" : "Kết nối CSDL";
        UpdateRunButton();
    }

    private void UpdateRunButton() => _runButton.Enabled = _connected && !_busy;

    private async Task RunAsync()
    {
        if (!_connected || _busy) return;

        var item = _current;
        if (!TryReadArguments(item, out var arguments)) return;

        _busy = true;
        UpdateRunButton();
        HideError();
        _resultStatus.Text = "Đang thực hiện...";
        UseWaitCursor = true;
        try
        {
            var result = await DatabaseService.ExecuteRoutineAsync(_settings, _exercise.Database, item.Routine, arguments, _lifetime.Token);
            if (!ReferenceEquals(item, _current)) return; // Người dùng đã chuyển sang câu khác

            ShowResult(result);
            _tabs.SelectedIndex = ResultTab;
        }
        catch (DatabaseException ex)
        {
            if (!ReferenceEquals(item, _current)) return;
            ClearResult("Không có kết quả.");
            ShowError(ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Form đã đóng.
        }
        finally
        {
            _busy = false;
            if (!IsDisposed)
            {
                UseWaitCursor = false;
                UpdateRunButton();
            }
        }
    }

    private bool TryReadArguments(ExerciseItem item, out List<KeyValuePair<string, object?>> arguments)
    {
        arguments = [];
        foreach (var parameter in item.Parameters)
        {
            var raw = parameter.Visible && _inputs.TryGetValue(parameter.Name, out var input) ? input.Text.Trim() : parameter.DefaultValue;
            if (!TryParse(parameter, raw, out var value, out var message))
            {
                ShowError(message);
                if (_inputs.TryGetValue(parameter.Name, out var invalid))
                {
                    invalid.Focus();
                    invalid.SelectAll();
                }
                return false;
            }
            arguments.Add(new(parameter.Name, value));
        }
        return true;
    }

    private static bool TryParse(ExerciseParameter parameter, string raw, out object? value, out string message)
    {
        value = null;
        message = "";
        if (raw.Length == 0)
        {
            message = $"Vui lòng nhập {parameter.Label.ToLowerInvariant()}.";
            return false;
        }

        switch (parameter.Kind)
        {
            case ParameterKind.Number:
                // Chấp nhận cả dấu phẩy và dấu chấm làm dấu thập phân
                if (double.TryParse(raw.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number))
                {
                    value = number;
                    return true;
                }
                message = $"{parameter.Label} phải là một số thực, ví dụ 2 hoặc -1,5.";
                return false;

            case ParameterKind.Integer:
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
                {
                    value = integer;
                    return true;
                }
                message = $"{parameter.Label} phải là một số nguyên.";
                return false;

            default:
                value = raw;
                return true;
        }
    }

    // ---------------------------------------------------------------------
    // Hiển thị kết quả, mã nguồn
    // ---------------------------------------------------------------------

    private void ShowResult(QueryResult result)
    {
        _grid.DataSource = null;
        _grid.Columns.Clear();
        _grid.DataSource = result.Table;

        var rows = result.Table.Rows.Count;
        _emptyState.Text = "Thủ tục không trả về dòng nào.";
        _emptyState.Visible = rows == 0;
        _grid.Visible = rows > 0;

        _resultStatus.Text = $"{rows} dòng  ·  {result.Elapsed.TotalMilliseconds:0} ms  ·  {DateTime.Now:HH:mm:ss}";
    }

    private void ClearResult(string message)
    {
        _grid.DataSource = null;
        _grid.Columns.Clear();
        _grid.Visible = false;
        _emptyState.Text = message;
        _emptyState.Visible = true;
        _resultStatus.Text = "";
    }

    private void FormatGridColumns()
    {
        if (_grid.DataSource is not DataTable table) return;

        foreach (DataGridViewColumn column in _grid.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            var type = table.Columns[column.DataPropertyName]?.DataType;
            if (type == typeof(DateTime))
            {
                column.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            {
                column.DefaultCellStyle.Format = "#,##0.####";
                AlignRight(column);
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(ulong) || type == typeof(short))
            {
                AlignRight(column);
            }
        }

        // Cột vừa nội dung; nếu còn trống thì cột cuối giãn ra lấp đầy.
        _grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        var total = _grid.Columns.Cast<DataGridViewColumn>().Sum(c => c.Width);
        if (_grid.Columns.Count > 0 && total < _grid.ClientSize.Width)
        {
            _grid.Columns[^1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }

    private static void AlignRight(DataGridViewColumn column)
    {
        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
    }

    private async Task LoadSourceAsync(ExerciseItem item)
    {
        if (!_connected)
        {
            _sourceView.Text = "-- Kết nối CSDL để xem mã nguồn hàm / thủ tục / trigger của câu này.";
            return;
        }
        if (_sourceCache.TryGetValue(item.Code, out var cached))
        {
            _sourceView.Text = cached;
            return;
        }

        _sourceView.Text = "-- Đang tải mã nguồn...";
        var builder = new StringBuilder();
        try
        {
            foreach (var source in item.Sources)
            {
                var definition = await DatabaseService.GetDefinitionAsync(_settings, _exercise.Database, source, _lifetime.Token);
                builder.AppendLine($"-- {DescribeType(source.Type)} {source.Name}");
                builder.AppendLine("DELIMITER $$");
                builder.AppendLine(NormalizeNewLines(definition).TrimEnd() + "$$");
                builder.AppendLine("DELIMITER ;");
                builder.AppendLine();
            }
            var text = builder.ToString().TrimEnd();
            _sourceCache[item.Code] = text;
            if (ReferenceEquals(item, _current)) _sourceView.Text = text;
        }
        catch (DatabaseException ex)
        {
            if (ReferenceEquals(item, _current)) _sourceView.Text = $"-- {ex.Message}";
        }
        catch (OperationCanceledException)
        {
            // Form đã đóng.
        }
    }

    private static string DescribeType(SqlObjectType type) => type switch
    {
        SqlObjectType.Function => "Hàm",
        SqlObjectType.Trigger => "Trigger",
        _ => "Thủ tục"
    };

    private static string NormalizeNewLines(string text) => text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

    private void UpdateCallCode()
    {
        var item = _current;
        var values = item.Parameters.Select(p =>
        {
            var raw = p.Visible && _inputs.TryGetValue(p.Name, out var input) ? input.Text.Trim() : p.DefaultValue;
            if (raw.Length == 0) return (p.Name, CSharp: "null", Sql: "NULL");
            return p.Kind == ParameterKind.Text
                ? (p.Name, CSharp: $"\"{raw.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"", Sql: $"'{raw.Replace("'", "''")}'")
                : (p.Name, CSharp: raw.Replace(',', '.'), Sql: raw.Replace(',', '.'));
        }).ToList();

        var code = new StringBuilder();
        code.AppendLine("// 1. Code kết nối CSDL (thư viện MySqlConnector)");
        code.AppendLine("var builder = new MySqlConnectionStringBuilder");
        code.AppendLine("{");
        code.AppendLine($"    Server = \"{_settings.Host}\",");
        code.AppendLine($"    Port = {_settings.Port},");
        code.AppendLine($"    UserID = \"{_settings.User}\",");
        code.AppendLine("    Password = \"********\",");
        code.AppendLine($"    Database = \"{_exercise.Database}\",");
        code.AppendLine("    CharacterSet = \"utf8mb4\"");
        code.AppendLine("};");
        code.AppendLine("await using var connection = new MySqlConnection(builder.ConnectionString);");
        code.AppendLine("await connection.OpenAsync();");
        code.AppendLine();
        code.AppendLine("// 2. Code lời gọi thủ tục");
        code.AppendLine($"await using var command = new MySqlCommand(\"{item.Routine}\", connection)");
        code.AppendLine("{");
        code.AppendLine("    CommandType = CommandType.StoredProcedure");
        code.AppendLine("};");
        foreach (var value in values)
        {
            code.AppendLine($"command.Parameters.AddWithValue(\"{value.Name}\", {value.CSharp});");
        }
        code.AppendLine();
        code.AppendLine("// 3. Đọc kết quả trả về");
        code.AppendLine("await using var reader = await command.ExecuteReaderAsync();");
        code.AppendLine("var result = new DataTable();");
        code.AppendLine("result.Load(reader);");
        code.AppendLine();
        code.AppendLine("/* Lệnh SQL tương đương:");
        code.AppendLine($"   CALL {item.Routine}({string.Join(", ", values.Select(v => v.Sql))}); */");
        _callView.Text = NormalizeNewLines(code.ToString());
    }

    // ---------------------------------------------------------------------
    // Tiện ích
    // ---------------------------------------------------------------------

    private void ShowTab(int index)
    {
        for (var i = 0; i < _tabPages.Count; i++) _tabPages[i].Visible = i == index;
        if (index == SourceTab && _connected) _ = LoadSourceAsync(_current);
    }

    private void ShowError(string message)
    {
        _error.Text = message;
        _error.Visible = true;
    }

    private void HideError() => _error.Visible = false;

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _connectButton.Focus();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _lifetime.Cancel();
        base.OnFormClosed(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _lifetime.Dispose();
        base.Dispose(disposing);
    }
}
