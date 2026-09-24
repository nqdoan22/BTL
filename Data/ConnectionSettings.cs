using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MySqlConnector;

namespace BTLWinForms.Data;

/// <summary>
/// Thông tin kết nối máy chủ MySQL. Được lưu tại %AppData%\BTLWinForms\settings.json;
/// mật khẩu (nếu người dùng chọn ghi nhớ) được mã hóa bằng Windows DPAPI theo tài khoản hiện tại.
/// </summary>
public sealed class ConnectionSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BTLWinForms", "settings.json");

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3306;
    public string User { get; set; } = "root";
    public string Password { get; set; } = "";
    public bool RememberPassword { get; set; }

    public string DisplayName => $"{User}@{Host}:{Port}";

    public string BuildConnectionString(string? database = null)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = Host,
            Port = (uint)Port,
            UserID = User,
            Password = Password,
            CharacterSet = "utf8mb4",
            ConnectionTimeout = 5,
            DefaultCommandTimeout = 30,
            AllowPublicKeyRetrieval = true,
            SslMode = MySqlSslMode.Preferred
        };
        if (!string.IsNullOrWhiteSpace(database)) builder.Database = database;
        return builder.ConnectionString;
    }

    public ConnectionSettings Clone() => (ConnectionSettings)MemberwiseClone();

    public void CopyFrom(ConnectionSettings other)
    {
        Host = other.Host;
        Port = other.Port;
        User = other.User;
        Password = other.Password;
        RememberPassword = other.RememberPassword;
    }

    public static ConnectionSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return new ConnectionSettings();
            var stored = JsonSerializer.Deserialize<StoredSettings>(File.ReadAllText(SettingsPath));
            if (stored is null) return new ConnectionSettings();

            return new ConnectionSettings
            {
                Host = string.IsNullOrWhiteSpace(stored.Host) ? "localhost" : stored.Host,
                Port = stored.Port is > 0 and <= 65535 ? stored.Port : 3306,
                User = stored.User ?? "root",
                RememberPassword = stored.ProtectedPassword is not null,
                Password = Unprotect(stored.ProtectedPassword)
            };
        }
        catch (Exception)
        {
            // File cấu hình hỏng hoặc không đọc được: dùng giá trị mặc định.
            return new ConnectionSettings();
        }
    }

    public void Save()
    {
        var stored = new StoredSettings
        {
            Host = Host,
            Port = Port,
            User = User,
            ProtectedPassword = RememberPassword ? Protect(Password) : null
        };
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(stored, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string Protect(string value)
    {
        var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(bytes);
    }

    private static string Unprotect(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        try
        {
            var bytes = ProtectedData.Unprotect(Convert.FromBase64String(value), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception)
        {
            return "";
        }
    }

    private sealed class StoredSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? User { get; set; }
        public string? ProtectedPassword { get; set; }
    }
}
