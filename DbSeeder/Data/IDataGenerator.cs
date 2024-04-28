using DbSeeder.Schema;

namespace DbSeeder.Data;

public interface IDataGenerator<out TOut>
{
    /// <summary>
    /// Generates a single instance of an object that matches the SQL schema's entry for the specified table.
    /// </summary>
    /// <param name="table">The table schema for which to generate an instance.</param>
    /// <returns>An object representing an instance of the SQL schema's entry.</returns>
    /// <remarks>Any constraints encountered are only applicable within the scope of this method call.</remarks>
    TOut Generate(Table table);

    /// <summary>
    /// Generates the requested number of records for the specified table.
    /// </summary>
    /// <param name="table">The table schema for which to generate records.</param>
    /// <param name="count">The number of records to generate.</param>
    /// <returns>An asynchronous stream of objects representing the generated records.</returns>
    /// <remarks>Any constraints encountered are only applicable within the scope of this method call.</remarks>
    IAsyncEnumerable<TOut> GenerateMultiple(Table table, int count);
}
