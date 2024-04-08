using System.Buffers;

namespace DbSeeder;

public class Constants
{
    // TODO: After .NET 9 release can be replaced with SearchValues<string>
    public static HashSet<string> Keywords =
    [
        "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP", "FROM", "WHERE", "AND", "OR", "NOT", "IN",
        "JOIN", "ON", "GROUP BY", "ORDER BY", "ASC", "DESC", "AS", "NULL", "TRUE", "FALSE"
    ];
    
    public static SearchValues<char> Punctuation = SearchValues.Create("(),.;");
}