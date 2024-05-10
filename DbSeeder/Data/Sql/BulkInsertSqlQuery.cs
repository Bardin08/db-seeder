namespace DbSeeder.Data.Sql;

public record BulkInsertSqlQuery(
    string Table,
    IReadOnlyList<IReadOnlyDictionary<string, string>> Values) // [col: value]
{

}