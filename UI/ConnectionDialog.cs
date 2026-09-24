using BTLWinForms.Data;

namespace BTLWinForms.UI;

/// <summary>Hộp thoại cài đặt thông tin đăng nhập máy chủ MySQL.</summary>
internal sealed class ConnectionDialog : Form
{
    private readonly ConnectionSettings _settings;
    private readonly TextBox _host = Ui.Input(260);
    private readonly NumericUpDown _port = new() { Minimum = 1, Maximum = 65535, Width = 100, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0) };
    private readonly TextBox _user = Ui.Input(260);
    private readonly TextBox _password = Ui.Input(260);
    private readonly CheckBox _remember = new() { Text = "Ghi nhớ mật khẩu trên máy này", AutoSize = true, Margin = new Padding(0, 10, 0, 0) };
    private readonly StatusIndicator _status = new();
    private readonly FlatButton _testButton = new("Kiểm tra kết nối");
    private readonly FlatButton _saveButton = new("Lưu", ButtonVariant.Primary);
    private CancellationTokenSource? _cts;

    public ConnectionDialog(ConnectionSettings settings)
    {
        _settings = settings;

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = "Cài đặt kết nối MySQL";
        Font = Theme.Body;
        BackColor = Theme.Background;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        _password.UseSystemPasswordChar = true;
        _host.Text = settings.Host;
        _port.Value = settings.Port;
        _user.Text = settings.User;
        _password.Text = settings.Password;
        _remember.Checked = settings.RememberPassword;
        _status.SetState("Chưa kiểm tra", Theme.TextDisabled);

        var layout = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Padding = new Padding(24, 20, 24, 20) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        AddRow(layout, "Máy chủ", _host);
        AddRow(layout, "Cổng", _port);
        AddRow(layout, "Tài khoản", _user);
        AddRow(layout, "Mật khẩu", _password);
        layout.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0) });
        layout.Controls.Add(_remember);

        _status.Margin = new Padding(0, 16, 0, 0);
        layout.Controls.Add(_status);
        layout.SetColumnSpan(_status, 2);

        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 0) };
        var cancelButton = new FlatButton("Hủy") { DialogResult = DialogResult.Cancel, Margin = new Padding(8, 0, 0, 0) };
        _saveButton.Margin = new Padding(8, 0, 0, 0);
        buttons.Controls.Add(_saveButton);
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(_testButton);
        layout.Controls.Add(buttons);
        layout.SetColumnSpan(buttons, 2);

        Controls.Add(layout);
        AcceptButton = _saveButton;
        CancelButton = cancelButton;

        _testButton.Click += async (_, _) => await TestAsync();
        _saveButton.Click += (_, _) => Save();
    }

    private static void AddRow(TableLayoutPanel layout, string label, Control input)
    {
        var caption = Ui.Label(label, Theme.Body, Theme.TextMuted);
        caption.Anchor = AnchorStyles.Left;
        caption.Margin = new Padding(0, 0, 16, 10);
        input.Margin = new Padding(0, 0, 0, 10);
        layout.Controls.Add(caption);
        layout.Controls.Add(input);
    }

    private ConnectionSettings ReadForm() => new()
    {
        Host = _host.Text.Trim(),
        Port = (int)_port.Value,
        User = _user.Text.Trim(),
        Password = _password.Text,
        RememberPassword = _remember.Checked
    };

    private bool ValidateInput(ConnectionSettings candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate.Host)) { ShowError("Vui lòng nhập địa chỉ máy chủ."); _host.Focus(); return false; }
        if (string.IsNullOrWhiteSpace(candidate.User)) { ShowError("Vui lòng nhập tài khoản."); _user.Focus(); return false; }
        return true;
    }

    private void ShowError(string message)
    {
        _status.ForeColor = Theme.Danger;
        _status.SetState(message, Theme.Danger);
    }

    private async Task TestAsync()
    {
        var candidate = ReadForm();
        if (!ValidateInput(candidate)) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _testButton.Enabled = false;
        _status.ForeColor = Theme.TextMuted;
        _status.SetState("Đang kết nối...", Theme.TextDisabled);
        UseWaitCursor = true;
        try
        {
            await DatabaseService.TestConnectionAsync(candidate, null, _cts.Token);
            _status.ForeColor = Theme.Text;
            _status.SetState("Kết nối thành công", Theme.Success);
        }
        catch (DatabaseException ex)
        {
            ShowError(ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Hộp thoại đã đóng hoặc người dùng kiểm tra lại.
        }
        finally
        {
            UseWaitCursor = false;
            if (!IsDisposed) _testButton.Enabled = true;
        }
    }

    private void Save()
    {
        var candidate = ReadForm();
        if (!ValidateInput(candidate)) return;

        _settings.CopyFrom(candidate);
        try
        {
            _settings.Save();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Không lưu được cài đặt: {ex.Message}\nCài đặt vẫn được dùng cho phiên làm việc này.",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        DialogResult = DialogResult.OK;
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        base.OnFormClosed(e);
    }
}
