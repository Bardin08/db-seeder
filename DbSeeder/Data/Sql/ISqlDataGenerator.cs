using DbSeeder.Schema;

namespace DbSeeder.Data.Sql;

public interface ISqlDataGenerator : IDataGenerator<InsertSqlQuery>
{
    /// <summary>
    /// Generates bulk insert SQL queries for inserting multiple instances of objects that match the SQL schema's entry for the specified table.
    /// </summary>
    /// <param name="table">The table schema for which to generate bulk insert queries.</param>
    /// <param name="options">Options specifying the total number of records and the number of records per insert.</param>
    /// <returns>An asynchronous stream of bulk insert SQL queries.</returns>
    /// <remarks>Any constraints encountered are only applicable within the scope of this method call.</remarks>
    IAsyncEnumerable<BulkInsertSqlQuery> GenerateBulk(Table table, BulkInsertOptions options);
}