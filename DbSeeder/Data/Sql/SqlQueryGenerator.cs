using System.Collections.Concurrent;
using DbSeeder.Schema;

namespace DbSeeder.Data.Sql;

internal class SqlQueryGenerator : ISqlDataGenerator
{
    private readonly ConcurrentDictionary<Guid, Dictionary<string, InsertSqlQuery>> _generationContextBuffer = [];

    public InsertSqlQuery Generate(Table table)
    {
        if (table.PrimaryKey is null)
        {
            throw new NotImplementedException("Currently unsupported. Check GitHub issue #21");
        }

        var generationContextId = Guid.NewGuid();
        var generationContext = new Dictionary<string, InsertSqlQuery>();
        _generationContextBuffer.GetOrAdd(generationContextId, generationContext);

        return GenerateInternal(table, generationContextId);
    }

    private InsertSqlQuery GenerateInternal(Table table, Guid generationContextId)
    {
        var isPkAutoIncremental = table.PrimaryKey!.Column.IsAutoIncrement;
        var cols = table.Columns.AsEnumerable();

        if (isPkAutoIncremental)
        {
            cols = cols.Where(c => !c.IsPrimaryKey);
        }

        var generatedValues = new Dictionary<string, string>();
        foreach (var col in cols.ToList())
        {
            var value = GeneratorFactory.GetGeneratorByColumnV2(col);
            generatedValues.Add(col.Name, value?.ToString() ?? "null");
        }

        var hasReferencedTables = table.ForeignKeys.Any();
        if (!hasReferencedTables)
        {
            return new InsertSqlQuery(table.Name, generatedValues);
        }

        return GenerateForeignSubKeys(table, generationContextId, generatedValues);
    }

    private InsertSqlQuery? GenerateForeignKey(
        Guid generatedContextId,
        IReadOnlyDictionary<string, string> referencerValues,
        ForeignKey fk)
    {
        var generatedContext = _generationContextBuffer[generatedContextId];
        if (generatedContext.ContainsKey(fk.RefTable.Name))
        {
            return null;
        }

        var insertSqlQuery = GenerateInternal(fk.RefTable, generatedContextId);
        insertSqlQuery = UpdateInsertQueryValues(insertSqlQuery, referencerValues, fk.RefColumn.Name, fk.Column.Name);

        generatedContext.TryAdd(insertSqlQuery.Table, insertSqlQuery);

        var hasReferencedTables = fk.RefTable.ForeignKeys.Any();
        return !hasReferencedTables
            ? insertSqlQuery
            : GenerateForeignSubKeys(fk.RefTable, generatedContextId, insertSqlQuery.Value.ToDictionary());
    }

    private static InsertSqlQuery UpdateInsertQueryValues(
        InsertSqlQuery insertSqlQuery,
        IReadOnlyDictionary<string, string> referencerValues,
        string src,
        string dest)
    {
        var updatedValue = new Dictionary<string, string>(insertSqlQuery.Value)
        {
            [src] = referencerValues[dest]
        };
        insertSqlQuery = insertSqlQuery with { Value = updatedValue };
        return insertSqlQuery;
    }

    private InsertSqlQuery GenerateForeignSubKeys(
        Table table,
        Guid generationContextId,
        Dictionary<string, string> generatedValues)
    {
        var relatedDataInsertQueries = new List<InsertSqlQuery>();
        foreach (var fk in table.ForeignKeys)
        {
            var generatedContext = _generationContextBuffer[generationContextId];
            if (generatedContext.TryGetValue(fk.RefTable.Name, out var existedRecord))
            {
                generatedValues[fk.Column.Name] = existedRecord.Value[fk.RefColumn.Name];
                continue;
            }

            var relatedDataInsertQuery = GenerateForeignKey(
                generationContextId, generatedValues, fk);

            if (relatedDataInsertQuery is not null)
            {
                relatedDataInsertQueries.Add(relatedDataInsertQuery);
            }
        }

        return new InsertSqlQuery(table.Name, generatedValues, relatedDataInsertQueries.AsReadOnly());
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
