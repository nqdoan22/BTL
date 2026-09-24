namespace BTLWinForms.UI;

/// <summary>
/// Bảng màu, font và kích thước dùng chung cho toàn bộ giao diện.
/// </summary>
internal static class Theme
{
    public static readonly Color Background = Color.White;
    public static readonly Color Surface = Color.FromArgb(248, 248, 249);
    public static readonly Color SurfaceHover = Color.FromArgb(242, 243, 245);
    public static readonly Color Border = Color.FromArgb(228, 228, 231);
    public static readonly Color BorderStrong = Color.FromArgb(212, 212, 216);
    public static readonly Color Text = Color.FromArgb(24, 24, 27);
    public static readonly Color TextMuted = Color.FromArgb(113, 113, 122);
    public static readonly Color TextDisabled = Color.FromArgb(161, 161, 170);
    public static readonly Color Accent = Color.FromArgb(37, 99, 235);
    public static readonly Color AccentHover = Color.FromArgb(29, 78, 216);
    public static readonly Color AccentSoft = Color.FromArgb(239, 244, 255);
    public static readonly Color Success = Color.FromArgb(22, 163, 74);
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);
    public static readonly Color DangerSoft = Color.FromArgb(254, 242, 242);

    private const string UiFontFamily = "Segoe UI";

    public static readonly Font Body = new(UiFontFamily, 9.75F);
    public static readonly Font BodySemibold = new(UiFontFamily + " Semibold", 9.75F);
    public static readonly Font Small = new(UiFontFamily, 8.75F);
    public static readonly Font SmallSemibold = new(UiFontFamily + " Semibold", 8.75F);
    public static readonly Font Heading = new(UiFontFamily + " Semibold", 12F);
    public static readonly Font Title = new(UiFontFamily + " Semibold", 16F);
    public static readonly Font Mono = CreateMonoFont();

    private static Font CreateMonoFont()
    {
        foreach (var family in new[] { "Cascadia Mono", "Consolas" })
        {
            using var probe = new Font(family, 9.5F);
            if (probe.Name == family) return new Font(family, 9.5F);
        }
        return new Font(FontFamily.GenericMonospace, 9.5F);
    }
}
