using System.Collections.Concurrent;
using DbSeeder.Schema;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Security;

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
            var (_, func) = GeneratorFactory.GetGeneratorByColumn(col);
            generatedValues.Add(col.Name, func().ToString()!);
        }

        var hasReferencedTables = table.ForeignKeys.Any();
        if (!hasReferencedTables)
        {
            return new InsertSqlQuery(table.Name, generatedValues);
        }


        var relatedDataInsertQueries = new List<InsertSqlQuery>();
        foreach (var fk in table.ForeignKeys)
        {
            var generatedContext = _generationContextBuffer[generationContextId];
            if (generatedContext.TryGetValue(fk.RefTable.Name, out var existedRecord))
            {
                generatedValues[fk.Column.Name] = existedRecord.Value[fk.RefColumn.Name];
                continue;
            }

            var relatedDataInsertQuery = GenerateRelatedData(
                generationContextId, generatedValues, fk);

            if (relatedDataInsertQuery is not null)
            {
                relatedDataInsertQueries.Add(relatedDataInsertQuery);
            }
        }

        return new InsertSqlQuery(table.Name, generatedValues, relatedDataInsertQueries.AsReadOnly());
    }

    private InsertSqlQuery? GenerateRelatedData(
        Guid generatedContextId,
        IReadOnlyDictionary<string, string> referencerValues,
        ForeignKey fk)
    {
        var generatedContext = _generationContextBuffer[generatedContextId];
        if (generatedContext.TryGetValue(fk.RefTable.Name, out var existedRecord))
        {
            return null;
        }

        var insertSqlQuery = GenerateInternal(fk.RefTable, generatedContextId);

        // set up the proper reference (col == refCol value);
        // To get the col's value that was generated earlier, we have to pass the context
        var valuesWithValidFkValue = insertSqlQuery.Value.ToDictionary();
        valuesWithValidFkValue[fk.RefColumn.Name] = referencerValues[fk.Column.Name];
        insertSqlQuery = insertSqlQuery with { Value = valuesWithValidFkValue };

        generatedContext.TryAdd(insertSqlQuery.Table, insertSqlQuery);

        var hasReferencedTables = fk.RefTable.ForeignKeys.Any();
        if (hasReferencedTables)
        {
            foreach (var intFk in fk.RefTable.ForeignKeys)
            {
                var relatedDataInsertQuery = GenerateRelatedData(
                    generatedContextId, insertSqlQuery.Value, intFk);

                if (relatedDataInsertQuery is not null)
                {
                    insertSqlQuery = insertSqlQuery with
                    {
                        RelatedQueries = [relatedDataInsertQuery]
                    };
                }
            }
        }

        return insertSqlQuery;
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