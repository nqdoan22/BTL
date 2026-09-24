using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using BTLWinForms.Exercises;
using MySqlConnector;

namespace BTLWinForms.Data;

/// <summary>Lỗi CSDL đã được chuyển thành thông báo dễ hiểu cho người dùng.</summary>
public sealed class DatabaseException(string message, Exception? inner = null) : Exception(message, inner);

public sealed record QueryResult(DataTable Table, TimeSpan Elapsed);

/// <summary>
/// Kết nối MySQL và thực thi thủ tục / đọc mã nguồn hàm, thủ tục, trigger.
/// Mọi thao tác đều bất đồng bộ để giao diện không bị treo.
/// </summary>
public static partial class DatabaseService
{
    public static async Task TestConnectionAsync(ConnectionSettings settings, string? database, CancellationToken ct = default)
    {
        try
        {
            await using var connection = new MySqlConnection(settings.BuildConnectionString(database));
            await connection.OpenAsync(ct);
            await using var command = new MySqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync(ct);
        }
        catch (MySqlException ex)
        {
            throw Translate(ex, database);
        }
    }

    public static async Task<QueryResult> ExecuteRoutineAsync(
        ConnectionSettings settings,
        string database,
        string routine,
        IReadOnlyList<KeyValuePair<string, object?>> arguments,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = new MySqlConnection(settings.BuildConnectionString(database));
            await connection.OpenAsync(ct);

            await using var command = new MySqlCommand(routine, connection) { CommandType = CommandType.StoredProcedure };
            foreach (var (name, value) in arguments)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            await using var reader = await command.ExecuteReaderAsync(ct);
            var table = new DataTable();
            table.Load(reader);
            return new QueryResult(table, stopwatch.Elapsed);
        }
        catch (MySqlException ex)
        {
            throw Translate(ex, database, routine);
        }
    }

    /// <summary>Đọc mã nguồn CREATE ... của một hàm, thủ tục hoặc trigger đang có trong CSDL.</summary>
    public static async Task<string> GetDefinitionAsync(ConnectionSettings settings, string database, SqlObject sqlObject, CancellationToken ct = default)
    {
        if (!IdentifierRegex().IsMatch(sqlObject.Name))
        {
            throw new ArgumentException($"Tên đối tượng không hợp lệ: {sqlObject.Name}", nameof(sqlObject));
        }

        var keyword = sqlObject.Type switch
        {
            SqlObjectType.Procedure => "PROCEDURE",
            SqlObjectType.Function => "FUNCTION",
            SqlObjectType.Trigger => "TRIGGER",
            _ => throw new ArgumentOutOfRangeException(nameof(sqlObject))
        };

        try
        {
            await using var connection = new MySqlConnection(settings.BuildConnectionString(database));
            await connection.OpenAsync(ct);
            await using var command = new MySqlCommand($"SHOW CREATE {keyword} `{sqlObject.Name}`", connection);
            await using var reader = await command.ExecuteReaderAsync(ct);

            // Cột thứ 3 là "Create Procedure" / "Create Function" / "SQL Original Statement"
            if (!await reader.ReadAsync(ct) || reader.IsDBNull(2))
            {
                return $"-- Không đọc được mã nguồn của {sqlObject.Name} (tài khoản thiếu quyền xem định nghĩa).";
            }
            return DefinerRegex().Replace(reader.GetString(2), "CREATE ");
        }
        catch (MySqlException ex)
        {
            throw Translate(ex, database, sqlObject.Name);
        }
    }

    private static DatabaseException Translate(MySqlException ex, string? database, string? routine = null)
    {
        var message = ex.ErrorCode switch
        {
            MySqlErrorCode.UnableToConnectToHost =>
                "Không thể kết nối đến máy chủ MySQL. Hãy kiểm tra MySQL Server đã chạy và địa chỉ, cổng kết nối.",
            MySqlErrorCode.AccessDenied =>
                "Đăng nhập MySQL thất bại: sai tên người dùng hoặc mật khẩu.",
            MySqlErrorCode.UnknownDatabase =>
                $"CSDL '{database}' chưa tồn tại. Hãy chạy file Database/BTL_Install.sql để tạo CSDL.",
            MySqlErrorCode.StoredProcedureDoesNotExist or MySqlErrorCode.TriggerDoesNotExist =>
                $"Không tìm thấy '{routine}' trong CSDL '{database}'. Hãy chạy lại file Database/BTL_Install.sql.",
            _ when ex.Number == 1644 => ex.Message,
            _ => $"Lỗi MySQL ({ex.Number}): {ex.Message}"
        };
        return new DatabaseException(message, ex);
    }

    [GeneratedRegex("^[A-Za-z0-9_]+$")]
    private static partial Regex IdentifierRegex();

    [GeneratedRegex(@"^CREATE\s+DEFINER\s*=\s*\S+\s+", RegexOptions.IgnoreCase)]
    private static partial Regex DefinerRegex();
}
