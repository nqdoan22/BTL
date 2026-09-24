namespace BTLWinForms.Exercises;

public enum ParameterKind { Text, Number, Integer }

/// <summary>Tham số truyền vào thủ tục. Tham số ẩn (Visible = false) luôn dùng giá trị mặc định.</summary>
public sealed record ExerciseParameter(
    string Name,
    string Label,
    ParameterKind Kind,
    string DefaultValue,
    bool Visible = true);

public enum SqlObjectType { Procedure, Function, Trigger }

public sealed record SqlObject(SqlObjectType Type, string Name);

/// <summary>Một câu (hoặc một bài không có câu nhỏ): câu hỏi, thủ tục cần gọi và mã nguồn liên quan.</summary>
public sealed class ExerciseItem
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Question { get; init; }
    public required string Routine { get; init; }
    public IReadOnlyList<ExerciseParameter> Parameters { get; init; } = [];
    public required IReadOnlyList<SqlObject> Sources { get; init; }
    /// <summary>Gợi ý giá trị mẫu có trong dữ liệu.</summary>
    public string? Hint { get; init; }
    /// <summary>Ghi chú cách hiểu đề hoặc cách cài đặt trên MySQL.</summary>
    public string? Note { get; init; }
}

public sealed class Exercise
{
    public required int Number { get; init; }
    public required string Title { get; init; }
    public required string Database { get; init; }
    /// <summary>Ghi chú chung cho mọi câu của bài, hiển thị dưới câu hỏi.</summary>
    public string? Context { get; init; }
    /// <summary>Lược đồ CSDL của bài (nếu có), hiển thị trong tab riêng.</summary>
    public string? Schema { get; init; }
    public required IReadOnlyList<ExerciseItem> Items { get; init; }

    public bool HasSubItems => Items.Count > 1;
}
