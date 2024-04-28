using DbSeeder.Schema;

namespace DbSeeder.Data;

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

/// <summary>
/// Represents options for generating bulk insert SQL queries.
/// </summary>
/// <param name="TotalRecords">The total number of records to generate.</param>
/// <param name="RecordsPerInsert">The number of records per insert query.</param>
public record BulkInsertOptions(int TotalRecords, int RecordsPerInsert);

internal class SqlQueryGenerator : ISqlDataGenerator
{
    public InsertSqlQuery Generate(Table table)
    {
        if (table.PrimaryKey is null)
        {
            throw new NotImplementedException("Currently unsupported. Check GitHub issue #21");
        }

        var isPkAutoIncremental = table.PrimaryKey.Column.IsAutoIncrement;
        var cols = table.Columns.AsEnumerable();

        if (isPkAutoIncremental)
        {
            cols = cols.Where(c => !c.IsPrimaryKey);
        }

        var dict = new Dictionary<string, string>();
        foreach (var col in cols.ToList())
        {
            var (_, func) = GeneratorFactory.GetGeneratorByColumn(col);
            dict.Add(col.Name, func().ToString());
        }

        return new InsertSqlQuery(table.Name, Value: dict);
    }



    public IAsyncEnumerable<InsertSqlQuery> GenerateMultiple(Table table, int count)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<BulkInsertSqlQuery> GenerateBulk(Table table, BulkInsertOptions options)
    {
        throw new NotImplementedException();
    }
}

internal static class GeneratorFactory
{
    private static readonly string[] StringTypes = ["text", "varchar", "nvarchar", "char"];
    private static readonly string[] NumeralTypes = ["int", "long", "bit"];

    public static (Type, Func<object>) GetGeneratorByColumn(Column col)
    {
        if (StringTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase))
        {
            return (typeof(string), () => Guid.NewGuid().ToString("N"));
        }

        if (NumeralTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase)) // add type limits check
        {
            return (typeof(long), () => Random.Shared.NextInt64() * 17);
        }

        throw new NotImplementedException($"{col.DataType} is not currently supported");
    }
}



public record InsertSqlQuery(
    string Table,
    IReadOnlyDictionary<string, string> Value) // col: value
{
    public override string ToString()
    {
        return $"insert into {Table}({GetInsertCols()}) " +
               $"value ({GetInsertValues()});";
    }

    private string GetInsertCols()
    {
        return string.Join(",", Value.Keys);
    }

    private string GetInsertValues()
    {
        return string.Join(",", Value.Values);
    }
}

// --> insert into {table}
//     value ( { string.Join(",", Values[i]) } );

public record BulkInsertSqlQuery(
    string Table,
    IReadOnlyList<IReadOnlyDictionary<string, string>> Values) // [col: value]
{

}

// --> insert into {table}
//     values (
//          foreach Values[i]: {string.Join(",", Values[i])}
//     );
