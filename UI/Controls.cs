using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace BTLWinForms.UI;

internal enum ButtonVariant { Primary, Secondary }

/// <summary>Nút phẳng tối giản, tự đổi màu khi bị vô hiệu hóa hoặc rê chuột.</summary>
internal sealed class FlatButton : Button
{
    private readonly ButtonVariant _variant;

    public FlatButton(string text, ButtonVariant variant = ButtonVariant.Secondary)
    {
        _variant = variant;
        Text = text;
        FlatStyle = FlatStyle.Flat;
        Font = Theme.BodySemibold;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        MinimumSize = new Size(0, 34);
        Padding = new Padding(12, 0, 12, 0);
        Cursor = Cursors.Hand;
        UseVisualStyleBackColor = false;
        FlatAppearance.BorderSize = variant == ButtonVariant.Primary ? 0 : 1;
        ApplyColors();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        ApplyColors();
    }

    private void ApplyColors()
    {
        if (_variant == ButtonVariant.Primary)
        {
            BackColor = Enabled ? Theme.Accent : Theme.Border;
            ForeColor = Enabled ? Color.White : Theme.TextDisabled;
            FlatAppearance.MouseOverBackColor = Theme.AccentHover;
            FlatAppearance.MouseDownBackColor = Theme.AccentHover;
        }
        else
        {
            BackColor = Theme.Background;
            ForeColor = Enabled ? Theme.Text : Theme.TextDisabled;
            FlatAppearance.BorderColor = Theme.BorderStrong;
            FlatAppearance.MouseOverBackColor = Theme.SurfaceHover;
            FlatAppearance.MouseDownBackColor = Theme.Border;
        }
        Cursor = Enabled ? Cursors.Hand : Cursors.Default;
    }

    // Tắt khung focus nét đứt mặc định; trạng thái focus được thể hiện bằng viền.
    protected override bool ShowFocusCues => false;

    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);
        if (Focused && Enabled)
        {
            using var pen = new Pen(_variant == ButtonVariant.Primary ? Theme.AccentHover : Theme.Accent, 2);
            pevent.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
        }
    }
}

/// <summary>Chấm tròn trạng thái + nhãn (Chưa kết nối / Đang kết nối / Đã kết nối / Lỗi).</summary>
internal sealed class StatusIndicator : Control
{
    private Color _dotColor = Theme.TextDisabled;

    public StatusIndicator()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
        Font = Theme.Small;
        ForeColor = Theme.TextMuted;
        BackColor = Color.Transparent;
        Margin = new Padding(0, 0, 12, 0);
    }

    public void SetState(string text, Color dotColor)
    {
        Text = text;
        _dotColor = dotColor;
        AccessibleName = text;
        Size = GetPreferredSize(Size.Empty);
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var textSize = TextRenderer.MeasureText(Text, Font);
        return new Size(textSize.Width + LogicalToDeviceUnits(16), Math.Max(textSize.Height, LogicalToDeviceUnits(20)));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var dot = LogicalToDeviceUnits(8);
        using (var brush = new SolidBrush(_dotColor))
        {
            e.Graphics.FillEllipse(brush, 0, (Height - dot) / 2f, dot, dot);
        }
        var textRect = new Rectangle(LogicalToDeviceUnits(14), 0, Width - LogicalToDeviceUnits(14), Height);
        TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding);
    }
}

/// <summary>Thanh tab dạng gạch chân, hỗ trợ bàn phím (mũi tên trái/phải).</summary>
internal sealed class TabStrip : Control
{
    private readonly List<string> _tabs = [];
    private readonly List<Rectangle> _bounds = [];
    private int _selectedIndex;
    private int _hoverIndex = -1;

    public event EventHandler? SelectedIndexChanged;

    public TabStrip(params string[] tabs)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.Selectable, true);
        _tabs.AddRange(tabs);
        Font = Theme.BodySemibold;
        BackColor = Theme.Background;
        TabStop = true;
        Height = 38;
        AccessibleRole = AccessibleRole.PageTabList;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < 0 || value >= _tabs.Count || value == _selectedIndex) return;
            _selectedIndex = value;
            Invalidate();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void LayoutTabs()
    {
        _bounds.Clear();
        var x = 0;
        var gap = LogicalToDeviceUnits(24);
        foreach (var tab in _tabs)
        {
            var width = TextRenderer.MeasureText(tab, Font).Width;
            _bounds.Add(new Rectangle(x, 0, width, Height));
            x += width + gap;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        LayoutTabs();
        using (var border = new Pen(Theme.Border))
        {
            e.Graphics.DrawLine(border, 0, Height - 1, Width, Height - 1);
        }

        for (var i = 0; i < _tabs.Count; i++)
        {
            var selected = i == _selectedIndex;
            var color = selected ? Theme.Text : i == _hoverIndex ? Theme.Text : Theme.TextMuted;
            TextRenderer.DrawText(e.Graphics, _tabs[i], Font, _bounds[i], color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            if (selected)
            {
                var thickness = LogicalToDeviceUnits(2);
                using var brush = new SolidBrush(Focused ? Theme.Accent : Theme.Text);
                e.Graphics.FillRectangle(brush, _bounds[i].X, Height - thickness, _bounds[i].Width, thickness);
            }
        }
    }

    private int HitTest(Point point)
    {
        LayoutTabs();
        return _bounds.FindIndex(r => r.Contains(point));
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var index = HitTest(e.Location);
        Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
        if (index != _hoverIndex)
        {
            _hoverIndex = index;
            Invalidate();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hoverIndex = -1;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        var index = HitTest(e.Location);
        if (index >= 0) SelectedIndex = index;
    }

    protected override bool IsInputKey(Keys keyData) => keyData is Keys.Left or Keys.Right || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Left) SelectedIndex = Math.Max(0, _selectedIndex - 1);
        if (e.KeyCode == Keys.Right) SelectedIndex = Math.Min(_tabs.Count - 1, _selectedIndex + 1);
    }

    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
}

/// <summary>Danh sách câu nhỏ của một bài: mã câu + tiêu đề, mục đang chọn có vạch nhấn bên trái.</summary>
internal sealed class NavList : ListBox
{
    public NavList()
    {
        DrawMode = DrawMode.OwnerDrawFixed;
        BorderStyle = BorderStyle.None;
        BackColor = Theme.Surface;
        Font = Theme.Body;
        IntegralHeight = false;
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        UpdateItemHeight();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateItemHeight();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateItemHeight();
    }

    private void UpdateItemHeight() => ItemHeight = Math.Min(255, Font.Height * 2 + LogicalToDeviceUnits(18));

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0 || Items[e.Index] is not NavItem item) return;

        var selected = (e.State & DrawItemState.Selected) != 0;
        using (var background = new SolidBrush(selected ? Theme.Background : Theme.Surface))
        {
            e.Graphics.FillRectangle(background, e.Bounds);
        }
        if (selected)
        {
            using var accent = new SolidBrush(Theme.Accent);
            e.Graphics.FillRectangle(accent, e.Bounds.X, e.Bounds.Y, LogicalToDeviceUnits(3), e.Bounds.Height);
        }

        var padX = LogicalToDeviceUnits(18);
        var padY = LogicalToDeviceUnits(9);
        var codeRect = new Rectangle(e.Bounds.X + padX, e.Bounds.Y + padY, e.Bounds.Width - padX * 2, Font.Height);
        var titleRect = codeRect with { Y = codeRect.Bottom };
        TextRenderer.DrawText(e.Graphics, item.Code, Theme.SmallSemibold, codeRect,
            selected ? Theme.Accent : Theme.TextMuted, TextFormatFlags.Left | TextFormatFlags.NoPadding);
        TextRenderer.DrawText(e.Graphics, item.Title, selected ? Theme.BodySemibold : Theme.Body, titleRect,
            Theme.Text, TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);

        if ((e.State & DrawItemState.Focus) != 0 && (e.State & DrawItemState.NoFocusRect) == 0)
        {
            using var pen = new Pen(Theme.BorderStrong) { DashStyle = DashStyle.Dot };
            e.Graphics.DrawRectangle(pen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 3, e.Bounds.Height - 3);
        }
    }

    public sealed record NavItem(string Code, string Title)
    {
        public override string ToString() => $"{Code} {Title}";
    }
}

/// <summary>Ô chọn bài tập trên màn hình chính. Nhấn chuột, Enter hoặc Space để mở bài.</summary>
internal sealed class ExerciseTile : Control
{
    private bool _hover;
    private bool _pressed;

    public string Caption { get; }
    public string Heading { get; }
    public string Footer { get; }

    public ExerciseTile(string caption, string heading, string footer)
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint | ControlStyles.Selectable | ControlStyles.StandardClick, true);
        Caption = caption;
        Heading = heading;
        Footer = footer;
        Text = $"{caption}: {heading}";
        AccessibleName = Text;
        AccessibleDescription = footer;
        AccessibleRole = AccessibleRole.PushButton;
        TabStop = true;
        Cursor = Cursors.Hand;
        BackColor = Theme.Background;
        Margin = new Padding(0, 0, 12, 12);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.Background);

        var radius = LogicalToDeviceUnits(8);
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedRectangle(rect, radius);
        using (var fill = new SolidBrush(_pressed ? Theme.SurfaceHover : _hover ? Theme.Surface : Theme.Background))
        {
            g.FillPath(fill, path);
        }
        var active = Focused || _hover;
        using (var pen = new Pen(Focused ? Theme.Accent : active ? Theme.BorderStrong : Theme.Border, Focused ? 2 : 1))
        {
            g.DrawPath(pen, path);
        }

        var pad = LogicalToDeviceUnits(18);
        var inner = new Rectangle(pad, pad, Width - pad * 2, Height - pad * 2);

        var captionHeight = Theme.SmallSemibold.Height;
        TextRenderer.DrawText(g, Caption, Theme.SmallSemibold, new Rectangle(inner.X, inner.Y, inner.Width, captionHeight),
            Theme.Accent, TextFormatFlags.Left | TextFormatFlags.NoPadding);

        var headingRect = new Rectangle(inner.X, inner.Y + captionHeight + LogicalToDeviceUnits(6), inner.Width,
            inner.Height - captionHeight * 2 - LogicalToDeviceUnits(10));
        TextRenderer.DrawText(g, Heading, Theme.Heading, headingRect, Theme.Text,
            TextFormatFlags.Left | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);

        var footerRect = new Rectangle(inner.X, inner.Bottom - Theme.Small.Height, inner.Width, Theme.Small.Height);
        TextRenderer.DrawText(g, Footer, Theme.Small, footerRect, Theme.TextMuted,
            TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var diameter = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; _pressed = false; Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); Focus(); _pressed = true; Invalidate(); }
    protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); _pressed = false; Invalidate(); }
    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode is Keys.Enter or Keys.Space)
        {
            e.Handled = true;
            OnClick(EventArgs.Empty);
        }
    }

    protected override bool IsInputKey(Keys keyData) => keyData is Keys.Enter or Keys.Space || base.IsInputKey(keyData);
}

internal static class Ui
{
    public static Label Label(string text, Font? font = null, Color? color = null) => new()
    {
        Text = text,
        Font = font ?? Theme.Body,
        ForeColor = color ?? Theme.Text,
        AutoSize = true,
        Margin = new Padding(0),
        UseMnemonic = false
    };

    /// <summary>Nhãn tự xuống dòng theo chiều rộng của cột chứa nó (dùng trong TableLayoutPanel).</summary>
    public static Label WrappingLabel(string text, Font? font = null, Color? color = null)
    {
        var label = Label(text, font, color);
        label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        return label;
    }

    public static Panel Divider(DockStyle dock = DockStyle.Top) => new()
    {
        Dock = dock,
        Height = 1,
        Width = 1,
        BackColor = Theme.Border
    };

    public static TextBox Input(int width = 160) => new()
    {
        Width = width,
        Font = Theme.Body,
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new Padding(0)
    };

    public static TextBox CodeView() => new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ReadOnly = true,
        WordWrap = false,
        ScrollBars = ScrollBars.Both,
        BorderStyle = BorderStyle.None,
        BackColor = Theme.Surface,
        ForeColor = Theme.Text,
        Font = Theme.Mono,
        TabStop = true
    };
}
