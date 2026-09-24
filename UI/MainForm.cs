using BTLWinForms.Data;
using BTLWinForms.Exercises;

namespace BTLWinForms.UI;

/// <summary>Giao diện chính: mỗi nút mở giao diện con của một bài tập.</summary>
internal sealed class MainForm : Form
{
    private readonly ConnectionSettings _settings;
    private readonly Label _connectionLabel;

    public MainForm(ConnectionSettings settings)
    {
        _settings = settings;

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = "Bài tập lớn Hệ cơ sở dữ liệu";
        Font = Theme.Body;
        BackColor = Theme.Background;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(940, 600);
        MinimumSize = new Size(720, 520);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(32, 28, 32, 16)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Header: tiêu đề + thông tin kết nối
        var header = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var titleStack = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0) };
        titleStack.Controls.Add(Ui.Label("Bài tập lớn Hệ cơ sở dữ liệu", Theme.Title));
        var subtitle = Ui.Label("Chọn một bài tập để thực hiện. Mỗi bài có nút kết nối đến CSDL của bài trước khi chạy.", Theme.Body, Theme.TextMuted);
        subtitle.Margin = new Padding(0, 6, 0, 0);
        titleStack.Controls.Add(subtitle);
        header.Controls.Add(titleStack, 0, 0);

        var connectionStack = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Margin = new Padding(16, 4, 0, 0), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _connectionLabel = Ui.Label("", Theme.Small, Theme.TextMuted);
        _connectionLabel.Margin = new Padding(0, 9, 12, 0);
        connectionStack.Controls.Add(_connectionLabel);
        var settingsButton = new FlatButton("Cài đặt kết nối");
        settingsButton.Click += (_, _) => OpenConnectionSettings();
        connectionStack.Controls.Add(settingsButton);
        header.Controls.Add(connectionStack, 1, 0);
        root.Controls.Add(header, 0, 0);

        var divider = Ui.Divider();
        divider.Dock = DockStyle.Fill;
        divider.Margin = new Padding(0, 20, 0, 20);
        root.Controls.Add(divider, 0, 1);

        // Lưới các bài tập
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Margin = new Padding(0) };
        for (var i = 0; i < 4; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        foreach (var exercise in ExerciseCatalog.All)
        {
            var footer = exercise.HasSubItems
                ? $"{exercise.Items.Count} câu · {exercise.Database}"
                : exercise.Database;
            var tile = new ExerciseTile($"Bài {exercise.Number}", exercise.Title, footer) { Dock = DockStyle.Fill };
            tile.Click += (_, _) => OpenExercise(exercise);
            grid.Controls.Add(tile);
        }
        root.Controls.Add(grid, 0, 2);

        var footerLabel = Ui.Label("Cài đặt CSDL: chạy Database/BTL_Install.sql trên MySQL 8.0 trở lên.", Theme.Small, Theme.TextMuted);
        footerLabel.Margin = new Padding(0, 4, 0, 0);
        root.Controls.Add(footerLabel, 0, 3);

        Controls.Add(root);
        UpdateConnectionLabel();
    }

    private void UpdateConnectionLabel() => _connectionLabel.Text = $"Máy chủ: {_settings.DisplayName}";

    private void OpenConnectionSettings()
    {
        using var dialog = new ConnectionDialog(_settings);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            UpdateConnectionLabel();
        }
    }

    private void OpenExercise(Exercise exercise)
    {
        using var form = new ExerciseForm(exercise, _settings);
        form.ShowDialog(this);
    }
}
