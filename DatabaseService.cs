using System.Data;
using MySqlConnector;

namespace BTLWinForms;

public static class DatabaseService
{
    public static void TestConnection(string connectionString, string? database = null)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        if (!string.IsNullOrWhiteSpace(database)) builder.Database = database;
        using var connection = new MySqlConnection(builder.ConnectionString);
        try
        {
            connection.Open();
        }
        catch (MySqlException ex)
        {
            throw new Exception(FormatMySqlErrorMessage(ex, builder.Database));
        }
    }

    public static DataTable ExecuteRoutine(string connectionString, string routine, (string Name, object Value)[] parameters, string? database = null)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        if (!string.IsNullOrWhiteSpace(database)) builder.Database = database;

        try
        {
            using var connection = new MySqlConnection(builder.ConnectionString);
            using var command = new MySqlCommand(routine, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };

            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Name, parameter.Value ?? DBNull.Value);
            }

            using var adapter = new MySqlDataAdapter(command);
            var result = new DataTable();
            adapter.Fill(result);
            return result;
        }
        catch (MySqlException ex)
        {
            throw new Exception(FormatMySqlErrorMessage(ex, builder.Database, routine));
        }
    }

    private static string FormatMySqlErrorMessage(MySqlException ex, string? database, string? routine = null)
    {
        return ex.Number switch
        {
            1042 => "Không thể kết nối đến máy chủ MySQL. Vui lòng kiểm tra MySQL Server đã khởi động và đúng cổng (3306).",
            1045 => "Đăng nhập MySQL thất bại. Sai tên người dùng hoặc mật khẩu.",
            1049 => $"Cơ sở dữ liệu '{database}' chưa tồn tại. Vui lòng chạy file BTL_Install.sql để tạo CSDL.",
            1305 => $"Không tìm thấy thủ tục '{routine ?? "yêu cầu"}' trong CSDL '{database}'. Vui lòng chạy lại file BTL_Install.sql.",
            _ => $"Lỗi MySQL ({ex.Number}): {ex.Message}"
        };
    }
}