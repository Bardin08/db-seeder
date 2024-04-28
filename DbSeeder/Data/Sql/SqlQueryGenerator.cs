using DbSeeder.Schema;

namespace DbSeeder.Data.Sql;

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

// --> insert into {table}
//     value ( { string.Join(",", Values[i]) } );

// --> insert into {table}
//     values (
//          foreach Values[i]: {string.Join(",", Values[i])}
//     );
