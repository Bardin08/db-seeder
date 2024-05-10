namespace DbSeeder.Data.Sql;

/// <summary>
/// Represents options for generating bulk insert SQL queries.
/// </summary>
/// <param name="TotalRecords">The total number of records to generate.</param>
/// <param name="RecordsPerInsert">The number of records per insert query.</param>
public record BulkInsertOptions(int TotalRecords, int RecordsPerInsert);