using System.Buffers;

namespace DbSeeder;

public static class Constants
{
    // TODO: After .NET 9 release can be replaced with SearchValues<string>
    public static readonly HashSet<string> Keywords =
    [
        "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP", "FROM", "WHERE",
        "JOIN", "ON", "GROUP BY", "ORDER BY", "ASC", "DESC", "AS", "NULL", "TRUE", "FALSE", "TABLE"
    ];

    // TODO: After .NET 9 release can be replaced with SearchValues<string>
    public static readonly HashSet<string> Operators =
    [
        "AND", "OR", "NOT", "IN"
    ];

    public static readonly HashSet<string> Constraints =
    [
        "PRIMARY KEY", "FOREIGN KEY", "NOT NULL", "UNIQUE"
    ];

    public static readonly SearchValues<char> Punctuation = SearchValues.Create("(),.;");
}